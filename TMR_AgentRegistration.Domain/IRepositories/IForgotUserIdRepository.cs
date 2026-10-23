
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities;
using TMR_AgentRegistration.Domain.Entities.Common;

namespace TMR_AgentRegistration.Domain.IRepositories
{
    public  interface IForgotUserIdRepository
    {
        Task<RepositoryResult<ForgotPasswordResult>> RequestUserIdOtpAsync(string email, string mobileNumber);
        Task<RepositoryResult<ForgotPasswordResult>> VerifyUserIdOtpAsync(string email, string mobileNumber, string otpType, string otpCode);
        Task<RepositoryResult<ForgotPasswordResult>> GetUserIdAfterVerificationAsync(string email, string mobileNumber);
    }
}
