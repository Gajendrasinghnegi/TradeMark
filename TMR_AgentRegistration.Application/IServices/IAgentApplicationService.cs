using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;

namespace TMR_AgentRegistration.Application.IServices
{
    public interface IAgentApplicationService
    {
        Task<ApiResponse<int>> SaveApplicationAsync(AgentApplicationRequest request);
        Task<ApiResponse<AgentApplicationRequest?>> GetApplicationAsync(int applicationNumber);
        Task<ApiResponse<AgentApplicationRequest?>> GetApplicationByPartyAsync(string partyCode);
    }
}
