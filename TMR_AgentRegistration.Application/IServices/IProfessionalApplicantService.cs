using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;

namespace TMR_AgentRegistration.Application.IServices
{
    public interface IProfessionalApplicantService
    {
        Task<(bool IsSuccess, string Message)> CheckProfessionalApplicantAsync(CheckProfessionalApplicantRequestDto dto);
        Task<(bool IsSuccess, string Message)> VerifyOtpAsync(VerifyProfessionalOtpRequestDto dto);
        Task<(bool IsSuccess, string Message)> CreateCredentialAsync(CreateProfessionalCredentialRequestDto dto);
    }
}
    
