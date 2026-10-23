using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities.Common;

using TMR_AgentRegistration.Domain.IRepositories;
using TMR_AgentRegistration.Infrastructure.Common;

namespace TMR_AgentRegistration.Infrastructure.Repositories
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public EmailTemplateRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<EmailTemplate> GetTemplateByCodeAsync(string templateCode)
        {
            EmailTemplate template = null;
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT TEMPLATE_ID, TEMPLATE_CODE, TEMPLATE_NAME, SUBJECT, BODY_HTML, IS_ACTIVE FROM TMR_ONLINE_EMAIL_TEMPLATE
                                        WHERE TEMPLATE_CODE = @p_template_code AND IS_ACTIVE = 'Y'";
                        command.CommandType = CommandType.Text;
                        var param = new SqlParameter("@p_template_code", SqlDbType.VarChar)
                        {
                            Value = (object)templateCode ?? DBNull.Value
                        };
                        command.Parameters.Add(param);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                template = new EmailTemplate
                                {
                                    TemplateId = reader.GetInt64(0),
                                    TemplateCode = reader.GetString(1),
                                    TemplateName = reader.GetString(2),
                                    Subject = reader.GetString(3),
                                    BodyHtml = reader.GetString(4),
                                    IsActive = reader.GetString(5)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTemplateByCodeAsync: " + ex.Message, ex);
            }
            return template;
        }
    }
}