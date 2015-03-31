using NUnit.Framework;
using Nicodem.Lexer;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExDerivativeTests
	{
		[Test]
		public void RegExStarSimpleDerivativeTest()
		{
			var aStar = RegExFactory.Star<char>(RegExFactory.Intersection<char>(RegExFactory.Range<char>('a'), RegExFactory.Complement<char>(RegExFactory.Range<char>('b'))));
			var bStar = RegExFactory.Star<char>(RegExFactory.Intersection<char>(RegExFactory.Range<char>('b'), RegExFactory.Complement<char>(RegExFactory.Range<char>('c'))));
			var cStar = RegExFactory.Star<char>(RegExFactory.Intersection<char>(RegExFactory.Range<char>('c'), RegExFactory.Complement<char>(RegExFactory.Range<char>('d'))));
			var dStar = RegExFactory.Star<char>(RegExFactory.Intersection<char>(RegExFactory.Range<char>('d'), RegExFactory.Complement<char>(RegExFactory.Range<char>('e'))));
			var aStarbStarcStar = RegExFactory.Concat<char>(aStar, bStar, cStar, dStar);
			Assert.AreEqual(dStar, aStarbStarcStar.Derivative('c'));
		}
	}
}

