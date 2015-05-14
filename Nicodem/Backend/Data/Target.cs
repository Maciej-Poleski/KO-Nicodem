﻿using System;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
	public class Target
	{
		public static readonly HardwareRegisterNode RAX = new HardwareRegisterNode ("rax");
		public static readonly HardwareRegisterNode RBX = new HardwareRegisterNode ("rbx");
		public static readonly HardwareRegisterNode RCX = new HardwareRegisterNode ("rcx");
		public static readonly HardwareRegisterNode RDX = new HardwareRegisterNode ("rdx");
		public static readonly HardwareRegisterNode RSP = new HardwareRegisterNode ("rsp");
		public static readonly HardwareRegisterNode RBP = new HardwareRegisterNode ("rbp");
		public static readonly HardwareRegisterNode RSI = new HardwareRegisterNode ("rsi");
        public static readonly HardwareRegisterNode RDI = new HardwareRegisterNode("rdi");
        public static readonly HardwareRegisterNode R8 = new HardwareRegisterNode("r8");
		public static readonly HardwareRegisterNode R9  = new HardwareRegisterNode ("r9");
		public static readonly HardwareRegisterNode R10 = new HardwareRegisterNode ("r10");
		public static readonly HardwareRegisterNode R11 = new HardwareRegisterNode ("r11");
		public static readonly HardwareRegisterNode R12 = new HardwareRegisterNode ("r12");
		public static readonly HardwareRegisterNode R13 = new HardwareRegisterNode ("r13");
		public static readonly HardwareRegisterNode R14 = new HardwareRegisterNode ("r14");
		public static readonly HardwareRegisterNode R15 = new HardwareRegisterNode ("r15");

		public Target () {
		}

		public Function CreateFunction() {
            throw new NotImplementedException();
			// return new Function();
            // FIXME You must provide local/temporary variables bitmap
		}
	}
}
