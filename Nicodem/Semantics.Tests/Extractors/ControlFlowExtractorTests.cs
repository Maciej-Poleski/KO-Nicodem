using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Nicodem.Semantics.AST;

namespace Semantics.Tests.Extractors
{
    [TestFixture]
    class ControlFlowExtractorTests
    {

        ExpressionNode block_expression_operator_and_if;

        private OperatorNode MakeOperator(OperatorType _operator, params ExpressionNode[] _arguments){
            OperatorNode made_one = new OperatorNode();
            made_one.Arguments = new List<ExpressionNode>(_arguments);
            made_one.Operator = _operator;
            return made_one;
        }

        [TestFixtureSetUp]
        public void InitAST()
        {
            VariableUseNode _a = new VariableUseNode();
            _a.Name = "a";
            VariableUseNode _2 = new VariableUseNode();
            _2.Name = "2";
            VariableUseNode _3 = new VariableUseNode();
            _3.Name = "3";
            VariableUseNode _5 = new VariableUseNode();
            _5.Name = "5";

            //build simple ast for a+2; a+(if(a>3){a+2;}else{a+5;})+5;
            OperatorNode first_statement = MakeOperator(OperatorType.PLUS, _a, _2); 
            OperatorNode _a_gret_3 = MakeOperator(OperatorType.GREATER, _a, _3);
            OperatorNode _a_plus_2 = MakeOperator(OperatorType.PLUS, _a, _2);
            IfNode _if = new IfNode();
        }

        [Test]
        public void BasicIfTest()
        {
            
            //check number of nodes in graph
        }
        
    }
}
