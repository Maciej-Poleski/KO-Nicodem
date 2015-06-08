using Nicodem.Source;
using Nicodem.Parser;
using Nicodem.Semantics.Visitors;
using Nicodem.Semantics.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Semantics.AST
{
    abstract class Node
    {
        public abstract void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol : ISymbol<TSymbol>;

        public virtual void Accept(AbstractVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            if (GetType() != obj.GetType()) return false;
            return Compare(obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        /* Compare assumes that this and rhs are of the same type. It then checks equality of fields
         * defined in current class and calls Compare of the base class.
         */
        protected virtual bool Compare(object rhs)
        {
            return true;
        }

        // Helper function used by Compare.
        protected static bool SequenceEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first != null && second != null) return first.SequenceEqual(second);
            else return first == null && second == null;
        }
        
        // In VisualStudio, in ImmediateView, type:
        // node.ToString(),nq
        // to print new lines.
        public override string ToString()
        {
            return Print("");
        }

        // Skips prefix for first line.
        public string Print(string prefix)
        {
            return GetType().Name + "\n" + PrintElements(prefix + "| ");
        }

        protected virtual string PrintElements(string prefix)
        {
            return "";
        }

        protected static string PrintVar(string prefix, string label, Node obj)
        {
            if (obj == null) return prefix + label + ": null\n";
            else return prefix + label + ": " + obj.Print(prefix);
        }

        protected static string PrintVar(string prefix, string label, string obj)
        {
            if (obj == null) return prefix + label + ": null\n";
            else return prefix + label + ": " + obj.ToString() + "\n";
        }

        protected static string PrintVar(string prefix, string label, object obj)
        {
            if (obj == null) return prefix + label + ": null\n";
            else return prefix + label + ": " + obj.ToString() + "\n";
        }

        protected static string PrintVar<T>(string prefix1, string label, IEnumerable<T> field)
            where T : Node
        {
            string result = prefix1 + label + " (n=" + field.Count() + "):\n";
            string prefix2 = prefix1 + "| ";
            string prefix3 = prefix2 + "| ";
            foreach (var i in field) result += prefix2 + i.Print(prefix3);
            return result;
        }
    }
}

