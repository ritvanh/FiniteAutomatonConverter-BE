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

            var dto = new AutomatonVizualizerClusterDto();
            dto.Enfa = new AutomatonVizualizerDto(automaton, "epsilonNfa");
            dto.Nfa = new AutomatonVizualizerDto(await _automatonConverter.ConvertEpsilonNfaToNfa(automaton), "nfa");
            dto.Dfa = new AutomatonVizualizerDto(await _automatonConverter.ConvertNfaToDfa(automaton), "dfa");
            dto.MinimizedDfa = new AutomatonVizualizerDto(await _automatonConverter.MinimizeDfa(automaton), "m-dfa");

            return Ok(dto);
        }

    }
}
