using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Extensions;
using TMR_AgentRegistration.Application.IServices;
using TMR_AgentRegistration.Application.Services;
using TMR_AgentRegistration.Domain.IRepositories;
using TMR_AgentRegistration.Infrastructure.Common;
using TMR_AgentRegistration.Infrastructure.Repositories;

namespace TMR_AgentRegistration.Infrastructure.Extensions
{
    public static class DependencyInjectionRepositories
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IAgentApplicationRepository, AgentApplicationRepository>();
            services.AddScoped<IAgentLoginRepository, AgentLoginRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IAccountSetupOptionRepository, AccountSetupOptionRepository>();
            services.AddScoped<IProfessionalApplicantRepository, ProfessionalApplicantRepository>();
            services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
            services.AddScoped<IForgotUserIdRepository, ForgotUserIdRepository>();

            return services;
        }
    }
}
