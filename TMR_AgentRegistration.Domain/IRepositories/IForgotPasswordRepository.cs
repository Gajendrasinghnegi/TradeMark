
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities;
using TMR_AgentRegistration.Domain.Entities.Common;

namespace TMR_AgentRegistration.Domain.IRepositories
{
    public interface IForgotPasswordRepository
    {
        Task<RepositoryResult<ForgotPasswordResult>> RequestResetOtpAsync(string userId);
        Task<RepositoryResult<ForgotPasswordResult>> VerifyResetOtpAsync(string userId, string otpCode);
        Task<RepositoryResult<ForgotPasswordResult>> ResetPasswordAsync(string userId, string newPasswordHash);
    }
}
