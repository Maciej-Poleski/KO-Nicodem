using System;

namespace Nicodem.Backend.Representation
{
    public class ConditionalJumpNode : Node
    {
        public ConditionalJumpNode(Node condition, Node nextNodeIfTrue, Node nextNodeIfFalse)
        {
            Condition = condition;
            NextNodeIfTrue = nextNodeIfTrue;
            NextNodeIfFalse = nextNodeIfFalse;
        }

        /// <summary>
        ///     This constructor allows to postpone initialization of non-tree edges (targets of this conditional jump).
        /// </summary>
        public ConditionalJumpNode(Node condition, out Action<Node> nextNodeIfTrueSetter,
            out Action<Node> nextNodeIfFalseSetter)
        {
            Condition = condition;
            nextNodeIfTrueSetter = node =>
            {
                if (NextNodeIfTrue == null)
                {
                    NextNodeIfTrue = node;
                }
                else
                {
                    throw new InvalidOperationException("NextNodeIfTrue property is already initialized");
                }
            };
            nextNodeIfFalseSetter = node =>
            {
                if (NextNodeIfFalse == null)
                {
                    NextNodeIfFalse = node;
                }
                else
                {
                    throw new InvalidOperationException("NextNodeIfFalse property is already initialized");
                }
            };
        }

        // Conditional jump will depend on result of this computation
        public Node Condition { get; private set; }
        // These are NOT in-tree parent-child connections (these links make cycles)
        public Node NextNodeIfTrue { get; private set; }
        public Node NextNodeIfFalse { get; private set; }

		public override Node[] GetChildren() {
			return new Node[]{ Condition, NextNodeIfTrue, NextNodeIfFalse };
		}

        #region Printing
        protected override string PrintElements(string prefix)
        {
            return PrintChild(prefix, "condition", Condition) 
                + PrintChild(prefix, "true", NextNodeIfTrue, false)
                + PrintChild(prefix, "false", NextNodeIfFalse, false);
        }
        #endregion

        #region implemented ReplaceRegisterWithLocal
        internal override Node ReplaceRegisterWithLocal(
            System.Collections.Generic.IReadOnlyDictionary<RegisterNode, Local> map, 
            System.Collections.Generic.List<Node> newTrees, 
            Function f)
        {
            Condition = Condition.ReplaceRegisterWithLocal(map, newTrees, f);
            NextNodeIfTrue = NextNodeIfTrue.ReplaceRegisterWithLocal(map, newTrees, f);
            NextNodeIfFalse = NextNodeIfFalse.ReplaceRegisterWithLocal(map, newTrees, f);
            return base.ReplaceRegisterWithLocal(map, newTrees, f);
        }
        #endregion
    }
}