using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Nicodem.Source
{
    public class MyFileOrigin : IMyOrigin
    {
        private readonly MyLocation begin;
        internal String SourcePath { get; private set; }

        MyFileOrigin(String filePath)
        {
            this.begin = new MyLocation(new SourcePosition(0, 0), this);
            this.SourcePath = filePath;
        }

        #region IMyOrigin Members

        public MyLocation Begin
        {
            get { return begin; }
        }

        #endregion
    }

    public class MyFileOriginReader : IMyOriginReader<MyFileOriginReader.FileMemento>
    {
        public struct FileMemento
        {
            internal readonly SourcePosition curPos;
            internal readonly long streamPosition;
            internal FileMemento(SourcePosition sourcePos, long streamPosition)
            {
                this.curPos = sourcePos;
                this.streamPosition = streamPosition;
            }
        }

        private MyFileOrigin origin;
        private StreamReader reader;
        private SourcePosition curPos;

        public MyFileOriginReader(MyFileOrigin fileOrigin)
        {
            this.origin = fileOrigin;
            curPos = origin.Begin.Position;
            reader = new StreamReader(new FileStream(origin.SourcePath, FileMode.Open), System.Text.Encoding.UTF8);
        }

        #region IMyOriginReader<FileMemento> Members

        public MyLocation CurrentLocation
        {
            get { return new MyLocation(curPos, origin); }
        }

        public char CurrentCharacter
        {
            get { return (char) reader.Peek(); }
        }

        public FileMemento MakeMemento()
        {
            SourcePosition copy = curPos;
            return new FileMemento(copy, reader.BaseStream.Position);
        }

        public void Rollback(FileMemento memento)
        {
            curPos = memento.curPos;
            reader.BaseStream.Position = memento.streamPosition;
        }

        public bool MoveNext()
        {
            if (reader.EndOfStream)
            {
                return false;
            }
            if (CurrentCharacter == '\n')
            {
                ++curPos.line;
                curPos.charPos = 0;
            }
            else
            {
                ++curPos.charPos;
            }
            reader.Read();
            return true;
        }

        #endregion
    }
}
