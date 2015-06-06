using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Nicodem.Semantics.AST;

namespace Semantics.Tests.AST
{
    [TestFixture]
    class TypeNodeTests
    {
        [Test]
        public void TypeNode_NamedTypeNodeEquals_Test()
        {
            Assert.IsTrue(NamedTypeNode.BoolType().Equals(NamedTypeNode.BoolType()));
            Assert.IsTrue(NamedTypeNode.ByteType().Equals(NamedTypeNode.ByteType()));
            Assert.IsTrue(NamedTypeNode.CharType().Equals(NamedTypeNode.CharType()));
            Assert.IsTrue(NamedTypeNode.IntType().Equals(NamedTypeNode.IntType()));
            Assert.IsTrue(NamedTypeNode.VoidType().Equals(NamedTypeNode.VoidType()));
            Assert.IsFalse(NamedTypeNode.ByteType().Equals(NamedTypeNode.VoidType()));
            Assert.IsFalse(NamedTypeNode.BoolType().Equals(NamedTypeNode.VoidType()));
            Assert.IsFalse(NamedTypeNode.IntType().Equals(NamedTypeNode.VoidType()));
            Assert.IsFalse(NamedTypeNode.CharType().Equals(NamedTypeNode.VoidType()));
            Assert.IsFalse(NamedTypeNode.BoolType().Equals(NamedTypeNode.IntType()));
            Assert.IsFalse(NamedTypeNode.ByteType().Equals(NamedTypeNode.IntType()));
            Assert.IsFalse(NamedTypeNode.CharType().Equals(NamedTypeNode.IntType()));
            Assert.IsFalse(NamedTypeNode.BoolType().Equals(NamedTypeNode.CharType()));
            Assert.IsFalse(NamedTypeNode.ByteType().Equals(NamedTypeNode.CharType()));
            Assert.IsFalse(NamedTypeNode.BoolType().Equals(NamedTypeNode.ByteType()));
            Assert.IsFalse(NamedTypeNode.ByteType().Equals(NamedTypeNode.BoolType()));
        }

        [Test]
        public void TypeNode_NamedTypeNodeHash_Test()
        {
            var hashSet = new HashSet<TypeNode>();

            hashSet.Add(NamedTypeNode.BoolType());
            Assert.AreEqual(1, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.BoolType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.ByteType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.CharType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.VoidType()));
            hashSet.Add(NamedTypeNode.BoolType());
            Assert.AreEqual(1, hashSet.Count);

            hashSet.Add(NamedTypeNode.ByteType());
            Assert.AreEqual(2, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.ByteType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.CharType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.VoidType()));
            hashSet.Add(NamedTypeNode.ByteType());
            Assert.AreEqual(2, hashSet.Count);

            hashSet.Add(NamedTypeNode.CharType());
            Assert.AreEqual(3, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.ByteType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.CharType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.VoidType()));
            hashSet.Add(NamedTypeNode.CharType());
            Assert.AreEqual(3, hashSet.Count);

            hashSet.Add(NamedTypeNode.IntType());
            Assert.AreEqual(4, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.ByteType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.CharType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(NamedTypeNode.VoidType()));
            hashSet.Add(NamedTypeNode.IntType());
            Assert.AreEqual(4, hashSet.Count);


            hashSet.Add(NamedTypeNode.VoidType());
            Assert.AreEqual(5, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.ByteType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.CharType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.IntType()));
            Assert.IsTrue(hashSet.Contains(NamedTypeNode.VoidType()));
            hashSet.Add(NamedTypeNode.VoidType());
            Assert.AreEqual(5, hashSet.Count);
        }

        [Test]
        public void TypeNode_ArrayTypeNodeEquals_Test()
        {
            Assert.IsTrue(ArrayTypeNode.BoolType().Equals(ArrayTypeNode.BoolType()));
            Assert.IsTrue(ArrayTypeNode.ByteType().Equals(ArrayTypeNode.ByteType()));
            Assert.IsTrue(ArrayTypeNode.CharType().Equals(ArrayTypeNode.CharType()));
            Assert.IsTrue(ArrayTypeNode.IntType().Equals(ArrayTypeNode.IntType()));
            Assert.IsTrue(ArrayTypeNode.VoidType().Equals(ArrayTypeNode.VoidType()));
            Assert.IsFalse(ArrayTypeNode.ByteType().Equals(ArrayTypeNode.VoidType()));
            Assert.IsFalse(ArrayTypeNode.BoolType().Equals(ArrayTypeNode.VoidType()));
            Assert.IsFalse(ArrayTypeNode.IntType().Equals(ArrayTypeNode.VoidType()));
            Assert.IsFalse(ArrayTypeNode.CharType().Equals(ArrayTypeNode.VoidType()));
            Assert.IsFalse(ArrayTypeNode.BoolType().Equals(ArrayTypeNode.IntType()));
            Assert.IsFalse(ArrayTypeNode.ByteType().Equals(ArrayTypeNode.IntType()));
            Assert.IsFalse(ArrayTypeNode.CharType().Equals(ArrayTypeNode.IntType()));
            Assert.IsFalse(ArrayTypeNode.BoolType().Equals(ArrayTypeNode.CharType()));
            Assert.IsFalse(ArrayTypeNode.ByteType().Equals(ArrayTypeNode.CharType()));
            Assert.IsFalse(ArrayTypeNode.BoolType().Equals(ArrayTypeNode.ByteType()));
            Assert.IsFalse(ArrayTypeNode.ByteType().Equals(ArrayTypeNode.BoolType()));
        }

        [Test]
        public void TypeNode_ArrayTypeNodeHash_Test()
        {
            var hashSet = new HashSet<TypeNode>();

            hashSet.Add(ArrayTypeNode.BoolType());
            Assert.AreEqual(1, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.BoolType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.ByteType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.CharType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.VoidType()));
            hashSet.Add(ArrayTypeNode.BoolType());
            Assert.AreEqual(1, hashSet.Count);

            hashSet.Add(ArrayTypeNode.ByteType());
            Assert.AreEqual(2, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.ByteType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.CharType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.VoidType()));
            hashSet.Add(ArrayTypeNode.ByteType());
            Assert.AreEqual(2, hashSet.Count);

            hashSet.Add(ArrayTypeNode.CharType());
            Assert.AreEqual(3, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.ByteType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.CharType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.VoidType()));
            hashSet.Add(ArrayTypeNode.CharType());
            Assert.AreEqual(3, hashSet.Count);

            hashSet.Add(ArrayTypeNode.IntType());
            Assert.AreEqual(4, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.ByteType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.CharType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.IntType()));
            Assert.IsFalse(hashSet.Contains(ArrayTypeNode.VoidType()));
            hashSet.Add(ArrayTypeNode.IntType());
            Assert.AreEqual(4, hashSet.Count);


            hashSet.Add(ArrayTypeNode.VoidType());
            Assert.AreEqual(5, hashSet.Count);
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.BoolType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.ByteType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.CharType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.IntType()));
            Assert.IsTrue(hashSet.Contains(ArrayTypeNode.VoidType()));
            hashSet.Add(ArrayTypeNode.VoidType());
            Assert.AreEqual(5, hashSet.Count);
        }
    }
}
