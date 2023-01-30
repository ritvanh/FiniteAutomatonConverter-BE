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
            enfa.Alphabet.Remove(Constants.Epsilon);
            return enfa;
        }

        public async Task<Automaton> ConvertNfaToDfa(Automaton enfa)
        {
            var nfa = await ConvertEpsilonNfaToNfa(enfa);
            var newTransitions = new Dictionary<List<string>, List<KeyValuePair<string, List<string>>>>();
            var newStates = new List<List<string>>();
            newStates.Add(new List<string>() { nfa.InitialState });
            var newStatesProcessor = new Queue<List<string>>();
            newStatesProcessor.Enqueue(new List<string>() { nfa.InitialState });

            while (newStatesProcessor.Any())
            {
                var currentState = newStatesProcessor.Dequeue();

                //finding transitions of current state(s)
                var currentStateTransitions = new List<KeyValuePair<string, List<string>>>();
                currentState.ForEach(x =>
                {
                    //grouping transitions by input to form lists on same input
                    var groupedTransitions = nfa.GetStateTransitionsGroupedByInputByName(x);

                    //adding them to current state transitions
                    foreach (var kvp in groupedTransitions)
                    {
                        //if input already exists, add found states
                        if (currentStateTransitions.Any(x => x.Key == kvp.Key))
                        {
                            currentStateTransitions
                            .FirstOrDefault(x => x.Key == kvp.Key).Value
                            .AddRange(kvp.Value.Where(x => !currentStateTransitions
                            .FirstOrDefault(x => x.Key == kvp.Key).Value
                            .Contains(x)));
                        }
                        //else add record for the newly found input
                        else
                        {
                            currentStateTransitions.Add(kvp);
                        }
                    }
                });
                //adding found transitions
                newTransitions.Add(currentState, currentStateTransitions);
                //adding states to process
                var newStateLists = currentStateTransitions.Where(x => !newStates.Contains(x.Value)).Select(x => x.Value).ToList();
                newStateLists.ForEach(x =>
                {
                    if (!newStates.Any(y => y.SequenceEqual(x)))
                    {
                        newStates.Add(x);
                        newStatesProcessor.Enqueue(x);
                    }
                });
            }

            var dfa = new Automaton()
            {
                FinalStates = new List<string>(),
                States = new Dictionary<string, List<KeyValuePair<string, string>>>(),
                Alphabet = nfa.Alphabet,
                InitialState = nfa.InitialState
            };
            newStates.ForEach(x =>
            {
                if (x.Intersect(nfa.FinalStates).Any())
                    dfa.FinalStates.Add(MergeStates(x));
            });
            foreach (var state in newTransitions)
            {
                var currentTransitions = new List<KeyValuePair<string, string>>();
                state.Value.ForEach(x =>
                {
                    currentTransitions.Add(new KeyValuePair<string, string>(x.Key, MergeStates(x.Value)));
                });
                dfa.States.Add(MergeStates(state.Key), currentTransitions);
            }

            dfa.ManageDeadStateOnDfaCreation();

            return dfa; 

        }
        
        public async Task<Automaton> MinimizeDfa(Automaton enfa)
        {
            var dfa = await ConvertNfaToDfa(await ConvertEpsilonNfaToNfa(enfa));
            //removing non reachable states
            var nonReachableStates = dfa.States.Select(x => x.Key).ToList();
            nonReachableStates.Remove(dfa.InitialState);
            dfa.GetStatesReachableByAnyInput(dfa.InitialState).ForEach(x => nonReachableStates.Remove(x));
            nonReachableStates.ForEach(x =>
            {
                dfa.States.Remove(x);
                if (dfa.FinalStates.Contains(x))
                    dfa.FinalStates.Remove(x);
            });
            
            //while there are equivalent states
            while (dfa.GetFirstEquivalentStateOccurence() != null)
            {
                var equivalentStates = dfa.GetFirstEquivalentStateOccurence();

                var newEquivalentStateValue = MergeStates(equivalentStates);
                var transitions = dfa.States[equivalentStates[0]];
                //remove old states
                equivalentStates.ForEach(x => dfa.States.Remove(x));
                //add new equivalent state
                dfa.States.Add(newEquivalentStateValue, transitions);

                //replace equivalent states with the new state
                foreach (var state in dfa.States)
                {
                    foreach (var t in state.Value.ToList())
                    {
                        if (equivalentStates.Contains(t.Value))
                        {
                            state.Value.Add(new KeyValuePair<string, string>(t.Key, newEquivalentStateValue));
                            state.Value.Remove(t);
                        }
                    }
                }

                if (equivalentStates.Contains(dfa.InitialState))
                    dfa.InitialState = newEquivalentStateValue;

                dfa.FinalStates.ForEach(x =>
                {
                    if (equivalentStates.Contains(x))
                        x = newEquivalentStateValue;
                });
                dfa.FinalStates = dfa.FinalStates.Distinct().ToList();

            }
            return dfa;
        }
        public string MergeStates(List<string> states) => String.Join(",", states);

    }
}
