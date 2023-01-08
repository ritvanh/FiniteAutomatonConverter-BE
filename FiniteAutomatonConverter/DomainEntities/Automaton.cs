﻿using FiniteAutomatonConverter.DTOs;
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

        public List<string> GetEpsilonClosureByStateValue(string stateValue)
        {

            var epsilonTransitions = GetStatesReachableByInput(stateValue, Constants.Epsilon);

            var epsilonReachableStates = new List<string>(epsilonTransitions);
            var epsilonReachableStatesProcessor = new Queue<string>(epsilonTransitions);

            while (epsilonReachableStatesProcessor.Any())
            {
                var newEpsilonReachableStates = GetStatesReachableByInput(epsilonReachableStatesProcessor.Dequeue(), Constants.Epsilon);

                epsilonReachableStates.AddRange(newEpsilonReachableStates);
                newEpsilonReachableStates.ForEach(o => epsilonReachableStatesProcessor.Enqueue(o));
            }
            epsilonReachableStates.Add(stateValue);

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
