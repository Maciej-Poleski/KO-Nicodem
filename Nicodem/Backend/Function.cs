using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    public class Function
    {
        private readonly Function _nestedIn;
        private readonly Location[] _parameters;
        private readonly Temporary _result;
        internal readonly Temporary FramePointer = new Temporary();
        private int _stackFrameSize;

        /// <summary>
        /// </summary>
        /// <param name="parameterIsLocal">Bitmap of parameters which need to be Local</param>
        public Function(IReadOnlyCollection<bool> parameterIsLocal, Function nestedInFunction = null)
        {
            _parameters = new Location[parameterIsLocal.Count];
            var i = 0;
            foreach (var isLocal in parameterIsLocal)
            {
                _parameters[i++] = isLocal ? (Location) AllocLocal() : new Temporary();
            }
            _result = new Temporary();
            _nestedIn = nestedInFunction;
        }

        /// <summary>
        ///     Use to get result Temporary for use in function implementation
        /// </summary>
        public Temporary Result
        {
            get { return _result; }
        }

        public Local AllocLocal()
        {
            // Assume all local are 8-byte wide
            // isn't true that variables on stack on AMD64 are 8-byte aligned?
            return new Local(this, _stackFrameSize += 8);
        }

        public LocationNode AccessLocal(Location local)
        {
            return local.AccessLocal(this);
        }

        public SequenceNode FunctionCall(Temporary[] args, Temporary result, out Action<Node> nextNodeSetter)
        {
            var sequence = new Node[args.Length + 2];
            var i = 0;
            foreach (var arg in args)
            {
                sequence[i++] = new AssignmentNode(_parameters[i].AccessLocal(this), arg.Node);
            }
            sequence[i++] = new FunctionCallNode(this);
            sequence[i++] = new AssignmentNode(result.Node, Result.Node);
            return new SequenceNode(sequence, out nextNodeSetter);
        }
    }
}