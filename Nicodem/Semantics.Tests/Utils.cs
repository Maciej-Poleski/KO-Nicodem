using Nicodem.Semantics.AST;
using System;
using System.Collections.Generic;

namespace Semantics.Tests
{
	internal static class Utils
	{
		#region types

		internal enum PrimitiveType {
			Int,
			Byte,
			Char,
			Bool,
			Void
		}
			
		internal enum Mutablitity {
			Constant,
			Mutable
		}

		internal static NamedTypeNode MakePrimitiveType(PrimitiveType type, Mutablitity isConstant) {
			NamedTypeNode result = null;
			switch (type)
			{
			case PrimitiveType.Int:
				result = NamedTypeNode.IntType();
				break;
			case PrimitiveType.Byte:
				result = NamedTypeNode.ByteType();
				break;
			case PrimitiveType.Char:
				result = NamedTypeNode.CharType();
				break;
			case PrimitiveType.Bool:
				result = NamedTypeNode.BoolType();
				break;
			case PrimitiveType.Void:
				result = NamedTypeNode.VoidType();
				break;
			default:
				throw new Exception ("Wrong primitive type");
			}
			result.IsConstant = isConstant == Mutablitity.Constant;
			return result;
		}

		#region mutable

		internal static NamedTypeNode MakeMutableInt() {
			return MakePrimitiveType (PrimitiveType.Int, Mutablitity.Mutable);
		}

		internal static NamedTypeNode MakeMutableByte() {
			return MakePrimitiveType (PrimitiveType.Byte, Mutablitity.Mutable);
		}

		internal static NamedTypeNode MakeMutableChar() {
			return MakePrimitiveType (PrimitiveType.Char, Mutablitity.Mutable);
		}

		internal static NamedTypeNode MakeMutableBool() {
			return MakePrimitiveType (PrimitiveType.Bool, Mutablitity.Mutable);
		}

		internal static NamedTypeNode MakeMutableVoid() {
			return MakePrimitiveType (PrimitiveType.Void, Mutablitity.Mutable);
		}

		internal static NamedTypeNode MakeMutableRecordType(string typeName) {
			return new NamedTypeNode {
				Name = typeName,
				IsConstant = false
			};
		}

		#endregion

		#region constant

		internal static NamedTypeNode MakeConstantInt() {
			return MakePrimitiveType (PrimitiveType.Int, Mutablitity.Constant);
		}

		internal static NamedTypeNode MakeConstantByte() {
			return MakePrimitiveType (PrimitiveType.Byte, Mutablitity.Constant);
		}

		internal static NamedTypeNode MakeConstantChar() {
			return MakePrimitiveType (PrimitiveType.Char, Mutablitity.Constant);
		}

		internal static NamedTypeNode MakeConstantBool() {
			return MakePrimitiveType (PrimitiveType.Bool, Mutablitity.Constant);
		}

		internal static NamedTypeNode MakeConstantVoid() {
			return MakePrimitiveType (PrimitiveType.Void, Mutablitity.Constant);
		}

		internal static NamedTypeNode MakeConstantRecordType(string typeName) {
			return new NamedTypeNode {
				Name = typeName,
				IsConstant = true
			};
		}

		#endregion

		#endregion

		#region literals

		internal static AtomNode IntLiteral(int value) {
			return new AtomNode (MakeConstantInt ()) { Value = value.ToString () };
		}

		internal static AtomNode CharLiteral(char value) {
			return new AtomNode (MakeConstantChar ()) { Value = "'" + value + "'" };
		}

		internal static AtomNode BoolLiteral(bool value) {
			return new AtomNode (MakeConstantBool ()) { Value = value ? "true" : "false" };
		}

		internal static AtomNode ByteLiteral(int value) {
			return new AtomNode (MakeConstantByte ()) { Value = value.ToString () };
		}

		#endregion

		#region declaration

		internal static VariableDeclNode Declaration(string name, NamedTypeNode type) {
			return new VariableDeclNode {
				Name = name,
				Type = type,
				ExpressionType = type
			};
		}

		internal static VariableDeclNode DeclareInt(string name) {
			return Declaration (name, MakeMutableInt ());
		}

		internal static VariableDeclNode DeclareChar(string name) {
			return Declaration (name, MakeMutableChar ());
		}

		internal static VariableDeclNode DeclareByte(string name) {
			return Declaration (name, MakeMutableByte ());
		}

		internal static VariableDeclNode DeclareBool(string name) {
			return Declaration (name, MakeMutableBool ());
		}

		internal static RecordTypeDeclarationNode DeclareRecordType(string typeName, params RecordTypeFieldDeclarationNode[] fields) {
			return new RecordTypeDeclarationNode {
				Name = new NamedTypeNode { Name = typeName, IsConstant = false },
				Fields = fields
			};
		}

