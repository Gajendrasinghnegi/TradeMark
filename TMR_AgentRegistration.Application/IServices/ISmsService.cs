using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;

namespace TMR_AgentRegistration.Application.IServices
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(SmsRequestDto request);
    }
}
