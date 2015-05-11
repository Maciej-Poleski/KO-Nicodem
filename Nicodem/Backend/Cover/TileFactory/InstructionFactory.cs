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

		static Instruction binopInstruction( string mnemonik, RegisterNode dst, RegisterNode src ) {
			return new Instruction (
				map => string.Format ("{0} {1}, {2}", mnemonik, map [dst], map [src]),
				use (dst, src), define (dst));
		}

		static Instruction binopInstruction<T>( string mnemonik, RegisterNode dst, ConstantNode<T> c ) {
			return new Instruction (
				map => string.Format ("{0} {1}, {2}", mnemonik, map [dst], c.Value),
				use (dst), define (dst));
		}

		public static Instruction Label( LabelNode label ) {
			return new Instruction (
				map => string.Format ("{0}:", label.Label),
				use (), define ());
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

		#region from memory

		public static Instruction MoveFromMemory( RegisterNode dst, RegisterNode src ) {
			return new Instruction (
				map => string.Format ("mov {0}, [{1}]", map [dst], map [src]),
				use (src, dst), define (dst));
		}

		public static Instruction MoveFromMemory<T>( RegisterNode dst, ConstantNode<T> c ) {
			return new Instruction (
				map => string.Format ("mov {0}, [{1}]", map [dst], c.Value),
				use (dst), define (dst));
		}

		#endregion

		#region to memory

		public static Instruction MoveToMemory( RegisterNode dst, RegisterNode src ) {
			return new Instruction (
				map => string.Format ("mov [{0}], {1}", map [dst], map [src]),
				use (src, dst), define ());
		}

		public static Instruction MoveToMemory<T>( RegisterNode dst, ConstantNode<T> val ) {
			return new Instruction (
				map => string.Format ("mov [{0}], {1}", map [dst], val.Value),
				use (dst), define ());
		}

		public static Instruction MoveToMemory<T>( ConstantNode<T> c, RegisterNode src ) {
			return new Instruction (
				map => string.Format ("mov [{0}], {1}", c.Value, map [src]),
				use (src), define ());
		}

		public static Instruction MoveToMemory<T>( ConstantNode<T> addr, ConstantNode<T> val ) {
			return new Instruction (
				map => string.Format ("mov [{0}], {1}", addr.Value, val.Value),
				use (), define ());
		}

		#endregion

		#endregion

		#region add, sub, mul, xor, and, or

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

		public static Instruction Cmp<T>( ConstantNode<T> c, RegisterNode right ) {
			return new Instruction (
				map => string.Format ("cmp {0}, {1}", c.Value, map [right]),
				use (right), define ());
		}

		#endregion

		#region jump

		public static Instruction Jump( string cond_type, LabelNode label ) {
			return new Instruction (
				map => string.Format ("j{0} {1}", cond_type, label.Label),
				use (), define ());
		}

		public static Instruction Jmp( LabelNode label ) {
			return Jump ("mp", label);
		}

		public static Instruction Jle( LabelNode label ) {
			return Jump ("le", label);
		}

		public static Instruction Jl( LabelNode label ) {
			return Jump ("l", label);
		}

		public static Instruction Jge( LabelNode label ) {
			return Jump ("ge", label);
		}

		public static Instruction Jg( LabelNode label ) {
			return Jump ("g", label);
		}

		public static Instruction Je( LabelNode label ) {
			return Jump ("e", label);
		}

		public static Instruction Jne( LabelNode label ) {
			return Jump ("ne", label);
		}

		#endregion

		#region cmovxx

		public static Instruction Cmov( string cond_type, RegisterNode reg1, RegisterNode reg2 ) {
			return new Instruction (
				map => string.Format ("cmov{0} {1}, {2}", cond_type, map[reg1], map[reg2]),
				use (reg1, reg2), define (reg1));
		}

		public static Instruction Cmovle( RegisterNode reg1, RegisterNode reg2 ) {
			return Cmov ("le", reg1, reg2);
		}

		public static Instruction Cmovl( RegisterNode reg1, RegisterNode reg2 ) {
			return Cmov ("l", reg1, reg2);
		}

		public static Instruction Cmovge( RegisterNode reg1, RegisterNode reg2 ) {
			return Cmov ("ge", reg1, reg2);
		}

		public static Instruction Cmovg( RegisterNode reg1, RegisterNode reg2 ) {
			return Cmov ("g", reg1, reg2);
		}

		public static Instruction Cmove( RegisterNode reg1, RegisterNode reg2 ) {
			return Cmov ("e", reg1, reg2);
		}

		public static Instruction Cmovne( RegisterNode reg1, RegisterNode reg2 ) {
			return Cmov ("ne", reg1, reg2);
		}

		#endregion

		#region setxx

		public static Instruction Set( string cond_type, RegisterNode reg ) {
			return new Instruction (
				map => string.Format ("set{0} {1}", cond_type, map [reg]),
				use (reg), define (reg));
		}

		public static Instruction Setle( RegisterNode reg ) {
			return Set ("le", reg);
		}

		public static Instruction Setl( RegisterNode reg ) {
			return Set ("l", reg);
		}

		public static Instruction Setge( RegisterNode reg ) {
			return Set ("ge", reg);
		}

		public static Instruction Setg( RegisterNode reg ) {
			return Set ("g", reg);
		}

		public static Instruction Sete( RegisterNode reg ) {
			return Set ("e", reg);
		}

		public static Instruction Setne( RegisterNode reg ) {
			return Set ("ne", reg);
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

