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
            var converted = await _automatonConverter.ConvertEpsilonNfaToNfa(new DomainEntities.Automaton(req));
            return Ok(new AutomatonVizualizerDto(converted,"nfa"));
        }
    }
}
