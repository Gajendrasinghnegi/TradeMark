using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities.Common;

namespace TMR_AgentRegistration.Domain.IRepositories
{
    public interface IAgentLoginRepository
    {
        Task<RepositoryResult<AgentLoginResult>> CheckQualifiedAgentAsync(string rollNumber, string dateOfBirth);

        Task<RepositoryResult<AgentLoginResult>> VerifyOtpAsync(string rollNumber, string otpType, string otpCode);

        Task<RepositoryResult<AgentLoginResult>> CreateLoginCredentialAsync(string rollNumber, string userId, string passwordHash);

        Task<RepositoryResult<AgentLoginResult>> AgentLoginAsync(string userId);

        Task<RepositoryResult<AgentLoginResult>> UpdateLastLoginAsync(string userId);
    }
}
