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

        BlockExpressionNode block_expression_operator_and_if;

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
            OperatorNode _a_plus_5 = MakeOperator(OperatorType.PLUS, _a, _5);
            IfNode _if = new IfNode();
            _if.Condition = _a_gret_3;
            _if.Then = _a_plus_2;
            _if.Else = _a_plus_5;
            OperatorNode second_statement = MakeOperator(OperatorType.PLUS, _a, MakeOperator(OperatorType.PLUS, _if, _5));
            BlockExpressionNode _if_block = new BlockExpressionNode();
            _if_block.Elements = new List<ExpressionNode> { first_statement, second_statement };
            block_expression_operator_and_if = _if_block;
        }

        [Test]
        public void BasicIfTest()
        {
            IEnumerable<Vertex> cf_graph = new ControlFlowExtractor().Extract(block_expression_operator_and_if);
            Assert.AreEqual(5, cf_graph.Count());
        }
        
    }
}
