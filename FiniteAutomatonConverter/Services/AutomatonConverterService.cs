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

            //var newStates = new Queue<string[]>();
            //newStates.Enqueue(new string[] { enfa.InitialState });

            //while (newStates.Any())
            //{
            //    var currentProcessingState = newStates.Dequeue();

            //    var newInitialStateTransition = new List<KeyValuePair<string, string>>();
            //    foreach (var state in currentProcessingState)
            //    {
            //        var groupedInitialStateTransitions = nfa.States[nfa.InitialState]
            //        .GroupBy(x => x.Key)
            //        .ToDictionary(x => x.Key, x => x.Select(kvp => kvp.Value)
            //        .ToList());


            //        foreach (var x in groupedInitialStateTransitions)
            //        {
            //            if (x.Value.Count == 1)
            //            {
            //                newInitialStateTransition.Add(new KeyValuePair<string, string>(x.Key, x.Value[0]));
            //            }
            //            else
            //            {
            //                newStates.Enqueue(x.Value.ToArray());
            //                newInitialStateTransition.Add(new KeyValuePair<string, string>(x.Key, String.Join(",", x.Value)));
            //            }
            //        }
            //    }
            //    newTransitions.Add(nfa.InitialState, newInitialStateTransition);

            //}
            var nfa = await ConvertEpsilonNfaToNfa(enfa);
            var newTransitions = new Dictionary<List<string>, List<KeyValuePair <string, List<string>>>>();
            var newStates = new List<List<string>>();
            newStates.Add(new List<string>() { nfa.InitialState});
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
                    foreach(var kvp in groupedTransitions)
                    {
                        //if input already exists, add found states
                        if (currentStateTransitions.Any(x => x.Key == kvp.Key))
                        {
                            currentStateTransitions.FirstOrDefault(x => x.Key == kvp.Key).Value.AddRange(kvp.Value);
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
                var newStateLists = currentStateTransitions.Where(x => !newStates.Contains(x.Value)).Select(x=>x.Value).ToList();
                newStates.AddRange(newStateLists);
                newStateLists.ForEach(x => newStatesProcessor.Enqueue(x));
            }



        }

        public Task<Automaton> MinimizeDfa(Automaton dfa)
        {
            throw new NotImplementedException();
        }

    }
}
