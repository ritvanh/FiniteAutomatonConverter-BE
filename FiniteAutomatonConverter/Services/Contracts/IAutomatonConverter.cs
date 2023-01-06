using FiniteAutomatonConverter.BaseEntities;

namespace FiniteAutomatonConverter.Services.Contracts
{
    public interface IAutomatonConverter
    {
        Task<FiniteAutomaton> ConvertEpsilonNfaToNfa(FiniteAutomaton enfa);
        Task<FiniteAutomaton> ConvertNfaToDfa(FiniteAutomaton nfa);
        Task<FiniteAutomaton> MinimizeDfa(FiniteAutomaton dfa);
    }
}
