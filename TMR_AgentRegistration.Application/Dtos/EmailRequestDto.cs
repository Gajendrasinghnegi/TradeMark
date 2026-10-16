using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos
{
    public class EmailRequestDto
    {
        public string TemplateCode { get; set; }         
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        // Key = placeholder name without braces (e.g. "OTP_CODE"), Value = actual value to replace with
        public Dictionary<string, string> Placeholders { get; set; } = new Dictionary<string, string>();
    }
}
