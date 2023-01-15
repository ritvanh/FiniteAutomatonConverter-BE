namespace FiniteAutomatonConverter.DTOs.AutomatonVizualiserDTOs
{
    public class AutomatonVizualizerClusterDto
    {
        public AutomatonVizualizerDto Enfa { get; set; }
        public AutomatonVizualizerDto Nfa { get; set; }
        public AutomatonVizualizerDto Dfa { get; set; }
        public AutomatonVizualizerDto MinimizedDfa { get; set; }
    }
}
