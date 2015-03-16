using System;
using System.IO;

namespace Nicodem.Source
{
    public class FileOriginReader : IOriginReader
    {
        private FileOrigin origin;
        private StreamReader reader;
        private bool beforeBegin; // flag, if reader is positioned before first first character

        public FileOriginReader(FileOrigin fileOrigin)
        {
            this.origin = fileOrigin;
            this.reader = new StreamReader(new FileStream(origin.sourcePath, FileMode.Open), System.Text.Encoding.UTF8);
            this.beforeBegin = true;
        }

        // -------------- IOriginReader methods --------------
        public bool MoveNext ()
        {
            if (beforeBegin)
            {
                beforeBegin = false;
            }
            else
            {
                reader.Read(); // pass one character
            }
            return reader.Peek() != -1;
        }
        public ILocation CurrentLocation {
            get {
                return beforeBegin ? FileLocation.BeginLocation(origin) : new FileLocation(origin, reader.BaseStream.Position);
            }
            set {
                if (value.Origin == origin)
                {
                    FileLocation val = (FileLocation) value;
                    if (val.IsOriginBegin())
                    {
                        System.Console.WriteLine("is begin ----> " + reader.Peek());
                        reader.BaseStream.Position = 0;
                        reader.DiscardBufferedData();
                        System.Console.WriteLine("set to [0] --> " + reader.Peek());
                        beforeBegin = true;
                    }
                    else
                    {
                        System.Console.WriteLine("not begin ---> " + reader.Peek());
                        reader.BaseStream.Position = val.streamPos;
                        reader.DiscardBufferedData();
                        System.Console.WriteLine("set to: [" + val.streamPos + "] --> " + reader.Peek());
                        beforeBegin = false;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid origin");
                }
            }
        }
        public char CurrentCharacter {
            get {
                return (char) reader.Peek();
            }
        }

        // -------------- IDisposable Dispose --------------
        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
