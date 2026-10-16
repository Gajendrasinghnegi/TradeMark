using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos
{
    public class SmsRequestDto
    {
        public string MobileNumber { get; set; }
        public string Message { get; set; }
    }
}
