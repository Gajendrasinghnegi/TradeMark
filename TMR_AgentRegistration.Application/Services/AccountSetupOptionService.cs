using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;
using TMR_AgentRegistration.Application.IServices;
using TMR_AgentRegistration.Domain.IRepositories;

namespace TMR_AgentRegistration.Application.Services
{
    public class AccountSetupOptionService: IAccountSetupOptionService
    {
        private readonly IAccountSetupOptionRepository _repository;

        public AccountSetupOptionService(IAccountSetupOptionRepository repository)
        {
            _repository = repository;
        }
        public async Task<(List<AccountSetupOptionDto> Data, bool IsSuccess, string Message)> GetActiveOptionsAsync()
        {
            var (entities, isSuccess, message) = await _repository.GetActiveOptionsAsync();

            var dtos = entities.Select(e => new AccountSetupOptionDto
            {
                SetupOptionId = e.SetupOptionId,
                Title = e.Title,
                DisplayOrder = e.DisplayOrder
            }).ToList();

            return (dtos, isSuccess, message);
        }

    }
}
