using Microsoft.AspNetCore.Mvc;
using TMR_AgentRegistration.Application.Dtos;
using TMR_AgentRegistration.Application.IServices;

namespace TMR_AgentRegistrationApi.Controllers.FrontOffice
{
        [ApiController]
        [Route("api/[controller]")]
        public class AgentApplicationController : ControllerBase
        {
            private readonly IAgentApplicationService _service;

            public AgentApplicationController(IAgentApplicationService service)
            {
                _service = service;
            }
            [HttpPost("save")]
            public async Task<IActionResult> Save([FromForm] AgentApplicationRequest request)
            {
                var result = await _service.SaveApplicationAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            [HttpGet("{applicationNumber}")]
            public async Task<IActionResult> Get(int applicationNumber)
            {
                var result = await _service.GetApplicationAsync(applicationNumber);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }

            [HttpGet("by-party/{partyCode}")]
            public async Task<IActionResult> GetByParty(string partyCode)
            {
                var result = await _service.GetApplicationByPartyAsync(partyCode);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }
        }
 }

