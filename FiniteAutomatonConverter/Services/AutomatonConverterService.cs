using FiniteAutomatonConverter.BaseEntities;
using FiniteAutomatonConverter.Services.Contracts;

namespace FiniteAutomatonConverter.Services
{
    public class AutomatonConverterService : IAutomatonConverter
    {
        public Task<FiniteAutomaton> ConvertEpsilonNfaToNfa(FiniteAutomaton enfa)
        {
            throw new NotImplementedException();
        }

        public Task<FiniteAutomaton> ConvertNfaToDfa(FiniteAutomaton nfa)
        {
            throw new NotImplementedException();
        }

        public Task<FiniteAutomaton> MinimizeDfa(FiniteAutomaton dfa)
        {
            throw new NotImplementedException();
        }
    }
}
