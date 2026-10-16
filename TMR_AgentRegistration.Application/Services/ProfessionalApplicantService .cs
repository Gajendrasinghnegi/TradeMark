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
    public class ProfessionalApplicantService : IProfessionalApplicantService
    {
        private readonly IProfessionalApplicantRepository _repository;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public ProfessionalApplicantService(
            IProfessionalApplicantRepository repository,
            IEmailService emailService,
            ISmsService smsService)
        {
            _repository = repository;
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task<(bool IsSuccess, string Message)> CheckProfessionalApplicantAsync(CheckProfessionalApplicantRequestDto dto)
        {
            var repoResult = await _repository.CheckProfessionalApplicantAsync(dto.QualificationBasis, dto.EnrolmentNo, dto.FullName, dto.Email, dto.Mobile);

            if (!repoResult.IsSuccess)
            {
                // Covers the "already registered" case returned early by the SP
                return (false, repoResult.Message);
            }

            // Send OTP to email - reuses the same "OTP_EMAIL" template used for the exam flow
            var emailRequest = new EmailRequestDto
            {
                TemplateCode = "OTP_EMAIL",
                ToEmail = repoResult.Email,
                ToName = repoResult.FullName,
                Placeholders = new Dictionary<string, string>
                {
                    { "USER_NAME", repoResult.FullName },
                    { "OTP_CODE", repoResult.EmailOtp }
                }
            };
            bool emailSent = await _emailService.SendEmailAsync(emailRequest);
            // Send OTP to mobile
            var smsRequest = new SmsRequestDto
            {
                MobileNumber = repoResult.Mobile,
                Message = $"Your OTP for Trademark Agent Portal registration is {repoResult.MobileOtp}. Valid for 10 minutes."
            };
            await _smsService.SendSmsAsync(smsRequest);
            if (!emailSent)
            {
                return (false, "OTP generated successfully, but email could not be sent.");
            }

            return (true, "OTP sent to your registered email and mobile.");
        }
        public async Task<(bool IsSuccess, string Message)> VerifyOtpAsync(VerifyProfessionalOtpRequestDto dto)
        {
            var result = await _repository.VerifyOtpAsync(dto.Email, dto.OtpType, dto.OtpCode);
            return (result.IsSuccess, result.Message);
        }

        public async Task<(bool IsSuccess, string Message)> CreateCredentialAsync(CreateProfessionalCredentialRequestDto dto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var result = await _repository.CreateCredentialAsync(dto.Email, dto.UserId, passwordHash);
            return (result.IsSuccess, result.Message);
        }
    }
}
