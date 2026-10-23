using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos.AgentLogin;
using TMR_AgentRegistration.Domain.Entities.Common;
using static TMR_AgentRegistration.Application.Dtos.AgentLogin.ForgotPassword;

namespace TMR_AgentRegistration.Application.IServices
{
    public interface IAgentLoginService
    {
        Task<RepositoryResult<string>> CheckQualifiedAgentAsync(CheckQualifiedAgentRequestDto dto);
        Task<RepositoryResult<string>> VerifyOtpAsync(VerifyOtpRequestDto dto);
        Task<RepositoryResult<string>> CreateLoginCredentialAsync(CreateLoginCredentialRequestDto dto);
        Task<RepositoryResult<AgentLoginResponseDto>> LoginAsync(AgentLoginRequestDto dto);
        //forget
        Task<RepositoryResult<string>> RequestResetOtpAsync(RequestResetOtpDto dto);
        Task<RepositoryResult<string>> VerifyResetOtpAsync(VerifyResetOtpDto dto);
        Task<RepositoryResult<string>> ResetPasswordAsync(ResetPasswordDto dto);
        //userid forget
        Task<RepositoryResult<string>> RequestUserIdOtpAsync(RequestUserIdOtpDto dto);
        Task<RepositoryResult<string>> VerifyUserIdOtpAsync(VerifyUserIdOtpDto dto);
        Task<RepositoryResult<string>> GetUserIdAfterVerificationAsync(GetUserIdDto dto);

    }
}
