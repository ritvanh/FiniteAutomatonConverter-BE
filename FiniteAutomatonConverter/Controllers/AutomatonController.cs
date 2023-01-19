using FiniteAutomatonConverter.DTOs;
using FiniteAutomatonConverter.DTOs.AutomatonVizualiserDTOs;
using FiniteAutomatonConverter.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiniteAutomatonConverter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AutomatonController : ControllerBase
    {
        private readonly IAutomatonConverter _automatonConverter;
        public AutomatonController(IAutomatonConverter automatonConverter)
        {
            _automatonConverter = automatonConverter;
        }
        [HttpPost("epsilonNfa")]
        public async Task<IActionResult> VizualizeEpsilonNfaObject(AutomatonDto req)
        {
            var automaton = new DomainEntities.Automaton(req);
            var nfa = await _automatonConverter.ConvertEpsilonNfaToNfa(automaton);
            var dfa = await _automatonConverter.ConvertNfaToDfa(nfa);
            var mdfa = await _automatonConverter.MinimizeDfa(dfa);

            var dto = new AutomatonVizualizerClusterDto();
            dto.Enfa = new AutomatonVizualizerDto(automaton, "epsilonNfa");
            dto.Nfa = new AutomatonVizualizerDto(nfa, "nfa");
            dto.Dfa = new AutomatonVizualizerDto(dfa, "dfa");
            dto.MinimizedDfa = new AutomatonVizualizerDto(mdfa, "m-dfa");

            return Ok(dto);
        }

    }
}
