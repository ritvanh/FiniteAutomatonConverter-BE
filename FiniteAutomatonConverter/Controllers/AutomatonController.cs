using FiniteAutomatonConverter.DTOs;
using FiniteAutomatonConverter.DTOs.AutomatonVizualiserDTOs;
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
        [HttpGet("epsilonNfa")]
        public async Task<IActionResult> VizualizeEpsilonNfaObject(AutomatonDto req)
        {
            return Ok(new AutomatonVizualizerDto(req));
        }
    }
}
