using NUnit.Framework;
using Nicodem.Source;

namespace Nicodem.Source.Tests
{
	[TestFixture]
	public class FileOriginTests : IOriginAbstractTests
	{
		public override IOrigin CreateOriginTest(string source)
		{
			return new StringOrigin(source);
		}
	}
}

