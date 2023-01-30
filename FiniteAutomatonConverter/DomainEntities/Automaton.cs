using FiniteAutomatonConverter.DTOs;
using FiniteAutomatonConverter.Models;

namespace FiniteAutomatonConverter.DomainEntities
{
    public class Automaton
    {
        public string InitialState { get; set; }
        public List<string> Alphabet { get; set; }
        public Dictionary<string, List<KeyValuePair<string, string>>> States { get; set; }
        public List<string> FinalStates { get; set; }
        public Automaton(AutomatonDto dto)
        {
            InitialState = dto.InitialState;
            Alphabet = dto.Alphabet;
            FinalStates = dto.States.Where(x => x.IsFinal).Select(x => x.Value).ToList();

            var statesDict = new Dictionary<string, List<KeyValuePair<string, string>>>();

            foreach(var state in dto.States)
            {
                var tempKvpList = new List<KeyValuePair<string, string>>();
                foreach(var transition in dto.AllTransitions.Where(x => x.From == state.Value))
                {
                    tempKvpList.Add(new KeyValuePair<string, string>(transition.WithInput, transition.To));
                }
                statesDict.Add(state.Value, tempKvpList);
            }
            States = statesDict;
        }
        public Automaton() { }

        public List<string> GetEpsilonClosureByStateValue(string stateValue)
        {

            var epsilonTransitions = GetStatesReachableByInput(stateValue, Constants.Epsilon);

            var epsilonReachableStates = new List<string>(epsilonTransitions);
            var epsilonReachableStatesProcessor = new Queue<string>(epsilonTransitions);

            while (epsilonReachableStatesProcessor.Any())
            {
                var currentState = epsilonReachableStatesProcessor.Dequeue();

                GetStatesReachableByInput(currentState, Constants.Epsilon).ForEach(x =>
                {
                    if (!(epsilonReachableStates.Contains(x) || x == stateValue))
                    {
                        epsilonReachableStates.Add(x);
                        epsilonReachableStatesProcessor.Enqueue(x);
                    }
                });
            }
            epsilonReachableStates.Add(stateValue);

            if (epsilonReachableStates.Intersect(FinalStates).Any())
            {
                if (!FinalStates.Contains(stateValue))
                {
                    FinalStates.Add(stateValue);
                }
            }

            return epsilonReachableStates;
        }
        public List<string> GetStatesReachableByInput(string stateValue,string input)
        {
            var state = States[stateValue];
            _ = state ?? throw new KeyNotFoundException("gjendja nuk u gjet");

            return state
                .Where(x => x.Key == input)
                .Select(x => x.Value)
                .ToList();
        }
        public List<string> GetStatesReachableByAnyInput(string stateValue)
        {
            var states = new List<string>();
            var stateProcessor = new Queue<string>();
            stateProcessor.Enqueue(stateValue);

            while (stateProcessor.Any())
            {
                var currentState = stateProcessor.Dequeue();
                this.States[currentState].ForEach(x =>
                {
                    if (!states.Contains(x.Value))
                    {
                        states.Add(x.Value);
                        stateProcessor.Enqueue(x.Value);
                    }
                });
            }

            return states;
        }
        public List<string> GetFirstEquivalentStateOccurence()
        {
            foreach (var state in this.States)
            {
                var eqStates = new List<string>();
                eqStates.Add(state.Key);
                foreach (var sub in this.States)
                {
                    if (state.Key != sub.Key)
                    {
                        if (Enumerable.SequenceEqual(sub.Value.OrderBy(x => x.Key), state.Value.OrderBy(x => x.Key)))
                        {
                            eqStates.Add(sub.Key);
                        }
                    }
                }
                if (eqStates.Count > 1)
                {
                    var groupedEqStates = GetGroupedStates(this.FinalStates, eqStates);
                    if (groupedEqStates.Count > 0)
                        return groupedEqStates[0];
                }
            }
            return null;
        }
        public List<List<string>> GetGroupedStates(List<string> finalStates, List<string> equalStates)
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

        public void ManageDeadStateOnDfaCreation()
        {
            foreach (var state in this.States)
            {
                var unusedCharacters = this.Alphabet.Where(x => state.Value.Where(y => y.Key == x).Count() < 1).ToList();
                unusedCharacters.Remove(Constants.Epsilon);
                unusedCharacters.ForEach(x =>
                {
                    state.Value.Add(new KeyValuePair<string, string>(x, Constants.DeadState));
                });
            }

            if (this.States.Any(x => x.Value.Any(y => y.Value == Constants.DeadState)))
            {
                var tr = new List<KeyValuePair<string, string>>();
                var errAlphabet = this.Alphabet;
                errAlphabet.Remove(Constants.Epsilon);
                tr.Add(new KeyValuePair<string, string>(String.Join(",", errAlphabet), Constants.DeadState));
                this.States.Add(Constants.DeadState, tr);
            }
        }
        public Dictionary<string,List<string>> GetStateTransitionsGroupedByInputByName(string stateName)
        {
            return this.States[stateName]
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Select(kvp => kvp.Value)
                    .ToList());
        }
    }
}
