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
                    var groupedTransitions = nfa.States[x]
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Select(kvp => kvp.Value)
                    .ToList());

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

            var dfaStates = new List<string>();
            var dfaFinalStates = new List<string>();
            newStates.ForEach(x =>
            {
                dfaStates.Add(string.Join(",", x));
                if (x.Intersect(nfa.FinalStates).Any())
                    dfaFinalStates.Add(string.Join(",", x));
            });
            var dfaTransitions = new Dictionary<string, List<KeyValuePair<string, string>>>();
            foreach (var state in newTransitions)
            {
                var currentTransitions = new List<KeyValuePair<string, string>>();
                state.Value.ForEach(x =>
                {
                    currentTransitions.Add(new KeyValuePair<string, string>(x.Key, string.Join(",", x.Value)));
                });
                dfaTransitions.Add(string.Join(",", state.Key), currentTransitions);
            }

            foreach (var state in dfaTransitions)
            {
                var unusedCharacters = nfa.Alphabet.Where(x => state.Value.Where(y => y.Key == x).Count() < 1).ToList();
                unusedCharacters.Remove(Constants.Epsilon);
                unusedCharacters.ForEach(x =>
                {
                    state.Value.Add(new KeyValuePair<string, string>(x, Constants.DeadState));
                });
            }

            if (dfaTransitions.Any(x => x.Value.Any(y => y.Value == Constants.DeadState)))
            {
                var tr = new List<KeyValuePair<string, string>>();
                var errAlphabet = nfa.Alphabet;
                errAlphabet.Remove(Constants.Epsilon);
                tr.Add(new KeyValuePair<string, string>(String.Join(",", errAlphabet), Constants.DeadState));
                dfaTransitions.Add(Constants.DeadState, tr);
            }

            return new Automaton()
            {
                Alphabet = nfa.Alphabet,
                InitialState = nfa.InitialState,
                FinalStates = dfaFinalStates,
                States = dfaTransitions
            };

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
            while (await GetFirstEquivalentStateOccurence(dfa.States,dfa.FinalStates) != null)
            {
                var equivalentStates = await GetFirstEquivalentStateOccurence(dfa.States,dfa.FinalStates);

                var newEquivalentStateValue = string.Join(",", equivalentStates);
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

            }
            return dfa;
        }

        public async Task<List<string>> GetFirstEquivalentStateOccurence(Dictionary<string,List<KeyValuePair<string,string>>> states,List<string> finalStates)
        { 
            foreach(var state in states)
            {
                var eqStates = new List<string>();
                eqStates.Add(state.Key);
                foreach (var sub in states)
                {
                    if (state.Key != sub.Key)
                    {
                        if(Enumerable.SequenceEqual(sub.Value.OrderBy(x=>x.Key),state.Value.OrderBy(x=>x.Key)))
                        {
                            eqStates.Add(sub.Key);
                        }
                    }
                }
                if (eqStates.Count > 1)
                {
                    var groupedEqStates = await GetGroupedStates(finalStates, eqStates);
                    if (groupedEqStates.Count > 0)
                        return groupedEqStates[0];
                }
            }
            return null;
        }
        public async Task<List<List<string>>> GetGroupedStates(List<string> finalStates, List<string> equalStates)
        {
            var fromFinal = new List<string>();
            var notFromFinal = new List<string>();
            equalStates.ForEach(x =>
            {
                if (finalStates.Contains(x))
                    fromFinal.Add(x);
                else
                    notFromFinal.Add(x);
            });
            var result = new List<List<string>>();
            if (fromFinal.Count > 1)
                result.Add(fromFinal);

            if (notFromFinal.Count > 1)
                result.Add(notFromFinal);

            return result;
        }

    }
}
