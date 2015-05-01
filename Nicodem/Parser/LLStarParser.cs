using System;
using System.Linq;
using System.Collections.Generic;
using Nicodem.Core;
using Nicodem.Source;
//using Strilanc.Value;

namespace Nicodem.Parser
{
	public class LLStarParser<TSymbol> : IParser<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
	{
		private readonly Grammar<TSymbol> _grammar;

		public LLStarParser(Grammar<TSymbol> grammar)
		{
			if(grammar.HasLeftRecursion) {
                throw new ArgumentException("Grammar has left recursion");
			}
            _grammar = grammar;
		}

        public IParseTree<TSymbol> Parse(IEnumerable<ParseLeaf<TSymbol>> word)
		{
            var memoizedWord = new MemoizedInput<ParseLeaf<TSymbol>>(word);
            var result = ParseTerm(_grammar.Start, memoizedWord, memoizedWord.Begin);
            // whole input has to be eaten
			if(result && result.Iterator == memoizedWord.End) {
				return result.Tree;
			} else {
            	return null;
			}
		}

		private ParseResult<TSymbol> ParseTerm(TSymbol term, MemoizedInput<ParseLeaf<TSymbol>> word, MemoizedInput<ParseLeaf<TSymbol>>.Iterator input)
		{
			var dfa = _grammar.Automatons[term];
			var children = new List<IParseTree<TSymbol>>(); 

			var it = input;
			var state = dfa.Start;
			var eof = ParserUtils<TSymbol>.GetEOF();

            var builder = new LookaheadDfaBuilder<TSymbol>();

			while(true) {
                LookaheadDfa<TSymbol> lookeaheadDfa = builder.Build(_grammar, it.Current.Symbol /* ? */, state);
				var lookState = lookeaheadDfa.Start;
                TSymbol currentSymbol = (it != word.End) ? it.Current.Symbol : eof;

                if(state.Accepting > 0 && _grammar.Follow[term].Contains(currentSymbol)) {
					var parsedTree = new ParseBranch<TSymbol>(
						GetFragmentRange(input.Current.Fragment, children.Last().Fragment),
						term, 
						_grammar.WhichProduction[state.Accepting], 
						children);

					return new ParseResult<TSymbol>(parsedTree, it);
				}

				var lookIt = it;
				while(lookState.Accepting == 0) {
                	TSymbol lookSym = (lookIt != word.End) ? lookIt.Current.Symbol : eof;
					lookState = FindTransition(lookState.Transitions, lookSym);
					lookIt = lookIt.Next();
				}

				var decisions = lookeaheadDfa.Decisions[lookState.Accepting];
				if(decisions.Count() > 1) {

					throw new NotImplementedException("Backtracking not implemented");
				} else if(!decisions.Any()) {

					return ReturnError(term, children, input, it);
				}

				TSymbol transSymbol = (TSymbol) decisions.ElementAt(0);
				if(_grammar.IsTerminal(transSymbol)) {

					children.Add(new ParseLeaf<TSymbol>(it.Current.Fragment, currentSymbol));
					it = it.Next();
				} else {

					var result = ParseTerm(transSymbol, word, it);
					if(!result) {
						return ReturnError(term, children, input, it);
					}
					children.Add(result.Tree);
					it = result.Iterator;
				}
				
				state = FindTransition(state.Transitions, transSymbol);
			}
		}

		private ParseResult<TSymbol> ReturnError(TSymbol term, IList<IParseTree<TSymbol>> children, 
				MemoizedInput<ParseLeaf<TSymbol>>.Iterator input, MemoizedInput<ParseLeaf<TSymbol>>.Iterator currentIt)
		{
			var branch = new ParseBranch<TSymbol>(
				GetFragmentRange(input.Current.Fragment, children.Last().Fragment), 
					term, 
					_grammar.Productions[term][0],  // TODO could not parse any productions
					children);
			return new ParseResult<TSymbol>(branch, currentIt, false);
		}

		private static IFragment GetFragmentRange(IFragment begin, IFragment end) {
			return new OriginFragment(begin.Origin, begin.GetBeginOriginPosition(), end.GetEndOriginPosition());
		}

		// TODO use binary search if possible and extract it somewhere
		private static DfaState<TSymbol> FindTransition(IReadOnlyList<KeyValuePair<TSymbol, DfaState<TSymbol>>> transitions, TSymbol edge)
		{
			return transitions.FirstOrDefault(kv => kv.Key.Equals(edge)).Value;
		}
	}
}

