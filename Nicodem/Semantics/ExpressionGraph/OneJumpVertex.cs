﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.ExpressionGraph
{
    class OneJumpVertex : Vertex
    {
        public Vertex Jump { get; set; }
    }
}