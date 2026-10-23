using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Domain.Entities.Common
{
    public class EmailTemplate
    {
        public long TemplateId { get; set; }
        public string TemplateCode { get; set; }   // e.g. "OTP_EMAIL"
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string BodyHtml { get; set; }       // raw HTML with {{PLACEHOLDER}} tokens
        public string IsActive { get; set; }
    }
}
