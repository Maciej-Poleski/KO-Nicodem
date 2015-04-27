using System.Text;
using Nicodem.Backend.Representation;
using NUnit.Framework;

namespace Nicodem.Backend.Tests.Representation
{
    public class VisitorOnNodeIsOperationalTests
    {
        [Test]
        public void ParseSimpleOperators()
        {
            // 2+2*2
            var simpleExpression = new AddOperatorNode(new ConstantNode<int>(2),
                new MulOperatorNode(new ConstantNode<int>(2), new ConstantNode<int>(2)));

            var printer = new BinaryOperatorPrinter();
            simpleExpression.Accept(printer);
            Assert.AreEqual("(2+(2*2))", printer.Result);

            var counter = new NodeCounter();
            simpleExpression.Accept(counter);
            Assert.AreEqual(5, counter.Count);
        }

        private class BinaryOperatorPrinter : AbstractRecursiveVisitor
        {
            private readonly StringBuilder _result = new StringBuilder();

            public string Result
            {
                get { return _result.ToString(); }
            }

            public override void Visit(BinaryOperatorNode node)
            {
                _result.Append('(');
                node.LeftOperand.Accept(this);
                _result.Append(ToString(node as dynamic));
                node.RightOperand.Accept(this);
                _result.Append(')');
            }

            public override void Visit<TConstant>(ConstantNode<TConstant> node)
            {
                _result.Append(node.Value);
            }

            private static string ToString(AddOperatorNode ignored)
            {
                return "+";
            }

            private static string ToString(MulOperatorNode ignored)
            {
                return "*";
            }

            private static string ToString(object ignored)
            {
                return "unknown";
            }
        }

        private class NodeCounter : AbstractRecursiveVisitor
        {
            public int Count { get; private set; }

            public override void Visit(Node node)
            {
                ++Count;
            }
        }
    }
}