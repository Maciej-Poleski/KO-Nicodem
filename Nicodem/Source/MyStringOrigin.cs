using System;
using System.Collections.Generic;
using System.Text;

namespace Nicodem.Source
{
    public class MyStringOrigin : IMyOrigin
    {
        private readonly MyLocation begin;
        internal String Source { get; private set; }

        MyStringOrigin(String source)
        {
            this.begin = new MyLocation(new SourcePosition(0, 0), this);
            this.Source = source;
        }

        #region IMyOrigin Members

        public MyLocation Begin
        {
            get { return begin; }
        }

        #endregion
    }

    public class MyStringOriginReader : IMyOriginReader<MyStringOriginReader.StringMemento>
    {
        public struct StringMemento
        {
            internal readonly SourcePosition curPos;
            internal readonly int positionInSource;
            internal StringMemento(SourcePosition sourcePos, int posInSource)
            {
                this.curPos = sourcePos;
                this.positionInSource = posInSource;
            }
        }

        private MyStringOrigin origin;
        private SourcePosition curPos;
        private int positionInSource;

        public MyStringOriginReader(MyStringOrigin stringOrigin)
        {
            this.origin = stringOrigin;
            curPos = origin.Begin.Position;
            positionInSource = 0;
        }

        #region IMyOriginReader<StringMemento> Members

        public MyLocation CurrentLocation
        {
            get { return new MyLocation(curPos, origin); }
        }

        public char CurrentCharacter
        {
            get { return origin.Source[positionInSource]; }
        }

        public StringMemento MakeMemento()
        {
            SourcePosition copy = curPos;
            return new StringMemento(copy, positionInSource);
        }

        public void Rollback(StringMemento memento)
        {
            curPos = memento.curPos;
            positionInSource = memento.positionInSource;
        }

        public bool MoveNext()
        {
            if (positionInSource + 1 < origin.Source.Length)
            {
                if (CurrentCharacter == '\n')
                {
                    ++curPos.line;
                    curPos.charPos = 0;
                }
                else
                {
                    ++curPos.charPos;
                }
                ++positionInSource;
                return true;
            }
            return false;
        }

        #endregion
    }
}
