using System;

namespace Nicodem.Parser
{
	// Attention! This is a temporary fix to allow this to compile.
	// We expect IFragment to be without type parameters. This is not implemented yet.
	// So let Parser's interfaces temporarily depend on this one and fix it when Source is updated.
	public interface IOrigin
	{
	}
	public interface IFragment
	{
		int AfeterEndCharacterInLinePosition { get; }
		int EndLineNumber { get; }
		int BeginCharacterInLinePosition { get; }
		int BeginLineNumber { get; }
		IOrigin Origin { get; }
	}
}

