using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities;
using TMR_AgentRegistration.Domain.Entities.Common;

namespace TMR_AgentRegistration.Domain.IRepositories
{
    public interface IAgentApplicationRepository
    {
        Task<RepositoryResult<int>> SaveApplicationAsync(AgentApplication app);
        Task<RepositoryResult<AgentApplication?>> GetApplicationAsync(int applicationNumber);
        Task<RepositoryResult<AgentApplication?>> GetApplicationByPartyAsync(string partyCode);
    }
}