		internal static RecordTypeFieldDeclarationNode DeclareField(TypeNode type, string fieldName) {
			return new RecordTypeFieldDeclarationNode {
				Type = type,
				Name = fieldName
			};
		}

		#endregion

		#region definition

		internal static VariableDefNode Definition(VariableDeclNode decl, ExpressionNode value) {
			return new VariableDefNode {
				Name = decl.Name,
				Type = decl.Type,
				ExpressionType = decl.ExpressionType,
				Value = value
			};
		}

		internal static VariableDefNode DefineInt(string name, int val) {
			return Definition (Declaration (name, MakeMutableInt ()), IntLiteral (val));
		}

		internal static VariableDefNode DefineChar(string name, char val) {
			return Definition (Declaration (name, MakeMutableChar ()), CharLiteral (val));
		}

		internal static VariableDefNode DefineByte(string name, byte val) {
			return Definition (Declaration (name, MakeMutableByte ()), ByteLiteral (val));
		}

		internal static VariableDefNode DefineBool(string name, bool val) {
			return Definition (Declaration (name, MakeMutableBool ()), BoolLiteral (val));
		}

		internal static VariableDefNode DefineConstantInt(string name, int val) {
			return Definition (Declaration (name, MakeConstantInt ()), IntLiteral (val));
		}

		internal static VariableDefNode DefineConstantChar(string name, char val) {
			return Definition (Declaration (name, MakeConstantChar ()), CharLiteral (val));
		}

		internal static VariableDefNode DefineConstantByte(string name, byte val) {
			return Definition (Declaration (name, MakeConstantByte ()), ByteLiteral (val));
		}

		internal static VariableDefNode DefineConstantBool(string name, bool val) {
			return Definition (Declaration (name, MakeConstantBool ()), BoolLiteral (val));
		}

		internal static RecordVariableDefNode DefineRecord(string typeName, string varName, params RecordVariableFieldDefNode[] fields) {
			return new RecordVariableDefNode {
				Type = MakeMutableRecordType(typeName),
				Name = varName,
				Fields = fields
			};
		}

		internal static RecordVariableDefNode DefineConstantRecord(string typeName, string varName, params RecordVariableFieldDefNode[] fields) {
			return new RecordVariableDefNode {
				Type = MakeConstantRecordType(typeName),
				Name = varName,
				Fields = fields
			};
		}

		internal static RecordVariableFieldDefNode field(string name, ExpressionNode value) {
			return new RecordVariableFieldDefNode {
				FieldName = name,
				Value = value
			};
		}

		#endregion

		internal static VariableUseNode Usage(VariableDeclNode def, bool inherit_decl = true) {
			return new VariableUseNode {
				Name = def.Name,
				ExpressionType = def.ExpressionType,
				Declaration = inherit_decl ? def : null
			};
		}

		internal static RecordVariableFieldUseNode UsageField(RecordVariableDefNode def, string field, bool inherit_decl = true) {
			return new RecordVariableFieldUseNode {
				RecordName = def.Name,
				Field = field,
				ExpressionType = def.ExpressionType,
				Definition = inherit_decl ? def : null
			};
		}

		internal static OperatorNode Assignment(ExpressionNode node1, ExpressionNode node2) {
			return new OperatorNode {
				Operator = OperatorType.ASSIGN,
				Arguments = new [] { node1, node2 }
			};
		}

		internal static OperatorNode Add(ExpressionNode node1, ExpressionNode node2) {
			return new OperatorNode {
				Operator = OperatorType.PLUS,
				Arguments = new [] { node1, node2 }
			};
		}

		internal static OperatorNode Sub(ExpressionNode node1, ExpressionNode node2) {
			return new OperatorNode {
				Operator = OperatorType.MINUS,
				Arguments = new [] { node1, node2 }
			};
		}

		internal static OperatorNode Less(ExpressionNode node1, ExpressionNode node2) {
			return new OperatorNode{
				Operator = OperatorType.LESS,
				Arguments = new [] {node1, node2}
			};
		}

		internal static OperatorNode Greater(ExpressionNode node1, ExpressionNode node2) {
			return new OperatorNode{
				Operator = OperatorType.GREATER,
				Arguments = new [] {node1, node2}
			};
		}

		internal static OperatorNode Modulo(ExpressionNode node1, ExpressionNode node2) {
			return new OperatorNode{
				Operator = OperatorType.MOD,
				Arguments = new [] {node1, node2}
			};
		}

		internal static IfNode If(ExpressionNode cond, ExpressionNode then) {
			return new IfNode {
				Condition = cond,
				Then = then,
			};
		}

		internal static IfNode IfElse(ExpressionNode cond, ExpressionNode then, ExpressionNode _else) {
			return new IfNode {
				Condition = cond,
				Then = then,
				Else = _else
			};
		}

		internal static WhileNode While(ExpressionNode cond, ExpressionNode body) {
			return new WhileNode {
				Condition = cond,
				Body = body
			};
		}

