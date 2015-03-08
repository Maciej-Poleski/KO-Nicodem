namespace Nicodem.Lexer
{
    internal interface IDfa<T> where T : IDfaState<T> 
    {
        T Start { get; }
    }
}