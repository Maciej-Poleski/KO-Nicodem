﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Nicodem.Source;

namespace Nicodem.Lexer
{
    internal class LexerCompiler
    {
        internal static Assembly getCompiledLexer<TDfa, TDfaState>(TDfa dfa)
            where TDfa : AbstractDfa<TDfaState, char>
            where TDfaState : AbstractDfaState<TDfaState, char>
        {
            var compileUnit = new CodeCompileUnit();
            var implementationNamespace = new CodeNamespace("Nicodem.Lexer.Compiled");
            compileUnit.Namespaces.Add(implementationNamespace);
            implementationNamespace.Imports.Add(new CodeNamespaceImport("System"));
            implementationNamespace.Imports.Add(new CodeNamespaceImport("Nicodem.Lexer")); // for LexerResult
            implementationNamespace.Imports.Add(new CodeNamespaceImport("Nicodem.Source")); // for input
            var compiledLexerClass = new CodeTypeDeclaration("Lexer");
            implementationNamespace.Types.Add(compiledLexerClass);
            var processMethod = GenerateProcessMethod<TDfa, TDfaState>(dfa);
            compiledLexerClass.Members.Add(processMethod);

            var codeProvider = new CSharpCodeProvider();
            var compilerParams = new CompilerParameters();
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = true;
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof (Lexer)).Location);
            compilerParams.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof (IOrigin)).Location);
            var compileResult = codeProvider.CompileAssemblyFromDom(compilerParams, compileUnit);
            if (compileResult.Errors.Count > 0)
            {
                Console.WriteLine("Lexer compile error");
                foreach (var error in compileResult.Errors)
                {
                    Console.WriteLine("  {0}", error);
                }
                return null;
            }
            return compileResult.CompiledAssembly;
        }

        private static CodeMemberMethod GenerateProcessMethod<TDfa, TDfaState>(TDfa dfa)
            where TDfa : AbstractDfa<TDfaState, char>
            where TDfaState : AbstractDfaState<TDfaState, char>
        {
            var processMethod = new CodeMemberMethod();
            processMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            processMethod.Name = "Process";
            processMethod.ReturnType = new CodeTypeReference(typeof (LexerResult));
            processMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (IOrigin), "origin"));
            var originArgument = new CodeArgumentReferenceExpression("origin");
            var resultVariable = new CodeVariableReferenceExpression("result");
            processMethod.Statements.AddRange(new[]
            {
                new CodeVariableDeclarationStatement(typeof (LexerState), "result",
                    new CodeObjectCreateExpression(typeof (LexerState), originArgument)),
                GenerateMainLoop<TDfa, TDfaState>(dfa, resultVariable),
                new CodeMethodReturnStatement(new CodeMethodInvokeExpression(resultVariable, "MakeLexerResult"))
            });

            return processMethod;
        }

        private static CodeStatement GenerateMainLoop<TDfa, TDfaState>(TDfa dfa,
            CodeVariableReferenceExpression lexerStateVariable)
            where TDfa : AbstractDfa<TDfaState, char>
            where TDfaState : AbstractDfaState<TDfaState, char>
        {
            var switchGenerator = new SwitchGenerator<TDfa, TDfaState>(dfa, lexerStateVariable);
            return switchGenerator.MakeStatement();
        }

        private class SwitchGenerator<TDfa, TDfaState>
            where TDfa : AbstractDfa<TDfaState, char>
            where TDfaState : AbstractDfaState<TDfaState, char>
        {
            private readonly Dictionary<TDfaState, int> _dfaStateToNativeState = new Dictionary<TDfaState, int>();
            private readonly CodeVariableReferenceExpression _lexerStateVariable;
            private readonly Dictionary<int, TDfaState> _nativeStateToDfaState = new Dictionary<int, TDfaState>();
            private readonly List<CodeStatement> _switchStatements = new List<CodeStatement>();
            private int _nextState;

            public SwitchGenerator(TDfa dfa, CodeVariableReferenceExpression lexerStateVariable)
            {
                _lexerStateVariable = lexerStateVariable;
                ReserveIds(dfa.Start);
                for (var i = 0; i < _nextState; ++i)
                {
                    GenerateState(lexerStateVariable, i);
                }
            }

            private void GenerateState(CodeVariableReferenceExpression lexerStateVariable, int stateId)
            {
                _switchStatements.Add(new CodeSnippetStatement("case " + stateId + ":"));
                var dfaState = _nativeStateToDfaState[stateId];
                Debug.Assert(dfaState.Transitions[0].Key == '\0', "Missing transition in DFA");
                _switchStatements.Add(
                    GenerateBranchingForState(lexerStateVariable, 0, dfaState.Transitions.Length, dfaState.Transitions));
                _switchStatements.Add(new CodeSnippetStatement("break;"));
            }

            private CodeStatement GenerateBranchingForState(CodeVariableReferenceExpression lexerStateVariable,
                int from, int to, KeyValuePair<char, TDfaState>[] transitions)
            {
                if (from + 1 == to)
                {
                    CodeStatement code;
                    // there is no dead DFA state - workaround - use pseudo dead
                    if (transitions[from].Value.IsPseudoDead())
                    {
                        code = new CodeExpressionStatement(
                            new CodeMethodInvokeExpression(lexerStateVariable, "EnterDeadState"));
                    }
                    else
                    {
                        code = new CodeExpressionStatement(
                            new CodeMethodInvokeExpression(lexerStateVariable, "EnterState",
                                new CodePrimitiveExpression(_dfaStateToNativeState[transitions[from].Value]),
                                new CodePrimitiveExpression(transitions[from].Value.Accepting)));
                    }
                    return code;
                }
                var currentCharExpr = new CodePropertyReferenceExpression(lexerStateVariable, "CurrentChar");
                var part = (to + from)/2;
                return new CodeConditionStatement(new CodeBinaryOperatorExpression(currentCharExpr,
                    CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(transitions[part].Key)),
                    new[] {GenerateBranchingForState(lexerStateVariable, from, part, transitions)},
                    new[] {GenerateBranchingForState(lexerStateVariable, part, to, transitions)});
            }

            // DFA serialization strategy.
            private void ReserveIds(TDfaState dfaState)
            {
                // (pseudo) dead states are not important - remove them
                if (_dfaStateToNativeState.ContainsKey(dfaState) || dfaState.IsPseudoDead())
                    return;
                var id = _nextState++;
                _dfaStateToNativeState[dfaState] = id;
                _nativeStateToDfaState[id] = dfaState;
                foreach (var transition in dfaState.Transitions)
                {
                    ReserveIds(transition.Value);
                }
            }

            public CodeStatement MakeStatement()
            {
                var impl = new CodeStatement[_switchStatements.Count + 2];
                impl[0] = new CodeSnippetStatement("switch(" + _lexerStateVariable.VariableName + ".CurrentState) {");
                for (var i = 0; i < _switchStatements.Count; ++i)
                {
                    impl[i + 1] = _switchStatements[i];
                }
                impl[_switchStatements.Count + 1] = new CodeSnippetStatement("}");
                return new CodeIterationStatement(new CodeSnippetStatement(),
                    new CodeMethodInvokeExpression(_lexerStateVariable, "ContinueIteration"), new CodeSnippetStatement(),
                    impl);
            }
        }
    }

    // Embedded into Compiled Lexer - shorter implementation
    public class LexerState
    {
        private readonly IOriginReader _sourceReader; // keep in sync with _currentLocation
        private readonly List<Tuple<IFragment, int>> _tokens; // result
        private ILocation _currentLocation; // will be Failed Location (duplicates _sourceReader.CurrentLocation)
        private bool _failed;
        private ILocation _lastAcceptedLocation; // will be Last Parsed Location
        private uint _lastAcceptingCategory;
        private ILocation _lastAcceptingLocation; // Memento equivalent

        public LexerState(IOrigin origin)
        {
            _sourceReader = origin.GetReader();
            _currentLocation = _sourceReader.CurrentLocation;
            _lastAcceptedLocation = _sourceReader.CurrentLocation;
            _lastAcceptingLocation = _lastAcceptedLocation;
            _tokens = new List<Tuple<IFragment, int>>();
        }

        public int CurrentState { get; private set; }

        public char CurrentChar { get; private set; }

        public LexerResult MakeLexerResult()
        {
            return new LexerResult(
                from token in _tokens
                select new Tuple<IFragment, IEnumerable<int>>(token.Item1, new[] {token.Item2}),
                _lastAcceptedLocation, _currentLocation);
        }

        public bool ContinueIteration()
        {
            if (!_failed && _sourceReader.MoveNext())
            {
                CurrentChar = _sourceReader.CurrentCharacter;
                _currentLocation = _sourceReader.CurrentLocation;
                return true;
            }
            if (!_failed && (_lastAcceptingLocation != _lastAcceptedLocation))
            {
                EmitToken();
            }
            return false;
        }

        public void EnterState(int state, uint category)
        {
            CurrentState = state;
            if (category != 0)
            {
                _lastAcceptingCategory = category;
                _lastAcceptingLocation = _currentLocation;
            }
        }

        public void EnterDeadState()
        {
            if (_lastAcceptingLocation == _lastAcceptedLocation)
            {
                _failed = true;
                // CurrentState doesn't matter
            }
            else
            {
                EmitToken();
            }
        }

        private void EmitToken()
        {
            _tokens.Add(
                Tuple.Create(
                    _lastAcceptedLocation.Origin.MakeFragment(_lastAcceptedLocation, _lastAcceptingLocation),
                    ((int) _lastAcceptingCategory) - 1));
            _lastAcceptedLocation = _lastAcceptingLocation;
            _currentLocation = _lastAcceptedLocation;
            _sourceReader.CurrentLocation = _lastAcceptedLocation;
            CurrentState = 0;
        }
    }
}