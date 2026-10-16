using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities.Common;

namespace TMR_AgentRegistration.Domain.IRepositories
{
    public interface IEmailTemplateRepository
    {
        Task<EmailTemplate> GetTemplateByCodeAsync(string templateCode);
    }
}
