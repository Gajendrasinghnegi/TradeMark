using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos
{
    public   class AccountSetupOptionDto
    {
        public int SetupOptionId { get; set; }
        public string Title { get; set; }
        public int DisplayOrder { get; set; }
    }
}
