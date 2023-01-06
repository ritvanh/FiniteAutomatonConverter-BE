namespace FiniteAutomatonConverter.DTOs
{
    public class AutomatonDto
    {
        public List<string> Alphabet { get; set; }
        public List<TransitionDto> AllTransitions { get; set; }
        public List<StateDto> States { get; set; }
        
    }
}
