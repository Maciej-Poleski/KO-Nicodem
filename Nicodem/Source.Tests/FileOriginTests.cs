using NUnit.Framework;
using Nicodem.Source;
using System.IO;

namespace Nicodem.Source.Tests
{
    [TestFixture]
    public class FileOriginTests : IOriginAbstractTests
    {
        private string tmpTestFilePath;

        [TestFixtureSetUp]
        public void Init()
        {
            System.Console.WriteLine("===== FILE ORIGIN TestFixture =====");
            System.Console.WriteLine("-> CREATE temporary file...");
            tmpTestFilePath = Path.GetTempFileName();
            System.Console.WriteLine("path: " + tmpTestFilePath);
            System.Console.WriteLine("DONE.");
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            System.Console.WriteLine("-> DELETE temporary file: " + tmpTestFilePath + "...");
            File.Delete(tmpTestFilePath);
            System.Console.WriteLine("DONE");
            System.Console.WriteLine("===================================");
        }

        public override IOrigin CreateOriginTest(string source)
        {
            System.Console.WriteLine("-> WRITING source to temporary file: \"" + source + "\"");
            File.WriteAllText(tmpTestFilePath, source);
            System.Console.WriteLine("DONE");
            return new FileOrigin(tmpTestFilePath);
        }
    }
}
