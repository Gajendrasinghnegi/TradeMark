using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos.AgentLogin;
using TMR_AgentRegistration.Domain.Entities.Common;

namespace TMR_AgentRegistration.Application.IServices
{
    public interface IAgentLoginService
    {
        Task<RepositoryResult<string>> CheckQualifiedAgentAsync(CheckQualifiedAgentRequestDto dto);

        Task<RepositoryResult<string>> VerifyOtpAsync(VerifyOtpRequestDto dto);

        Task<RepositoryResult<string>> CreateLoginCredentialAsync(CreateLoginCredentialRequestDto dto);

        Task<RepositoryResult<AgentLoginResponseDto>> LoginAsync(AgentLoginRequestDto dto);
    }
}
