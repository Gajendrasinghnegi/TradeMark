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
using static TMR_AgentRegistration.Application.Dtos.AgentLogin.ForgotPassword;

namespace TMR_AgentRegistration.Application.Services
{
    public  class AgentLoginService: IAgentLoginService
    {
        private readonly IAgentLoginRepository _agentLoginRepository;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;
        private readonly ISmsService _smsService;
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly IForgotUserIdRepository _forgotUserIdRepository;

        public AgentLoginService(IAgentLoginRepository agentLoginRepository, IEmailService emailService, IOptions<JwtSettings> jwtSettings, ISmsService smsService, IForgotPasswordRepository forgotPasswordRepository, IForgotUserIdRepository forgotUserIdRepository)
        {
            _agentLoginRepository = agentLoginRepository;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _smsService = smsService;
            _forgotPasswordRepository = forgotPasswordRepository;
            _forgotUserIdRepository = forgotUserIdRepository;
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
            if (!emailSent)
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
        // Generates OTP for password reset and sends it to the user's registered email
        public async Task<RepositoryResult<string>> RequestResetOtpAsync(RequestResetOtpDto dto)
        {
            var repoResult = await _forgotPasswordRepository.RequestResetOtpAsync(dto.UserId);
            if (!repoResult.IsSuccess)
            {
                return new RepositoryResult<string> { IsSuccess = false, Message = repoResult.Message };
            }
            // Build email request using the OTP and email returned from the SP
            var emailRequest = new EmailRequestDto
            {
                TemplateCode = "FORGOT_PASSWORD_OTP",
                ToEmail = repoResult.Id.Email,
                ToName = repoResult.Id.Email, // Full name not available here, using email as fallback
                Placeholders = new Dictionary<string, string>
              {{ "OTP_CODE", repoResult.Id.OtpCode }}      
            };

            bool emailSent = await _emailService.SendEmailAsync(emailRequest);
            if (!emailSent)
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
                Message = "OTP sent successfully to your registered email."
            };
        }
        public async Task<RepositoryResult<string>> VerifyResetOtpAsync(VerifyResetOtpDto dto)
        {
            var repoResult = await _forgotPasswordRepository.VerifyResetOtpAsync(dto.UserId, dto.OtpCode);
            return new RepositoryResult<string>
            {
                IsSuccess = repoResult.IsSuccess,
                Message = repoResult.Message
            };
        }

        public async Task<RepositoryResult<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            // Password 
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            var repoResult = await _forgotPasswordRepository.ResetPasswordAsync(dto.UserId, hashedPassword);
            return new RepositoryResult<string>
            {
                IsSuccess = repoResult.IsSuccess,
                Message = repoResult.Message
            };
        }
        // Sends separate OTP to Email and separate OTP to Mobile
        public async Task<RepositoryResult<string>> RequestUserIdOtpAsync(RequestUserIdOtpDto dto)
        {
            var repoResult = await _forgotUserIdRepository.RequestUserIdOtpAsync(dto.Email, dto.MobileNumber);

            if (!repoResult.IsSuccess)
            {
                return new RepositoryResult<string> { IsSuccess = false, Message = repoResult.Message };
            }

            // Send EMAIL OTP
            var emailRequest = new EmailRequestDto
            {
                TemplateCode = "FORGOT_USERID_OTP",
                ToEmail = dto.Email,
                ToName = dto.Email,
                Placeholders = new Dictionary<string, string>
        {
            { "OTP_CODE", repoResult.Id.EmailOtp }
        }
            };
            bool emailSent = await _emailService.SendEmailAsync(emailRequest);

            // Send MOBILE OTP
            var smsRequest = new SmsRequestDto
            {
                MobileNumber = dto.MobileNumber,
                Message = $"Your OTP to recover User ID is {repoResult.Id.MobileOtp}. Valid for 10 minutes."
            };
            bool smsSent = await _smsService.SendSmsAsync(smsRequest);

            if (!emailSent && !smsSent)
            {
                return new RepositoryResult<string> { IsSuccess = false, Message = "OTP generated, but could not be sent via email or SMS." };
            }

            string msg = "OTP sent to your registered ";
            msg += emailSent && smsSent ? "email and mobile number." :
                   emailSent ? "email. SMS delivery failed." :
                   "mobile number. Email delivery failed.";

            return new RepositoryResult<string> { IsSuccess = true, Message = msg };
        }

        // Verifies one OTP at a time - called once for EMAIL, once for MOBILE
        public async Task<RepositoryResult<string>> VerifyUserIdOtpAsync(VerifyUserIdOtpDto dto)
        {
            var repoResult = await _forgotUserIdRepository.VerifyUserIdOtpAsync(dto.Email, dto.MobileNumber, dto.OtpType, dto.OtpCode);
            return new RepositoryResult<string>
            {
                IsSuccess = repoResult.IsSuccess,
                Message = repoResult.Message
            };
        }

        // Called after both EMAIL and MOBILE OTPs are verified - fetches and emails the UserID
        public async Task<RepositoryResult<string>> GetUserIdAfterVerificationAsync(GetUserIdDto dto)
        {
            var repoResult = await _forgotUserIdRepository.GetUserIdAfterVerificationAsync(dto.Email, dto.MobileNumber);

            if (!repoResult.IsSuccess)
            {
                return new RepositoryResult<string> { IsSuccess = false, Message = repoResult.Message };
            }

            var emailRequest = new EmailRequestDto
            {
                TemplateCode = "USERID_RECOVERY_MAIL",
                ToEmail = dto.Email,
                ToName = dto.Email,
                Placeholders = new Dictionary<string, string>
        {
            { "USER_ID", repoResult.Id.UserId }
        }
            };
            bool emailSent = await _emailService.SendEmailAsync(emailRequest);

            return new RepositoryResult<string>
            {
                IsSuccess = emailSent,
                Message = emailSent ? "Your UserID has been sent to your registered email." : "Verified, but email could not be sent."
            };
        }
    }
}
