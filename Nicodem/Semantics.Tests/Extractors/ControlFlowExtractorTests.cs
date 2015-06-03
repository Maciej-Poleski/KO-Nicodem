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

        private OperatorNode MakeOperator(OperatorType _operator, params ExpressionNode[] _arguments){
            OperatorNode made_one = new OperatorNode();
            made_one.Arguments = new List<ExpressionNode>(_arguments);
            made_one.Operator = _operator;
            return made_one;
        }

        private VariableUseNode Var(String s)
        {
            VariableUseNode _var = new VariableUseNode();
            _var.Name = s;
            return _var;
        }

        [TestFixtureSetUp]
        public void InitAST()
        {
        }

        [Test]
        public void ControlFlowExtractor_SimpleIfInBlockExpression_Test()
        {
            //[
            //a+2; 
            //a+(if(a>3){a+2;}else{a+5;})+5;
            //]

            OperatorNode first_statement = MakeOperator(OperatorType.PLUS, Var("a"), Var("2"));
            OperatorNode _a_gret_3 = MakeOperator(OperatorType.GREATER, Var("a"),  Var("3"));
            OperatorNode _a_plus_2 = MakeOperator(OperatorType.PLUS,  Var("a"),  Var("2"));
            OperatorNode _a_plus_5 = MakeOperator(OperatorType.PLUS,  Var("a"),  Var("5"));
            IfNode _if = new IfNode();
            _if.Condition = _a_gret_3;
            _if.Then = _a_plus_2;
            _if.Else = _a_plus_5;
            OperatorNode second_statement = MakeOperator(OperatorType.PLUS,  Var("a"), MakeOperator(OperatorType.PLUS, _if,  Var("5")));
            BlockExpressionNode _if_block = new BlockExpressionNode();
            _if_block.Elements = new List<ExpressionNode> { first_statement, second_statement };

            List<Vertex> cf_graph = new List<Vertex>(new ControlFlowExtractor().Extract(_if_block));

            Assert.AreEqual(6, cf_graph.Count);

            Assert.IsTrue(cf_graph[0].Expression is OperatorNode);
            Assert.IsTrue(cf_graph[1].Expression is OperatorNode);
            Assert.IsTrue(cf_graph[2].Expression is OperatorNode);
            Assert.IsTrue(cf_graph[3].Expression is OperatorNode);
            Assert.IsTrue(cf_graph[4].Expression is VariableUseNode);
            Assert.IsTrue(cf_graph[5].Expression is OperatorNode);

            Assert.IsTrue(cf_graph[1] is ConditionalJumpVertex);
        }

        [Test]
        public void ControlFlowExtractor_SimpleWhileInBlockExrpession_Test()
        {
            //[
            //a-while(a<3){a+1}else{a+3}+5;
            //a*2;
            //]

            OperatorNode a_less_3 = MakeOperator(OperatorType.LESS,  Var("a"), Var("3"));
            OperatorNode a_plus_1 = MakeOperator(OperatorType.PLUS,  Var("a"), Var("1"));
            OperatorNode a_plus_5 = MakeOperator(OperatorType.PLUS,  Var("a"), Var("5"));
            WhileNode _while = new WhileNode();
            _while.Condition = a_less_3;
            _while.Body = a_plus_1;
            _while.Else = a_plus_5;
            OperatorNode _first_statement = MakeOperator(OperatorType.MINUS, Var("a"), MakeOperator(OperatorType.PLUS, _while, Var("5")));
            
            OperatorNode _second_statement = MakeOperator(OperatorType.MUL, Var("a"), Var("2"));

            BlockExpressionNode _while_block = new BlockExpressionNode();
            _while_block.Elements = new List<ExpressionNode>{_first_statement, _second_statement};

            List<Vertex> cf_graph = new List<Vertex>(new ControlFlowExtractor().Extract(_while_block));

            Assert.AreEqual(6, cf_graph.Count);

            Assert.IsTrue(cf_graph[0].Expression is OperatorNode);
            Assert.IsTrue(cf_graph[1].Expression is OperatorNode);
            Assert.IsTrue(cf_graph[2].Expression is OperatorNode);
            Assert.IsTrue(cf_graph[3].Expression is VariableUseNode);
            Assert.IsTrue(cf_graph[4].Expression is OperatorNode);
            Assert.AreEqual(OperatorType.MINUS, ((OperatorNode)cf_graph[4].Expression).Operator);
            Assert.IsTrue(cf_graph[5].Expression is OperatorNode);
            Assert.AreEqual(OperatorType.MUL, ((OperatorNode)cf_graph[5].Expression).Operator);

            Assert.IsTrue(cf_graph[0] is ConditionalJumpVertex);

        }

        [Test]
        public void ControlFlowExtractor_SimpleFunctionCallWithoutBlockExpression_Test()
        {
            //f(a, 2, [5, 3], 4, 5, b, a);
            BlockExpressionNode _block_expression = new BlockExpressionNode();
            _block_expression.Elements = new List<ExpressionNode>{Var("5"), Var("3")};
            FunctionCallNode _function_call = new FunctionCallNode();
            _function_call.Arguments = new List<ExpressionNode> { Var("a"), Var("2"), _block_expression, Var("4"), Var("5"), Var("b"), Var("a") };

            List<Vertex> cf_graph = new List<Vertex>(new ControlFlowExtractor().Extract(_function_call));

            Assert.AreEqual(3, cf_graph.Count);

            Assert.IsTrue(cf_graph[0].Expression is VariableUseNode);
            Assert.AreEqual("5", ((VariableUseNode)cf_graph[4].Expression).Name);
            Assert.IsTrue(cf_graph[1].Expression is VariableUseNode);
            Assert.AreEqual("3", ((VariableUseNode)cf_graph[4].Expression).Name);
            Assert.IsTrue(cf_graph[2].Expression is FunctionCallNode);
            Assert.IsTrue(((FunctionCallNode)cf_graph[2].Expression).Arguments[2] is VariableUseNode);
        }
        
    }
}
