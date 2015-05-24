using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class InstructionFactoryTests
	{
		static readonly RegisterNode reg1 = new TemporaryNode ();
		static readonly RegisterNode reg2 = new TemporaryNode ();
		static readonly ConstantNode<long> constant = new ConstantNode<long> (10L);
		static readonly LabelNode label = new LabelNode ("myLabel");
		static readonly FunctionCallNode fun = new FunctionCallNode (new Function ("foo", new[]{ false }));

		[Test]
		public void Test_LabelInstructions () {
			var lst = new [] {
				InstructionFactory.Label (label)
			};

			foreach (var insn in lst) {
				Assert.IsFalse (insn.IsCopyInstruction);
				Assert.IsFalse (insn.IsJumpInstruction);
				Assert.IsTrue (insn.IsLabel);
				Assert.AreEqual (label.Label, insn.Label);
			}
		}

		[Test]
		public void Test_CopyInstructions () {
			var lst = new [] {
				InstructionFactory.Move (reg1, reg2)
			};

			foreach (var insn in lst) {
				Assert.IsTrue (insn.IsCopyInstruction);
				Assert.IsFalse (insn.IsJumpInstruction);
				Assert.IsFalse (insn.IsLabel);
				Assert.IsNull (insn.Label);
			}
		}

		[Test]
		public void Test_JumpInstructions () {
			var lst = new [] {
				InstructionFactory.Je (label),
				InstructionFactory.Jg (label),
				InstructionFactory.Jge (label),
				InstructionFactory.Jl (label),
				InstructionFactory.Jle (label),
				InstructionFactory.Jmp (label),
				InstructionFactory.Jne (label)
			};

			foreach (var insn in lst) {
				Assert.IsFalse (insn.IsCopyInstruction);
				Assert.IsTrue (insn.IsJumpInstruction);
				Assert.IsFalse (insn.IsLabel);
				Assert.AreEqual (label.Label, insn.Label);
			}
		}

		[Test]
		public void Test_NormalInstructions () {
			var lst = new[] {
				InstructionFactory.Add (reg1, reg2),
				InstructionFactory.Add (reg1, constant),
				InstructionFactory.Sub (reg1, reg2),
				InstructionFactory.Sub (reg1, constant),
				InstructionFactory.Xor (reg1, reg2),
				InstructionFactory.Xor (reg1, constant),
				InstructionFactory.And (reg1, reg2),
				InstructionFactory.And (reg1, constant),
				InstructionFactory.Or (reg1, reg2),
				InstructionFactory.Or (reg1, constant),
				InstructionFactory.Test (reg1, reg2),
				InstructionFactory.Test (reg1, constant),

				InstructionFactory.Shl (reg1, constant),
				InstructionFactory.Shr (reg1, constant),

				InstructionFactory.Cmp (reg1, reg2),
				InstructionFactory.Cmp (reg1, constant),
				InstructionFactory.Cmp (constant, reg2),

				InstructionFactory.Mul (reg1),
				InstructionFactory.Div (reg1),
				InstructionFactory.Inc (reg1),
				InstructionFactory.Dec (reg1),
				InstructionFactory.Neg (reg1),
				InstructionFactory.Not (reg1),

				InstructionFactory.Call (fun),
				InstructionFactory.Ret (),

				InstructionFactory.Pop (reg1),
				InstructionFactory.Push (reg1),

				InstructionFactory.Cmove (reg1, reg2),
				InstructionFactory.Cmovg (reg1, reg2),
				InstructionFactory.Cmovge (reg1, reg2),
				InstructionFactory.Cmovl (reg1, reg2),
				InstructionFactory.Cmovle (reg1, reg2),
				InstructionFactory.Cmovne (reg1, reg2),

				InstructionFactory.Sete (reg1),
				InstructionFactory.Setg (reg1),
				InstructionFactory.Setge (reg1),
				InstructionFactory.Setl (reg1),
				InstructionFactory.Setle (reg1),
				InstructionFactory.Setne (reg1),

				InstructionFactory.Call (fun)
			};

			foreach (var insn in lst) {
				Assert.IsFalse (insn.IsCopyInstruction);
				Assert.IsFalse (insn.IsJumpInstruction);
				Assert.IsFalse (insn.IsLabel);
				Assert.IsNull (insn.Label);
			}
		}
	}
}

