using Microsoft.AspNetCore.Mvc;
using TMR_AgentRegistration.Application.Dtos;
using TMR_AgentRegistration.Application.Dtos.AgentLogin;
using TMR_AgentRegistration.Application.IServices;

namespace TMR_AgentRegistrationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentLoginController : Controller
    {

        private readonly IAgentLoginService _agentLoginService;
        private readonly IAccountSetupOptionService _accountSetupOptionService;
        private readonly IProfessionalApplicantService _professionalApplicantService;

        public AgentLoginController(IAgentLoginService agentLoginService, IAccountSetupOptionService accountSetupOptionService, IProfessionalApplicantService professionalApplicantService)
        {
            _agentLoginService = agentLoginService;
            _accountSetupOptionService = accountSetupOptionService;
            _professionalApplicantService = professionalApplicantService;
        }
      
        [HttpPost("check-qualified-agent")]
        public async Task<IActionResult> CheckQualifiedAgent([FromBody] CheckQualifiedAgentRequestDto dto)
        {
             var result = await _agentLoginService.CheckQualifiedAgentAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
        {
            var result = await _agentLoginService.VerifyOtpAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("create-credential")]
        public async Task<IActionResult> CreateLoginCredential([FromBody] CreateLoginCredentialRequestDto dto)
        {
            var result = await _agentLoginService.CreateLoginCredentialAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AgentLoginRequestDto dto)
        {
            var result = await _agentLoginService.LoginAsync(dto);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        [HttpGet("account-setup-options")]
        public async Task<IActionResult> GetAccountSetupOptions()
        {
            var (data, isSuccess, message) = await _accountSetupOptionService.GetActiveOptionsAsync();
            return isSuccess ? Ok(new { isSuccess, message, data }) : BadRequest(new { isSuccess, message, data });
        }

        [HttpPost("check-professional-applicant")]
        public async Task<IActionResult> CheckProfessionalApplicant([FromBody] CheckProfessionalApplicantRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { isSuccess = false, message = string.Join(" | ", errors) });
            }

            var (isSuccess, message) = await _professionalApplicantService.CheckProfessionalApplicantAsync(dto);
            return isSuccess ? Ok(new { isSuccess, message }) : BadRequest(new { isSuccess, message });
        }

        [HttpPost("verify-professional-otp")]
        public async Task<IActionResult> VerifyProfessionalOtp([FromBody] VerifyProfessionalOtpRequestDto dto)
        {
            var (isSuccess, message) = await _professionalApplicantService.VerifyOtpAsync(dto);
            return isSuccess ? Ok(new { isSuccess, message }) : BadRequest(new { isSuccess, message });
        }

        [HttpPost("create-professional-credential")]
        public async Task<IActionResult> CreateProfessionalCredential([FromBody] CreateProfessionalCredentialRequestDto dto)
        {
            var (isSuccess, message) = await _professionalApplicantService.CreateCredentialAsync(dto);
            return isSuccess ? Ok(new { isSuccess, message }) : BadRequest(new { isSuccess, message });
        }
    }
}
