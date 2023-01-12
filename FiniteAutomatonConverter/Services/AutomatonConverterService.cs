using FiniteAutomatonConverter.DomainEntities;
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
                newTransitions.Add(state.Key, currentStateTransitionList.Distinct().ToList());
            }
            enfa.States = newTransitions;
            return enfa;
        }

        public async Task<Automaton> ConvertNfaToDfa(Automaton enfa)
        {
            var nfa = await ConvertEpsilonNfaToNfa(enfa);
            var newTransitions = new Dictionary<string, List<KeyValuePair<string, string>>>();
            var newStates = new Queue<string[]>();
            newStates.Enqueue(new string[] { enfa.InitialState });

            while (newStates.Any())
            {
                var currentProcessingState = newStates.Dequeue();

                var newInitialStateTransition = new List<KeyValuePair<string, string>>();
                foreach (var state in currentProcessingState)
                {
                    var groupedInitialStateTransitions = nfa.States[nfa.InitialState]
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Select(kvp => kvp.Value)
                    .ToList());

                    
                    foreach (var x in groupedInitialStateTransitions)
                    {
                        if (x.Value.Count == 1)
                        {
                            newInitialStateTransition.Add(new KeyValuePair<string, string>(x.Key, x.Value[0]));
                        }
                        else
                        {
                            newStates.Enqueue(x.Value.ToArray());
                            newInitialStateTransition.Add(new KeyValuePair<string, string>(x.Key, String.Join(",", x.Value)));
                        }
                    }
                }
                newTransitions.Add(nfa.InitialState, newInitialStateTransition);

            }

            throw new NotImplementedException();
        }

        public Task<Automaton> MinimizeDfa(Automaton dfa)
        {
            throw new NotImplementedException();
        }

    }
}
