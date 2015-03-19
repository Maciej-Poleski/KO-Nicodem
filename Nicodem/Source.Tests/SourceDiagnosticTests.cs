using NUnit.Framework;
using Nicodem.Source;

namespace Nicodem.Source.Tests
{
    [TestFixture]
    class SourceDiagnosticTests
    {
        [Test]
        public void PrintMessageTest()
        {
            string source = "OK. Error <- here is an error!";
            IOriginReader reader = new StringOrigin(source).GetReader();
            for (int i = 0; i < 4; i++) {
                reader.MoveNext(); // 'OK. '
            }
            ILocation locBeg = reader.CurrentLocation;
            for (int i = 0; i < 5; i++) {
                reader.MoveNext(); // 'Error'
            }
            ILocation locEnd = reader.CurrentLocation;

            IFragment fr = locBeg.Origin.MakeFragment(locBeg, locEnd);

            SourceDiagnostic sd = new SourceDiagnostic();
            string res = sd.PrepareMessage("warning", fr, "found an error in your source");
            sd.PrintMessage("warning", fr, "found an error in your source");
            Assert.AreEqual(res, "warning at line 1:4 \"Error\" - found an error in your source");
        }
    }
}
