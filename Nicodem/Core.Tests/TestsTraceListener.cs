using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nicodem.Core
{
    public class TestsTraceListener : TraceListener
    {
        public static void Setup()
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new ConsoleTraceListener());
            Debug.Listeners.Add(new TestsTraceListener());
        }

        public override void Write(string message)
        {
            Assert.Fail();
        }

        public override void WriteLine(string message)
        {
            Assert.Fail();
        }
    }
}
