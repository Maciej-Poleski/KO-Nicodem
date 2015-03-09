using System;
using System.Collections.Generic;
using System.Text;

namespace Nicodem.Source
{
    public interface IMyOriginReader<TMemento>
    {
        MyLocation CurrentLocation { get; }
        char CurrentCharacter { get; }
        TMemento MakeMemento();
        void Rollback(TMemento memento);
        bool MoveNext();
    }

    public interface IMyOrigin
    {
        MyLocation Begin { get; }
        //IMyOriginReader<TMemento> GetReader(); // remove dependence on TMemento
        //IMyFragment MakeFragment(IMyLocation from, IMyLocation to); // use fragment constructor
    }
    
    // -------------

    public struct SourcePosition
    {
        public int line, charPos;
        public SourcePosition(int line, int charPos)
        {
            this.line = line;
            this.charPos = charPos;
        }
    }

    public class MyLocation
    {
        public SourcePosition Position { get; private set; }
        public IMyOrigin Origin { get; private set; }

        public MyLocation(SourcePosition pos, IMyOrigin origin)
        {
            this.Position = pos;
            this.Origin = origin;
        }
    }

    public class MyFragment
    {
        public SourcePosition From { get; private set; }
        public SourcePosition To { get; private set; }
        public IMyOrigin Origin { get; private set; }

        public MyFragment(SourcePosition from, SourcePosition to, IMyOrigin origin)
        {
            this.From = from;
            this.To = to;
            this.Origin = origin;
        }
    }

    // -------------
}
