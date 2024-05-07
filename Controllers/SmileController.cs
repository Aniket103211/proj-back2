using CCA_BAL.DTO;
using CCA_BAL.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCA1.Controllers
{
    [Route("api")]
    [ApiController]
    public class SmileController : ControllerBase
    {
        private readonly ISmileService _service;
        public SmileController(ISmileService service)
        {
            _service = service;
        }
        [HttpGet("[controller]/{month}/{rpId}")]
        public async Task<IActionResult> getSmile(int month, string rpId)
        {
            var res = await _service.GetSmile(month, rpId);
            if (res!=null)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(500, "As occupancy is less than zero which cannot be possible");
            }
        }
    }
}
