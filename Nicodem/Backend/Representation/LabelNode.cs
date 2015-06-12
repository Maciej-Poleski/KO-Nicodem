﻿using System;

namespace Nicodem.Backend.Representation
{
	public class LabelNode : Node
	{
		public string Label { get; private set; }

		public LabelNode (string label)
		{
			Label = label;
		}

        #region Printing
        protected override string Print()
        {
            return Label;
        }
        #endregion
	}
}
