using System;
using System.Linq;
using System.Collections.Generic;
using Nicodem.Core;
using Nicodem.Source;
using Strilanc.Value;

namespace Nicodem.Parser
{
	public class LLStarParser<TProduction> : IParser<TProduction> where TProduction:IProduction
	{
		private readonly Grammar<TProduction> _grammar;

		public LLStarParser(Grammar<TProduction> grammar)
		{
			if(grammar.HasLeftRecursion()) {
                throw new ArgumentException("Grammar has left recursion");
			}
            _grammar = grammar;
		}

        public IParseTree<TProduction> Parse(IEnumerable<IParseTree<TProduction>> word)
		{
			var memoizedWord = new MemoizedInput<IParseTree<TProduction>>(word);
			var result = ParseTerm(_grammar.Start, memoizedWord, memoizedWord.Begin);

            // whole input has to be eaten
            if(result && result.Iterator == memoizedWord.End) {
                return result.Tree;
            } else {
                return null;
            }
		}

		private ParseResult<TProduction> ParseTerm(ISymbol term, MemoizedInput<IParseTree<TProduction>> word, MemoizedInput<IParseTree<TProduction>>.Iterator input)
		{
			var dfa = _grammar.Automatons[term];
			var children = new List<IParseTree<TProduction>>(); 

			var it = input;
			var state = dfa.Start;

			while(true) {
				LookaheadDfa lookeaheadDfa = null; // TODO will be initialized
				var lookState = lookeaheadDfa.Start;
                ISymbol currentSymbol = (it != word.End) ? it.Current.Symbol : term.EOF;

                if(state.Accepting > 0 && _grammar.Follow[term].Contains(currentSymbol)) {
					var parsedTree = new ParseBranch<TProduction>(
						GetFragmentRange(input.Current.Fragment, children.Last().Fragment),
						term, 
						_grammar.WhichProduction[state.Accepting], 
						children);

					return new ParseResult<TProduction>(parsedTree, it);
				}

				var lookIt = it;
				while(lookState.Accepting == 0) {
                	ISymbol lookSym = (lookIt != word.End) ? lookIt.Current.Symbol : term.EOF;
					lookState = FindTransition(lookState.Transitions, lookSym);
					lookIt = lookIt.Next();
				}

				var decisions = lookeaheadDfa.Decisions[lookState.Accepting];
				if(decisions.Count() > 1) {

					throw new NotImplementedException("Backtracking not implemented");
				} else if(!decisions.Any()) {

					return ReturnError(term, children, input, it);
				}

				ISymbol transSymbol = decisions.ElementAt(0).ForceGetValue();
				if(_grammar.IsTerminal(transSymbol)) {

					children.Add(new ParseLeaf<TProduction>(it.Current.Fragment, currentSymbol));
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

		private ParseResult<TProduction> ReturnError(ISymbol term, IList<IParseTree<TProduction>> children, 
				MemoizedInput<IParseTree<TProduction>>.Iterator input, MemoizedInput<IParseTree<TProduction>>.Iterator currentIt)
		{
			var branch = new ParseBranch<TProduction>(
				GetFragmentRange(input.Current.Fragment, children.Last().Fragment), 
					term, 
					_grammar.Productions[term][0],  // TODO could not parse any productions
					children);
			return new ParseResult<TProduction>(branch, currentIt, false);
		}

		private static IFragment GetFragmentRange(IFragment begin, IFragment end) {
			return new OriginFragment(begin.Origin, begin.GetBeginOriginPosition(), end.GetEndOriginPosition());
		}

		// TODO use binary search if possible and extract it somewhere
		private static DfaState<ISymbol> FindTransition(IReadOnlyList<KeyValuePair<ISymbol, DfaState<ISymbol>>> transitions, ISymbol edge)
		{
			return transitions.FirstOrDefault(kv => kv.Key.Equals(edge)).Value;
		}
	}
}

