using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos.AgentLogin
{
    public class RequestUserIdOtpDto
    {
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }
    public class VerifyUserIdOtpDto
    {
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string OtpType { get; set; }   // "EMAIL" or "MOBILE"
        public string OtpCode { get; set; }
    }
    public class GetUserIdDto
    {
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }
}