		internal static IEnumerable<VariableDeclNode> parameters(params VariableDeclNode[] p) {
			return p;
		}

		internal static BlockExpressionNode body(params ExpressionNode[] exprs) {
			return new BlockExpressionNode { Elements = exprs };
		}

		internal static FunctionDefinitionNode FunctionDef(string name, IEnumerable<VariableDeclNode> parameters, NamedTypeNode type, BlockExpressionNode body) {
			return new FunctionDefinitionNode {
				Name = name,
				Parameters = parameters,
				ResultType = type,
				Body = body
			};
		}

		internal static FunctionCallNode FunctionCall(string name, params ExpressionNode[] args) {
			return new FunctionCallNode {
				Name = name,
				Arguments = args
			};
		}

		internal static ProgramNode Program(params FunctionDefinitionNode[] defs) {
			return new ProgramNode (new LinkedList<FunctionDefinitionNode> (defs), new LinkedList<RecordTypeDeclarationNode>());
		}

		internal static ProgramNode Program(IEnumerable<FunctionDefinitionNode> defs, IEnumerable<RecordTypeDeclarationNode> records) {
			return new ProgramNode (new LinkedList<FunctionDefinitionNode> (defs), new LinkedList<RecordTypeDeclarationNode>(records));
		}

		/*
		 * f(int mutable a, int mutable b) -> int
         * {
         *   int mutable c = -1
         *   byte mutable d = 0
         *   g(char g, bool e) -> void
         *   {
         *     int mutable f = 5
         *     a = 1
         *     e = false
         *     f = 3
         *     d = 2
         *   }
         *   a = 1
         *   b = 2
         *   c = 3
         *   d = 4
         *   b
         * }
         */
		internal static ProgramNode AST_NestedUses() {
			var fParamA = DeclareInt ("a");
			var fParamB = DeclareInt ("b");
			var fVarC = DefineInt ("c", -1);
			var fVarD = DefineByte ("d", 0);

			// function g
			var gParamG = DeclareChar ("g");
			var gParamE = DeclareBool ("e");
			var gBodyEx1 = DefineInt ("f", 5);
			var gBodyEx2 = Assignment (fParamA, IntLiteral (1));
			var gBodyEx3 = Assignment (gParamE, BoolLiteral (false));
			var gBodyEx4 = Assignment (gBodyEx1, IntLiteral (3));
			var gBodeEx5 = Assignment (fVarD, IntLiteral (2));
			var gFunction = FunctionDef (
				"g", parameters (gParamG, gParamE), MakeConstantVoid (),
				body (gBodyEx1, gBodyEx2, gBodyEx3, gBodyEx4, gBodeEx5)
			);

			// function f
			var fBodyEx1 = fVarC;
			var fBodyEx2 = fVarD;
			var fBodyEx3 = gFunction;
			var fBodyEx4 = Assignment (fParamA, IntLiteral (1));
			var fBodyEx5 = Assignment (fParamB, IntLiteral (2));
			var fBodyEx6 = Assignment (fBodyEx1, IntLiteral (3));
			var fBodyEx7 = Assignment (fBodyEx2, ByteLiteral (4));
			var fBodyEx8 = Usage (fParamB);
			var fFunction = FunctionDef (
				"f", parameters (fParamA, fParamB), MakeConstantInt (),
				body (fBodyEx1, fBodyEx2, fBodyEx3, fBodyEx4, fBodyEx5, fBodyEx6, fBodyEx7, fBodyEx8)
			);

			return Program (fFunction);
		}

		/*
		 * f(int mutable a) -> int
         * {
         *   a = 1
         * }
         */
		internal static ProgramNode AST_SimpleFunction() {
			var fParamA = DeclareInt ("a");
			var fBody = Assignment (fParamA, IntLiteral (1));
			var fFunction = FunctionDef ("f", parameters (fParamA), MakeConstantInt (), body (fBody));
			return Program (fFunction);
		}

		/*
		 * f(int mutable a) -> int
         * {
         * }
         */
		internal static ProgramNode AST_EmptyFunction() {
			var fParamA = DeclareInt ("a");
			var fFunction = FunctionDef ("f", parameters (fParamA), MakeConstantInt (), body ());
			return Program (fFunction);
		}

		/*
		 * f(int mutable a) -> int
         * {
         *   g(int mutable b) -> byte
         *   {
         *     1
         *   }
         * }
         */
		internal static ProgramNode AST_NestedFunction() {
			var fParam = DeclareInt ("a");
			var gParam = DeclareInt ("b");
			var gExpr = IntLiteral (1);
			var gFunction = FunctionDef ("g", parameters (gParam), MakeConstantByte (), body (gExpr));
			var fFunction = FunctionDef ("f", parameters (fParam), MakeConstantInt (), body (gFunction));
			return Program (fFunction);
		}
	}
}

