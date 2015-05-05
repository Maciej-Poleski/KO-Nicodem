using Nicodem.Backend.Representation;
using System.Collections.Generic;

namespace Nicodem.Backend.Cover
{
	public static class InstructionFactory
	{
		static IEnumerable<RegisterNode> use(params RegisterNode[] nodes ){
			return nodes;
		}

		static IEnumerable<RegisterNode> define(params RegisterNode[] nodes ){
			return nodes;
		}

		#region mov

		public static Instruction Move( RegisterNode dst, RegisterNode src ) {
			return new Instruction (
				map => string.Format ("mov {0}, {1}", map [dst], map [src]),
				use (src, dst), define (dst), true);
		}

		public static Instruction Move<T>( RegisterNode dst, ConstantNode<T> c ) {
			return new Instruction (
				map => string.Format ("mov {0}, {1}", map [dst], c.Value),
				use (dst), define (dst));
		}

		#endregion

		#region add, sub, mul, xor, and, or

		static Instruction binopInstruction( string mnemonik, RegisterNode dst, RegisterNode src ) {
			return new Instruction (
				map => string.Format ("{0} {1}, {2}", mnemonik, map [dst], map [src]),
				use (dst, src), define (dst));
		}

		public static Instruction Add( RegisterNode dst, RegisterNode src ) {
			return binopInstruction ("add", dst, src);
		}

		public static Instruction Sub( RegisterNode dst, RegisterNode src ) {
			return binopInstruction ("sub", dst, src);
		}

		public static Instruction Xor( RegisterNode dst, RegisterNode src ) {
			return binopInstruction ("xor", dst, src);
		}

		public static Instruction Or( RegisterNode dst, RegisterNode src ) {
			return binopInstruction ("or", dst, src);
		}

		static Instruction binopInstruction<T>( string mnemonik, RegisterNode dst, ConstantNode<T> c ) {
			return new Instruction (
				map => string.Format ("{0} {1}, {2}", mnemonik, map [dst], c.Value),
				use (dst), define (dst));
		}

		public static Instruction Add<T>( RegisterNode dst, ConstantNode<T> c ) {
			return binopInstruction ("add", dst, c);
		}

		public static Instruction Sub<T>( RegisterNode dst, ConstantNode<T> c ) {
			return binopInstruction ("sub", dst, c);
		}

		public static Instruction Xor<T>( RegisterNode dst, ConstantNode<T> c ) {
			return binopInstruction ("xor", dst, c);
		}

		public static Instruction Or<T>( RegisterNode dst, ConstantNode<T> c ) {
			return binopInstruction ("or", dst, c);
		}

		#endregion

		#region and, test

		public static Instruction And( RegisterNode dst, RegisterNode src ) {
			return binopInstruction ("and", dst, src);
		}

		public static Instruction And<T>( RegisterNode dst, ConstantNode<T> c ) {
			return binopInstruction ("and", dst, c);
		}

		public static Instruction Test( RegisterNode left, RegisterNode right ) {
			return new Instruction (
				map => string.Format ("test {0}, {1}", map [left], map [right]),
				use (left, right), define ());
		}

		public static Instruction Test<T>( RegisterNode left, ConstantNode<T> c ) {
			return new Instruction (
				map => string.Format ("test {0}, {1}", map [left], c.Value),
				use (left), define ());
		}

		#endregion

		#region inc, dec, neg, not

		static Instruction unopInstruction( string mnemonik, RegisterNode reg ) {
			return new Instruction (
				map => string.Format ("{0} {1}", mnemonik, map [reg]),
				use (reg), define (reg));
		}

		public static Instruction Inc( RegisterNode reg ) {
			return unopInstruction ("inc", reg);
		}

		public static Instruction Dec( RegisterNode reg ) {
			return unopInstruction ("dec", reg);
		}

		public static Instruction Neg( RegisterNode reg ) {
			return unopInstruction ("neg", reg);
		}

		public static Instruction Not( RegisterNode reg ) {
			return unopInstruction ("not", reg);
		}

		#endregion

		#region call, ret

		public static Instruction Call( string fname ) {
			return new Instruction (
				map => string.Format ("call {0}", fname),
				use (), define ());
		}

		public static Instruction Ret() {
			return new Instruction (map => "ret", use (), define ());
		}

		#endregion

		#region pop, push

		public static Instruction Pop( RegisterNode reg ) {
			return new Instruction (
				map => string.Format ("pop {0}", map [reg]),
				use (reg), define (reg));
		}

		public static Instruction Push( RegisterNode reg ) {
			return new Instruction (
				map => string.Format ("push {0}", map [reg]),
				use (reg), define ());
		}

		#endregion

		#region cmp

		public static Instruction Cmp( RegisterNode left, RegisterNode right ) {
			return new Instruction (
				map => string.Format ("cmp {0}, {1}", map [left], map [right]),
				use (left, right), define ());
		}

		public static Instruction Cmp<T>( RegisterNode left, ConstantNode<T> c ) {
			return new Instruction (
				map => string.Format ("cmp {0}, {1}", map [left], c.Value),
				use (left), define ());
		}

		#endregion

		#region jump

		public static Instruction Jump( string cond_type, string label ) {
			return new Instruction (
				map => string.Format ("j{0} {1}", cond_type, label),
				use (), define ());
		}

		public static Instruction Jmp( string label ) {
			return Jump ("mp", label);
		}

		public static Instruction Jle( string label ) {
			return Jump ("le", label);
		}

		public static Instruction Jl( string label ) {
			return Jump ("l", label);
		}

		public static Instruction Jge( string label ) {
			return Jump ("ge", label);
		}

		public static Instruction Jg( string label ) {
			return Jump ("g", label);
		}

		public static Instruction Je( string label ) {
			return Jump ("e", label);
		}

		public static Instruction Jne( string label ) {
			return Jump ("ne", label);
		}

		#endregion

		#region cmovxx

		public static Instruction Cmov<T>( string cond_type, RegisterNode reg, T c ) {
			return new Instruction (
				map => string.Format ("cmov{0} {1}, {2}", cond_type, map[reg], c),
				use (reg), define (reg));
		}

		public static Instruction Cmovle<T>( RegisterNode reg, T c ) {
			return Cmov ("le", reg, c);
		}

		public static Instruction Cmovl<T>( RegisterNode reg, T c ) {
			return Cmov ("l", reg, c);
		}

		public static Instruction Cmovge<T>( RegisterNode reg, T c ) {
			return Cmov ("ge", reg, c);
		}

		public static Instruction Cmovg<T>( RegisterNode reg, T c ) {
			return Cmov ("g", reg, c);
		}

		public static Instruction Cmove<T>( RegisterNode reg, T c ) {
			return Cmov ("e", reg, c);
		}

		public static Instruction Cmovne<T>( RegisterNode reg, T c ) {
			return Cmov ("ne", reg, c);
		}

		#endregion

		#region lea
		#endregion

		#region shl shr

		public static Instruction Shl<T>( RegisterNode reg, ConstantNode<T> c) {
			return binopInstruction ("shl", reg, c);
		}

		public static Instruction Shr<T>( RegisterNode reg, ConstantNode<T> c) {
			return binopInstruction ("shr", reg, c);
		}

		#endregion
	}
}

