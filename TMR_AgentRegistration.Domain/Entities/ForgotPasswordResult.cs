using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Domain.Entities
{
    public class ForgotPasswordResult
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string? EmailOtp { get; set; }
        public string? MobileOtp { get; set; }
        public string? UserId { get; set; }
    }
}
