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

        // New classes should override method Compare. The implementation of Compare
        // should call Compare in the base class.
        public sealed override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            return Compare(obj);
        }

        public sealed override int GetHashCode()
        {
            return 1;
        }

        // Compare may assume that this and rhs are of the same type.
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
        
        // New classes should just override PrintElements.
        //
        // If your ToString() string is displayed in one line, in VisualStudio, 
        // in ImmediateView, type node.ToString(),nq
        // to make newlines visible.
        public sealed override string ToString()
        {
            return Print("");
        }

        protected virtual string PrintElements(string prefix)
        {
            return "";
        }

        #region Printing helpers
        // Skips prefix for first line.
        protected string Print(string prefix)
        {
            return GetType().Name + "\n" + PrintElements(prefix + "| ");
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
        #endregion
    }
}

