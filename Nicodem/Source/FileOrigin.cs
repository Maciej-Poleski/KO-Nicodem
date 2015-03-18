using System;
using System.IO;

namespace Nicodem.Source
{
    public class FileOrigin : AbstractOrigin
    {
        internal readonly String sourcePath;
 
        public FileOrigin(String sourcePath) : base(File.ReadAllLines(sourcePath,System.Text.Encoding.UTF8))
        {
            this.sourcePath = sourcePath;
        }

        public override string ToString()
        {
            return "FileOrigin of: " + sourcePath;
        }
    }
}
