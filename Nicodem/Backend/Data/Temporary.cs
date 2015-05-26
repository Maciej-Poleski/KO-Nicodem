using System;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    public class Temporary : Location
    {
        private readonly RegisterNode _node;

        public Temporary()
        {
            _node = new TemporaryNode();
        }

        /// <summary>
        ///     Use this constructor to wrap existing instance of TemporaryNode
        /// </summary>
        /// <param name="nodeToWrap"></param>
        public Temporary(RegisterNode nodeToWrap)
        {
            _node = nodeToWrap;
        }

        public RegisterNode Node
        {
            get { return _node; }
        }

        public override LocationNode AccessLocal(Function function, LocationNode stackFrame)
        {
            return Node;
        }
    }
}