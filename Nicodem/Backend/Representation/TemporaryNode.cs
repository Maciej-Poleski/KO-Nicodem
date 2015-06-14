using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public class TemporaryNode : RegisterNode
    {
        static int nextId=0;
        private int _id;

        public TemporaryNode(){
            _id = nextId++;
        }

        public override string Id { get {return "t_" + _id;} }
    }
}
