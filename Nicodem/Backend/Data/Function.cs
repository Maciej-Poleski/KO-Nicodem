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

        private readonly Function _enclosedIn;
        // Function with not null _enclosedIn have additional variable on stack just below old RBP - pointer to nearest enclosing Function stack frame
        private int _stackFrameSize;
        // RBP - this stack frame
        // (optional) RBP-8 - address of stack frame of nearest enclosing Function (NOTE: space will be alocated in preamble)

        /// <summary>
        /// </summary>
        /// <param name="parameterIsLocal">Bitmap of parameters which need to be Local</param>
        public Function(IReadOnlyList<bool> parameters, Function enclosedInFunction = null)
        {
            Parameters = parameters;
            _enclosedIn = enclosedInFunction;
        }

        // Can be public if somebody wants (but this structure will change soon)
        private IReadOnlyList<bool> Parameters { get; set; }

        internal Function EnclosedIn
        {
            get { return _enclosedIn; }
        }

        internal LocationNode GetCurrentStackFrame()
        {
            return Target.RBP;
        }

        private LocationNode GetEnclosingFunctionStackFrame(Node stackFrame)
        {
            if (_enclosedIn == null)
            {
                throw new InvalidOperationException("There is no enclosing function");
            }
            return new MemoryNode(new SubOperatorNode(stackFrame, new ConstantNode<long>(8)));
            // Address of enclosing function stack frame is just below old RBP (current RBP-8)
        }

        internal LocationNode GetEnclosingFunctionStackFrame()
        {
            return GetEnclosingFunctionStackFrame(GetCurrentStackFrame());
        }

        public Local AllocLocal()
        {
            // Assume all local are 8-byte wide
            // isn't true that variables on stack on AMD64 are 8-byte aligned?
            return new Local(this, _stackFrameSize += 8);
        }

        public LocationNode AccessLocal(Location local)
        {
            return local.AccessLocal(this, GetCurrentStackFrame());
        }

        public SequenceNode FunctionCall(Function from, Node[] args, out Action<Node> nextNodeSetter)
        {
            var pl = _enclosedIn == null ? 0 : 1; // space for enclosing function stack frame address
            var seq = new Node[args.Length + Math.Max(0, args.Length - HardwareRegistersOrder.Length) + 1 + pl];
            // params in registers, params on stack, call
            if (pl == 1)
            {
                seq[0] = new AssignmentNode(HardwareRegistersOrder[0], from.ComputationOfStackFrameAddress(this));
            }
            var i = 0;
            for (; i < HardwareRegistersOrder.Length && i < args.Length; ++i)
            {
                seq[i + pl] = new AssignmentNode(HardwareRegistersOrder[i], args[i]);
            }
            var ptr = i + pl;
            for (; i < args.Length; ++i)
            {
                var v = Push(args[i]);
                seq[ptr++] = v.Item1;
                seq[ptr++] = v.Item2;
            }
            seq[ptr++] = new FunctionCallNode(this);
            return new SequenceNode(seq, out nextNodeSetter, Target.RAX);
        }

        private Node ComputationOfStackFrameAddress(Function target)
        {
            if (target.EnclosedIn == this)
            {
                return GetCurrentStackFrame();
            }
            return ComputationOfStackFrameAddress(GetCurrentStackFrame(), target);
        }

        private Node ComputationOfStackFrameAddress(Node stackFrame, Function target)
        {
            if (target == this)
            {
                return GetEnclosingFunctionStackFrame(stackFrame);
            }
            return _enclosedIn.ComputationOfStackFrameAddress(GetEnclosingFunctionStackFrame(stackFrame), target);
        }

        private static Tuple<Node, Node> Push(Node value)
        {
            var push = new AssignmentNode(new MemoryNode(Target.RSP), value);
            var move = new AssignmentNode(Target.RSP, new SubOperatorNode(Target.RSP, new ConstantNode<long>(8)));
            return new Tuple<Node, Node>(push, move);
        }
    }
}