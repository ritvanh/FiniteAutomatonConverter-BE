namespace FiniteAutomatonConverter.DTOs
{
    public class AutomatonDto
    {
        public string InitialState { get; set; }
        public List<string> Alphabet { get; set; }
        public List<TransitionDto> AllTransitions { get; set; }
        public List<StateDto> States { get; set; }
        
    }
}
