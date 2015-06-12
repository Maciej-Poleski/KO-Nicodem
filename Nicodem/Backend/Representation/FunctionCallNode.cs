using System.Collections.Generic;

namespace Nicodem.Backend.Representation
{
    public class FunctionCallNode : Node
    {
        public FunctionCallNode(Function function)
        {
            Function = function;
        }

        // Function type depends on architecture. Adjust when Target will be in shape.
        public Function Function { get; private set; }

        #region Printing
        protected override string Print()
        {
            return "call " + Function.Label;
        }
        #endregion
    }
}