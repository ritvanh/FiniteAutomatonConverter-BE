using FiniteAutomatonConverter.DomainEntities;

namespace FiniteAutomatonConverter.Services.Contracts
{
    public interface IAutomatonConverter
    {
        Task<Automaton> ConvertEpsilonNfaToNfa(Automaton enfa);
        Task<Automaton> ConvertNfaToDfa(Automaton nfa);
        Task<Automaton> MinimizeDfa(Automaton dfa);
    }
}
