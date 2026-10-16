using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Domain.Entities.Common
{
    public class AgentLoginResult
    {

        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string IsCredentialCreated { get; set; }
        public string EmailOtp { get; set; }
        public string MobileOtp { get; set; }
        public string StoredPasswordHash { get; set; }
        public string RollNumber { get; set; }
    }
}
