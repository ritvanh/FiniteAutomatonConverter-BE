namespace FiniteAutomatonConverter.BaseEntities
{
    public class FiniteAutomaton
    {
        public string InitialState { get; set; }
        public List<string> FinalStates { get; set; }
        public List<string> AutomatonStates { get; set; }
        public List<string> Alphabet { get; set; }
        public List<Transition> Transitions { get; set; }

    }
}
