using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Domain.IRepositories
{
    public interface IProfessionalApplicantRepository
    {
        Task<(bool IsSuccess, string Message, string FullName, string Email, string Mobile, string EmailOtp, string MobileOtp)>CheckProfessionalApplicantAsync(string qualificationBasis, string enrolmentNo, string fullName, string email, string mobile);
        Task<(bool IsSuccess, string Message)> VerifyOtpAsync(string email, string otpType, string otpCode);
        Task<(bool IsSuccess, string Message)> CreateCredentialAsync(string email, string userId, string password);
    }
}
