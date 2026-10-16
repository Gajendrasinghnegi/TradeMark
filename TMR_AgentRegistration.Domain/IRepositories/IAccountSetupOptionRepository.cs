using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities;

namespace TMR_AgentRegistration.Domain.IRepositories
{
    public interface IAccountSetupOptionRepository
    {
        Task<(List<AccountSetupOption> Data, bool IsSuccess, string Message)> GetActiveOptionsAsync();
    }
}

