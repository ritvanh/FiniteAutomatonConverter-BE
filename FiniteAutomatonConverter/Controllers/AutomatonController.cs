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
        [HttpPost("aut")]
        public async Task<IActionResult> postMe(AutomatonDto req)
        {
            return Ok(new AutomatonVizualizerDto(req));
        }

        [HttpPost("viz")]
        public async Task<IActionResult> posthah(AutomatonVizualizerDto dto)
        {
            return Ok(dto);
        }
    }
}
