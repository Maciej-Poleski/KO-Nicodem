using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    // This is convention is based on IA64 ABI
    public class Function
    {
        private static readonly HardwareRegisterNode[] HardwareRegistersOrder =
        {
            Target.RDI,
            Target.RSI,
            Target.RDX,
            Target.RCX,
            Target.R8,
            Target.R9
        };

        private readonly SequenceNode _body;
        private readonly Function _nestedIn;
        private int _stackFrameSize;

        /// <summary>
        /// </summary>
        /// <param name="parameterIsLocal">Bitmap of parameters which need to be Local</param>
        public Function(SequenceNode body, IReadOnlyCollection<bool> parameterIsLocal, Function nestedInFunction = null)
        {
            _body = body;
            _nestedIn = nestedInFunction;
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

        public SequenceNode FunctionCall(Node[] args, out Action<Node> nextNodeSetter)
        {
            var seq = new Node[args.Length + Math.Max(0, args.Length - HardwareRegistersOrder.Length) + 1];
            // params in registers, params on stack, call
            var i = 0;
            for (; i < HardwareRegistersOrder.Length && i < args.Length; ++i)
            {
                seq[i] = new AssignmentNode(HardwareRegistersOrder[i], args[i]);
            }
            var ptr = i;
            for (; i < args.Length; ++i)
            {
                var v = Push(args[i]);
                seq[ptr++] = v.Item1;
                seq[ptr++] = v.Item2;
            }
            seq[ptr++] = new FunctionCallNode(_body);
            return new SequenceNode(seq, out nextNodeSetter, Target.RAX);
        }

        private static Tuple<Node, Node> Push(Node value)
        {
            var push = new AssignmentNode(new MemoryNode(Target.RSP), value);
            var move = new AssignmentNode(Target.RSP, new SubOperatorNode(Target.RSP, new ConstantNode<long>(8)));
            return new Tuple<Node, Node>(push, move);
        }
    }
}