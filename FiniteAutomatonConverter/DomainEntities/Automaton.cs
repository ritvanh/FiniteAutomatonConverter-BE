using AutoMapper;
using FiniteAutomatonConverter.DTOs;
using FiniteAutomatonConverter.Models;
using System.Linq;

namespace FiniteAutomatonConverter.DomainEntities
{
    public class Automaton
    {
        public string InitialState { get; set; }
        public List<string> Alphabet { get; set; }
        public Dictionary<string, List<KeyValuePair<string, string>>> States { get; set; }
        public List<string> FinalStates { get; set; }

        public List<string> GetEpsilonClosureByStateValue(string stateValue)
        {

            var epsilonTransitions = GetStatesReachableByInput(stateValue, Constants.Epsilon);

            epsilonTransitions.Add(stateValue);

            var epsilonReachableStates = new List<string>(epsilonTransitions);
            var epsilonReachableStatesProcessor = new Queue<string>(epsilonTransitions);

            while (epsilonReachableStatesProcessor.Any())
            {
                var newEpsilonReachableStates = GetStatesReachableByInput(epsilonReachableStatesProcessor.Dequeue(), Constants.Epsilon);

                epsilonReachableStates.AddRange(newEpsilonReachableStates);
                newEpsilonReachableStates.ForEach(o => epsilonReachableStatesProcessor.Enqueue(o));
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
    }
}
