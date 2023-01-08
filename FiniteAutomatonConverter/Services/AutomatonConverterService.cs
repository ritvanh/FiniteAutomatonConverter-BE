using FiniteAutomatonConverter.DomainEntities;
using FiniteAutomatonConverter.DTOs;
using FiniteAutomatonConverter.DTOs.AutomatonVizualiserDTOs;
using FiniteAutomatonConverter.Models;
using FiniteAutomatonConverter.Services.Contracts;

namespace FiniteAutomatonConverter.Services
{
    public class AutomatonConverterService : IAutomatonConverter
    {
        public async Task<Automaton> ConvertEpsilonNfaToNfa(Automaton enfa)
        {
            var newTransitions = new Dictionary<string, List<KeyValuePair<string, string>>>();
            //foreach state
            foreach (var state in enfa.States)
            {
                var currentStateTransitionList = new List<KeyValuePair<string, string>>();

                //foreach epsilon closure
                enfa.GetEpsilonClosureByStateValue(state.Key)
                    .ForEach(state =>
                {
                    //foreach transition of epsilon enclosure
                    enfa.States[state].ForEach(transition =>
                    {
                        //add nfa transition for every epsilon closure of current transition of first epsilon enclosure
                        if (transition.Key != Constants.Epsilon)
                        {
                            enfa.GetEpsilonClosureByStateValue(transition.Value)
                            .ForEach(secondEnclosure =>
                            {
                                currentStateTransitionList
                                .Add(new KeyValuePair<string, string>(transition.Key, secondEnclosure));
                            });
                        }
                    });

                });
                newTransitions.Add(state.Key, currentStateTransitionList);
            }
            enfa.States = newTransitions;
            return enfa;
        }

        public Task<Automaton> ConvertNfaToDfa(Automaton nfa)
        {
            throw new NotImplementedException();
        }

        public Task<Automaton> MinimizeDfa(Automaton dfa)
        {
            throw new NotImplementedException();
        }

    }
}
