using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos.AgentLogin
{
    public class ForgotPassword
    {
        public class RequestResetOtpDto
        {
            public string UserId { get; set; }
        }

        public class VerifyResetOtpDto
        {
            public string UserId { get; set; }
            public string OtpCode { get; set; }
        }

        public class ResetPasswordDto
        {
            public string UserId { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
