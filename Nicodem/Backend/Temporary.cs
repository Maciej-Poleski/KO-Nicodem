using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    public class Temporary : Location
    {
        private readonly TemporaryNode _node;

        public Temporary()
        {
            _node = new TemporaryNode();
        }

        /// <summary>
        ///     Use this constructor to wrap existing instance of TemporaryNode
        /// </summary>
        /// <param name="nodeToWrap"></param>
        public Temporary(TemporaryNode nodeToWrap)
        {
            _node = nodeToWrap;
        }

        public TemporaryNode Node
        {
            get { return _node; }
        }

        public override LocationNode AccessLocal(Function function)
        {
            return Node;
        }
    }
}