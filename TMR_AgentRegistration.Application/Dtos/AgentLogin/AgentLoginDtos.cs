using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos.AgentLogin
{
    public class CheckQualifiedAgentRequestDto
    {
        public string RollNumber { get; set; }
        public string DateOfBirth { get; set; }
    }
    public class VerifyOtpRequestDto
    {
        public string RollNumber { get; set; }
        public string OtpType { get; set; }   // "EMAIL" or "MOBILE"
        public string OtpCode { get; set; }
    }
    public class CreateLoginCredentialRequestDto
    {
        public string RollNumber { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }  
    }
    public class AgentLoginRequestDto
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    // Returned only after successful login(JWT token)
    public class AgentLoginResponseDto
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

