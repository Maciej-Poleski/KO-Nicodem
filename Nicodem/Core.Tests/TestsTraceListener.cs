using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Core
{
    public class TestsTraceListener : TraceListener
    {
        public delegate void FailTest();

        private FailTest failTest;

        private TestsTraceListener(FailTest failTest)
        {
            this.failTest = failTest;
        }

        public static void Setup(FailTest failTest)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new ConsoleTraceListener());
            Debug.Listeners.Add(new TestsTraceListener(failTest));
        }

        public override void Write(string message)
        {
            failTest();
        }

        public override void WriteLine(string message)
        {
            failTest();
        }
    }
}
