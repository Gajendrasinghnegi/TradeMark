using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.IServices;
using TMR_AgentRegistration.Application.Services;
using TMR_AgentRegistration.Domain.IRepositories;

namespace TMR_AgentRegistration.Application.Extensions
{
    public static class DependencyInjectionService
    {
        // Added 'IConfiguration configuration' parameter - needed to read appsettings.json sections
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAgentApplicationService, AgentApplicationService>();
            services.Configure<MailServiceSettings>(configuration.GetSection("MailService"));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<SmsServiceSettings>(configuration.GetSection("SmsService"));

            services.AddScoped<IAgentLoginService, AgentLoginService>();
            services.AddHttpClient<ISmsService, SmsService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccountSetupOptionService, AccountSetupOptionService>();
            services.AddScoped<IProfessionalApplicantService, ProfessionalApplicantService>();

            return services;
        }
    }
}