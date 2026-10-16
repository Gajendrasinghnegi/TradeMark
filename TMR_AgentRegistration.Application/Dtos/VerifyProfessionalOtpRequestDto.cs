using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos
{
    public class VerifyProfessionalOtpRequestDto
    {
      
            public string Email { get; set; }
            public string OtpType { get; set; }
            public string OtpCode { get; set; }
       

       
    }
}
