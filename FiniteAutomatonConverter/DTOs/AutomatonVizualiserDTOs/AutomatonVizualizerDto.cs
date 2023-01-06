namespace FiniteAutomatonConverter.DTOs.AutomatonVizualiserDTOs
{
    public class AutomatonVizualizerDto
    {
        public string Id { get; set; }
        public string Initial { get; set; } 

        public Dictionary<string, StateVizualizerDto> States { get; set; }
        public AutomatonVizualizerDto(AutomatonDto aut)
        {
            this.Id = "NFA";
            this.Initial = aut.States.FirstOrDefault(x => x.IsInitial).Value;
            this.States = new Dictionary<string, StateVizualizerDto>();

            foreach(var s in aut.States)
            {
                var stateViz = new StateVizualizerDto();
                if (s.IsFinal)
                    stateViz.Meta = true;

                stateViz.On = new Dictionary<string, string>();

                var stateTransitions = aut.AllTransitions.Where(x => x.From == s.Value);
                foreach(var t in stateTransitions)
                {
                    if(t.WithInput == "E")
                    {
                        t.WithInput = "ε";
                    }
                    stateViz.On.Add(t.WithInput, t.To);
                }

                this.States.Add(s.Value, stateViz);
            }
        }
    }
}
