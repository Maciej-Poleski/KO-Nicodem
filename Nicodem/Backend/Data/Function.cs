using System;
using System.Linq;
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

		public static readonly HardwareRegisterNode[] CallerSavedRegisters = 
		{
			Target.RAX,
			Target.RCX,
			Target.RDX,
			Target.R8,
			Target.R9,
			Target.R10,
			Target.R11
		};

		public static readonly HardwareRegisterNode[] CalleeSavedRegisters = { };

        private readonly Function _enclosedIn;
        // Function with not null _enclosedIn have additional variable on stack just below old RBP - pointer to nearest enclosing Function stack frame
        private int _stackFrameSize;
        // RBP - this stack frame
        // (optional) RBP-8 - address of stack frame of nearest enclosing Function (NOTE: space will be alocated in preamble)

        // Can be public if somebody wants (but this structure will change soon)
        private IReadOnlyList<bool> Parameters { get; set; }

        // ------------------- properties -------------------

        internal Function EnclosedIn
        {
            get { return _enclosedIn; }
        }

        /// <value>Label of this function.</value>
        public string Label { get; private set; }

        /// <value>Body of this function.</value>
        public IEnumerable<Node> Body { get; set; } // Currently implementation requires return value to be stored in Body.ResultRegister

        //private Location[] ArgsLocations; // TODO: use this!

        public LocationNode GetArgLocationNode(int i){
            throw new NotImplementedException();
        }

        /// <value>Locations representing arguments inside Body.</value>
        [Obsolete]
        public LocationNode[] ArgsLocations { get; private set; } // TODO: set it inside constructor

		public LocationNode[] CalleeSavedRegLocations { get; private set; } // TODO: create inside constructor (count = count of CalleeSavedRegisters)

        /// <value>Number of arguments of this function.</value>
        public int ArgsCount { get { return ArgsLocations.Length; } }

        /// <value>Node which value will be returned as this function result.</value>
        public Node Result { get; set; }    // very very bad idea - one temporary for all function calls?

        // ------------------- constructor -------------------

        /// <summary>
        /// </summary>
        /// <param name="parameterIsLocal">Bitmap of parameters which need to be Local</param>
        public Function(string label, IReadOnlyList<bool> parameters, Function enclosedInFunction = null)
        {
            Label = label;
            Parameters = parameters;
            _enclosedIn = enclosedInFunction;
        }

        // ------------------- internal methods -------------------

        internal LocationNode GetCurrentStackFrame()
        {
            return Target.RBP;
        }

        internal LocationNode GetEnclosingFunctionStackFrame()
        {
            return GetEnclosingFunctionStackFrame(GetCurrentStackFrame());
        }

        // ------------------- public methods -------------------

        public Local AllocLocal()
        {
            // Assume all locals are 8-byte wide
            // isn't true that variables on stack on AMD64 are 8-byte aligned?
            return new Local(this, _stackFrameSize += 8);
        }

        public LocationNode AccessLocal(Location local)
        {
            return local.AccessLocal(this, GetCurrentStackFrame());
        }

        public SequenceNode FunctionCall(Function from, Node[] args, out Action<Node> nextNodeSetter)
        {
            return FunctionCall(from, args, new TemporaryNode(), out nextNodeSetter);
        }

        public SequenceNode FunctionCall(Function from, Node[] args, RegisterNode result, out Action<Node> nextNodeSetter)
        {
            var pl = _enclosedIn == null ? 0 : 1; // space for enclosing function stack frame address
            var seq = new Node[args.Length + Math.Max(0, args.Length - HardwareRegistersOrder.Length) + 3 + pl];
            // params in registers, params on stack, call, result, cleanup stack
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
            seq[ptr++] = new AssignmentNode(result, Body.Last().ResultRegister);  // Assume value of function body is return value
            seq[ptr++] = new AssignmentNode(Target.RSP, new AddOperatorNode(Target.RSP, new ConstantNode<long>(_stackFrameSize)));
            return new SequenceNode(seq, out nextNodeSetter, result);
        }

        /// <summary>
        /// Generates the whole body of this function: prologue, body, epilogue.
        /// </summary>
        public IEnumerable<Node> GenerateTheWholeBody()
        {
            return GeneratePrologue().Concat(Body.Concat(GenerateEpilogue()));
        }

        // ------------------- private methods -------------------

        private LocationNode GetEnclosingFunctionStackFrame(Node stackFrame)
        {
            if (_enclosedIn == null)
            {
                throw new InvalidOperationException("There is no enclosing function");
            }
            return new MemoryNode(new SubOperatorNode(stackFrame, new ConstantNode<long>(8)));
            // Address of enclosing function stack frame is just below old RBP (current RBP-8)
        }

        // ------ computation of stack ------

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

        // ------ prologue, epilogue ------

        private IEnumerable<Node> GeneratePrologue()
        {
			var prologue = new List<Node> ();
			// add label
			prologue.Add(new LabelNode(Label));
			// push rbp
			var pushRbp = Push (Target.RBP);
			prologue.Add (pushRbp.Item1);
			prologue.Add (pushRbp.Item2);
			// mov rbp, rsp
			prologue.Add (new AssignmentNode(Target.RBP, Target.RSP));
			// sub rsp, _stackFrameSize
			prologue.Add (new SubOperatorNode(Target.RSP, new ConstantNode<long>(_stackFrameSize)));

            // move arguments from hardware registers to LocationNodes in ArgsLocations
			for (int i = 0; i < ArgsCount; i++) {
                prologue.Add (new AssignmentNode (GetArgLocationNode(i), HardwareRegistersOrder [i]));
			}

			// save callee-saved registers
			for (int i = 0; i < CalleeSavedRegisters.Length; i++) {
				prologue.Add (new AssignmentNode (CalleeSavedRegLocations [i], CalleeSavedRegisters [i]));
			}

			return prologue;
        }

        private IEnumerable<Node> GenerateEpilogue()
        {
			var epilogue = new List<Node> ();
            // function result in Result -> mov it to RAX
			epilogue.Add (new AssignmentNode (Target.RAX, Result));
			// restore callee-saved registers
			for (int i = 0; i < CalleeSavedRegisters.Length; i++) {
				epilogue.Add (new AssignmentNode (CalleeSavedRegisters [i], CalleeSavedRegLocations [i]));
			}
			// mov rsp, rbp
			epilogue.Add(new AssignmentNode (Target.RSP, Target.RBP));
			// pop rbp
			epilogue.Add(new AssignmentNode (Target.RBP, new MemoryNode(Target.RSP)));
			epilogue.Add (new AssignmentNode (Target.RSP, new AddOperatorNode (Target.RSP, new ConstantNode<long> (8))));
			// ret
			epilogue.Add(new RetNode());
			return epilogue;
        }
    }
}