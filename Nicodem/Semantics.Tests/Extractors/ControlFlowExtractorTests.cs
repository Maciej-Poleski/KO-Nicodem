using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Extractors;
using Nicodem.Semantics.ExpressionGraph;

namespace Semantics.Tests.Extractors
{
	[TestFixture]
	class ControlFlowExtractorTests
	{

		private OperatorNode MakeOperator (OperatorType _operator, params ExpressionNode[] _arguments)
		{
			OperatorNode made_one = new OperatorNode ();
			made_one.Arguments = new List<ExpressionNode> (_arguments);
			made_one.Operator = _operator;
			return made_one;
		}

		private VariableUseNode Var (String s)
		{
			VariableUseNode _var = new VariableUseNode ();
			_var.Name = s;
			return _var;
		}

		[TestFixtureSetUp]
		public void InitAST ()
		{
		}

		[Test]
		public void ControlFlowExtractor_NestedIfs_Test ()
		{
			// if(a > b) {if(a > b) {a} else {b}} else {if(a > b) {b} else {a}}

			var cond = MakeOperator (OperatorType.GREATER, Var ("a"), Var ("b"));

			var innerIf1 = new IfNode{ Condition = cond, Then = Var ("a"), Else = Var ("b") };
			var innerIf2 = new IfNode{ Condition = cond, Then = Var ("b"), Else = Var ("a") };

			var expr = new IfNode{ Condition = cond, Then = innerIf1, Else = innerIf2 };
			var cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (expr));
		}

