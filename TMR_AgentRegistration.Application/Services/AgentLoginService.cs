using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;
using TMR_AgentRegistration.Application.Dtos.AgentLogin;
using TMR_AgentRegistration.Application.Extensions;
using TMR_AgentRegistration.Application.IServices;
using TMR_AgentRegistration.Domain.Entities.Common;
using TMR_AgentRegistration.Domain.IRepositories;

namespace TMR_AgentRegistration.Application.Services
{
    public  class AgentLoginService: IAgentLoginService
    {
        private readonly IAgentLoginRepository _agentLoginRepository;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;
        private readonly ISmsService _smsService;

        public AgentLoginService(IAgentLoginRepository agentLoginRepository, IEmailService emailService, IOptions<JwtSettings> jwtSettings, ISmsService smsService)
        {
            _agentLoginRepository = agentLoginRepository;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _smsService = smsService;
        }
        //  send OTP to email
        public async Task<RepositoryResult<string>> CheckQualifiedAgentAsync(CheckQualifiedAgentRequestDto dto)
        {
            var repoResult = await _agentLoginRepository.CheckQualifiedAgentAsync(dto.RollNumber, dto.DateOfBirth);

            if (!repoResult.IsSuccess)
            {
                return new RepositoryResult<string> { IsSuccess = false, Message = repoResult.Message };
            }
            var emailRequest = new EmailRequestDto
            {
                TemplateCode = "OTP_EMAIL",
                ToEmail = repoResult.Id.Email,
                ToName = repoResult.Id.FullName,
                Placeholders = new Dictionary<string, string>
                {
                    { "USER_NAME", repoResult.Id.FullName },
                    { "OTP_CODE", repoResult.Id.EmailOtp }
                }
            };
            bool emailSent = await _emailService.SendEmailAsync(emailRequest);
            //if (!emailSent)
            if (emailSent)
            {
                return new RepositoryResult<string>
                {
                    IsSuccess = false,
                    Message = "OTP generated successfully, but email could not be sent."
                };
            }

            return new RepositoryResult<string>
            {
                IsSuccess = true,
                Message = "OTP sent successfully to your email."
            };
        
            // NOTE: Mobile OTP (SMS) sending is not implemented yet -
            //var smsRequest = new SmsRequestDto
            //{
            //    MobileNumber = repoResult.Id.MobileNumber,
            //    Message = $"Your OTP for Trademark Agent Registration is {repoResult.Id.MobileOtp}. Valid for 10 minutes."
            //};

            //return new RepositoryResult<string> { IsSuccess = true, Message = "OTP sent to your email. Mobile OTP pending SMS integration." };
        }
        // Verify OTP (called once for EMAIL, once for MOBILE)
        public async Task<RepositoryResult<string>> VerifyOtpAsync(VerifyOtpRequestDto dto)
        {
            var repoResult = await _agentLoginRepository.VerifyOtpAsync(dto.RollNumber, dto.OtpType, dto.OtpCode);
            return new RepositoryResult<string>
            {
                IsSuccess = repoResult.IsSuccess,
                Message = repoResult.Message
            };
        }
      
        // Create login credential - password hashed here, 
        public async Task<RepositoryResult<string>> CreateLoginCredentialAsync(CreateLoginCredentialRequestDto dto)
        {
            // BCrypt.HashPassword automatically generates 
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var repoResult = await _agentLoginRepository.CreateLoginCredentialAsync(dto.RollNumber, dto.UserId, passwordHash);
            return new RepositoryResult<string>
            {
                IsSuccess = repoResult.IsSuccess,
                Message = repoResult.Message
            };
        }
        //  verify password hash, generate JWT, update last login
        public async Task<RepositoryResult<AgentLoginResponseDto>> LoginAsync(AgentLoginRequestDto dto)
         {
            var repoResult = await _agentLoginRepository.AgentLoginAsync(dto.UserId);
            if (!repoResult.IsSuccess)
            {
                return new RepositoryResult<AgentLoginResponseDto> { IsSuccess = false, Message = repoResult.Message };
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, repoResult.Id.StoredPasswordHash);
            if (!isPasswordValid)
            {
                return new RepositoryResult<AgentLoginResponseDto> { IsSuccess = false, Message = "Invalid UserID or Password." };
            }

            // Password correct - now safe to update last login timestamp
            await _agentLoginRepository.UpdateLastLoginAsync(dto.UserId);
            // Generate JWT token
            var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
            var token = GenerateJwtToken(dto.UserId, repoResult.Id.RollNumber, expiry);

            var response = new AgentLoginResponseDto
            {
                Token = token,
                FullName = repoResult.Id.FullName,
                Email = repoResult.Id.Email,
                ExpiresAt = expiry
            };

            return new RepositoryResult<AgentLoginResponseDto> { IsSuccess = true, Message = "Login successful.", Id = response };
        }        
        // PRIVATE HELPER: builds and signs the JWT token
        private string GenerateJwtToken(string userId, string rollNumber, DateTime expiry)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim("RollNumber", rollNumber),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
