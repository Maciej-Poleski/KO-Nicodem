using NUnit.Framework;
using Nicodem.Source.Tmp;

namespace Nicodem.Source.Tests
{
    [TestFixture]
	public class StringOriginTests : IOriginAbstractTests
    {
        public override IOrigin CreateOriginTest(string source)
        {
            return new StringOrigin(source);
        }
    }
}
