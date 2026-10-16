using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos
{
    public class CreateProfessionalCredentialRequestDto
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