		[Test]
		public void ControlFlowExtractor_NestedWhiles_Test ()
		{
			// while(a<b){a+=(while(c<b){c+=c}{c})}else{a}

			var cond1 = MakeOperator (OperatorType.LESS, Var ("a"), Var ("b"));
			var cond2 = MakeOperator (OperatorType.LESS, Var ("c"), Var ("b"));
			var body1 = MakeOperator (OperatorType.ASSIGN, Var ("c"), Utils.Add (Var ("c"), Var ("c")));

			var innerWhile = new WhileNode{ Condition = cond2, Body = body1, Else = Var ("c") };
			var body2 = MakeOperator (OperatorType.ASSIGN, Var ("a"), Utils.Add (Var ("a"), innerWhile));
			var outerWhile = new WhileNode{ Condition = cond1, Body = body2, Else = Var ("a") };
			
			var cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (outerWhile));
		}

		[Test]
		public void ControlFlowExtractor_WhileWithComplexCondition_Test ()
		{
			// while((if(a<b) {a} else {b})<c){a+=a}else{a}
		
			var cond1 = MakeOperator (OperatorType.LESS, Var ("a"), Var ("b"));
			var _if = new IfNode{ Condition = cond1, Then = Var ("a"), Else = Var ("b") };
			var cond2 = MakeOperator (OperatorType.LESS, _if, Var ("c"));
			var body = MakeOperator (OperatorType.ASSIGN, Var ("a"), Utils.Add (Var ("a"), Var ("a")));
			var expr = new WhileNode{ Condition = cond2, Body = body, Else = Var ("a") };

			var cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (expr));			
		}

		[Test]
		public void ControlFlowExtractor_IfWithWhileIncludedInCondition_Test ()
		{
			// if ((while(a < b) {a=a+c} else {a}) < d){a}else{b}
		
			var cond1 = MakeOperator (OperatorType.LESS, Var ("a"), Var ("b"));
			var body1 = MakeOperator (OperatorType.ASSIGN, Var ("a"), Utils.Add (Var ("a"), Var ("c")));
			var _while = new WhileNode{ Condition = cond1, Body = body1, Else = Var ("a") };
			var cond2 = MakeOperator (OperatorType.LESS, _while, Var ("d"));
			var expr = new IfNode{ Condition = cond2, Then = Var ("a"), Else = Var ("b") };

			var cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (expr));			
		}

		[Test]
		public void ControlFlowExtractor_IfsAndWhilesMixed_Test ()
		{
			// if (a < b) {while(a<c){a=b+c}else{a}} else {while(b<c){b=b+c}else{b}}

			var cond1 = MakeOperator (OperatorType.LESS, Var ("a"), Var ("b"));
			var cond2 = MakeOperator (OperatorType.LESS, Var ("a"), Var ("c"));
			var cond3 = MakeOperator (OperatorType.LESS, Var ("b"), Var ("c"));

			var body1 = MakeOperator (OperatorType.ASSIGN, Var ("a"), Utils.Add (Var ("b"), Var ("c")));
			var body2 = MakeOperator (OperatorType.ASSIGN, Var ("b"), Utils.Add (Var ("b"), Var ("c")));

			var _while1 = new WhileNode{ Condition = cond2, Body = body1, Else = Var ("a") };
			var _while2 = new WhileNode{ Condition = cond3, Body = body2, Else = Var ("b") };

			var expr = new IfNode{ Condition = cond1, Then = _while1, Else = _while2 };

			var cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (expr));			
		}

		[Test]
		public void ControlFlowExtractor_AllAtOnce_Test ()
		{
			// if(((while (a < b) {a=a+a} else {a}) + (while(b < c) {b=b+b} else {b})) < d) {a} else {b}

			var cond1 = MakeOperator (OperatorType.LESS, Var ("a"), Var ("b"));
			var cond2 = MakeOperator (OperatorType.LESS, Var ("b"), Var ("c"));

			var body1 = MakeOperator (OperatorType.ASSIGN, Var ("a"), Utils.Add (Var ("a"), Var ("a")));
			var body2 = MakeOperator (OperatorType.ASSIGN, Var ("b"), Utils.Add (Var ("b"), Var ("b")));

			var _while1 = new WhileNode{ Condition = cond1, Body = body1, Else = Var ("a") };
			var _while2 = new WhileNode{ Condition = cond2, Body = body2, Else = Var ("b") };

			var cond3 = MakeOperator (OperatorType.LESS, Utils.Add (_while1, _while2), Var ("d"));

			var expr = new IfNode{ Condition = cond3, Then = Var ("a"), Else = Var ("b") };
		}

		[Test]
		public void ControlFlowExtractor_SimpleIfInBlockExpression_Test ()
		{
			//[
			//a+2; 
			//a+(if(a>3){a+2;}else{a+5;})+5;
			//]

			OperatorNode first_statement = Utils.Add (Var ("a"), Var ("2"));
			OperatorNode _a_gret_3 = MakeOperator (OperatorType.GREATER, Var ("a"), Var ("3"));
			OperatorNode _a_plus_2 = Utils.Add (Var ("a"), Var ("2"));
			OperatorNode _a_plus_5 = Utils.Add (Var ("a"), Var ("5"));
			IfNode _if = new IfNode ();
			_if.Condition = _a_gret_3;
			_if.Then = _a_plus_2;
			_if.Else = _a_plus_5;
			OperatorNode second_statement = Utils.Add (Var ("a"), Utils.Add (_if, Var ("5")));
			BlockExpressionNode _if_block = new BlockExpressionNode ();
			_if_block.Elements = new List<ExpressionNode> { first_statement, second_statement };

			List<Vertex> cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (_if_block));

			Assert.AreEqual (6, cf_graph.Count);

			Assert.IsTrue (cf_graph [0].Expression is OperatorNode);
			Assert.IsTrue (cf_graph [1].Expression is OperatorNode);
			Assert.IsTrue (cf_graph [2].Expression is OperatorNode);
			Assert.IsTrue (cf_graph [3].Expression is OperatorNode);
			Assert.IsTrue (cf_graph [4].Expression is VariableUseNode);
			Assert.IsTrue (cf_graph [5].Expression is OperatorNode);

			Assert.IsTrue (cf_graph [1] is ConditionalJumpVertex);
		}

		[Test]
		public void ControlFlowExtractor_SimpleWhileInBlockExrpession_Test ()
		{
			//[
			//a-while(a<3){a+1}else{a+3}+5;
			//a*2;
			//]

			OperatorNode a_less_3 = MakeOperator (OperatorType.LESS, Var ("a"), Var ("3"));
			OperatorNode a_plus_1 = Utils.Add (Var ("a"), Var ("1"));
			OperatorNode a_plus_5 = Utils.Add (Var ("a"), Var ("5"));
			WhileNode _while = new WhileNode ();
			_while.Condition = a_less_3;
			_while.Body = a_plus_1;
			_while.Else = a_plus_5;
			OperatorNode _first_statement = Utils.Sub (Var ("a"), Utils.Add (_while, Var ("5")));
            
			OperatorNode _second_statement = MakeOperator (OperatorType.MUL, Var ("a"), Var ("2"));

			BlockExpressionNode _while_block = new BlockExpressionNode ();
			_while_block.Elements = new List<ExpressionNode>{ _first_statement, _second_statement };

			List<Vertex> cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (_while_block));

			Assert.AreEqual (6, cf_graph.Count);

			Assert.IsTrue (cf_graph [0].Expression is OperatorNode);
			Assert.IsTrue (cf_graph [1].Expression is OperatorNode);
			Assert.IsTrue (cf_graph [2].Expression is OperatorNode);
			Assert.IsTrue (cf_graph [3].Expression is VariableUseNode);
			Assert.IsTrue (cf_graph [4].Expression is OperatorNode);
			Assert.AreEqual (OperatorType.MINUS, ((OperatorNode)cf_graph [4].Expression).Operator);
			Assert.IsTrue (cf_graph [5].Expression is OperatorNode);
			Assert.AreEqual (OperatorType.MUL, ((OperatorNode)cf_graph [5].Expression).Operator);

			Assert.IsTrue (cf_graph [0] is ConditionalJumpVertex);

		}

		[Test]
		public void ControlFlowExtractor_SimpleFunctionCallWithoutBlockExpression_Test ()
		{
			//f(a, 2, [5, 3], 4, 5, b, a);
			BlockExpressionNode _block_expression = new BlockExpressionNode ();
			_block_expression.Elements = new List<ExpressionNode>{ Var ("5"), Var ("3") };
			FunctionCallNode _function_call = new FunctionCallNode ();
			_function_call.Arguments = new List<ExpressionNode> {
				Var ("a"),
				Var ("2"),
				_block_expression,
				Var ("4"),
				Var ("5"),
				Var ("b"),
				Var ("a")
			};

			List<Vertex> cf_graph = new List<Vertex> (new ControlFlowExtractor ().Extract (_function_call));

			Assert.AreEqual (3, cf_graph.Count);

			Assert.IsTrue (cf_graph [0].Expression is VariableUseNode);
			Assert.AreEqual ("5", ((VariableUseNode)cf_graph [0].Expression).Name);
			Assert.IsTrue (cf_graph [1].Expression is VariableUseNode);
			Assert.AreEqual ("3", ((VariableUseNode)cf_graph [1].Expression).Name);
			Assert.IsTrue (cf_graph [2].Expression is FunctionCallNode);
			Assert.IsTrue (((FunctionCallNode)cf_graph [2].Expression).Arguments [2] is VariableUseNode);
		}
        
	}
}
