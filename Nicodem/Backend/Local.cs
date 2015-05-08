using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    public class Local : Location
    {
        private readonly int _offsetFromStackFrame;
        private readonly Function _owningFunction;

        public Local(Function owningFunction, int offset)
        {
            _owningFunction = owningFunction;
            _offsetFromStackFrame = offset;
        }

        public override LocationNode AccessLocal(Function function)
        {
            return new MemoryNode(new AddOperatorNode(_owningFunction.FramePointer.Node,
                new ConstantNode<long>(_offsetFromStackFrame)));
        }
    }
}