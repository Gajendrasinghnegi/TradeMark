using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities;
using TMR_AgentRegistration.Domain.IRepositories;
using TMR_AgentRegistration.Infrastructure.Common;

namespace TMR_AgentRegistration.Infrastructure.Repositories
{
    public class AccountSetupOptionRepository : IAccountSetupOptionRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public AccountSetupOptionRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<(List<AccountSetupOption> Data, bool IsSuccess, string Message)> GetActiveOptionsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SP_GET_ACCOUNT_SETUP_OPTIONS";
            command.CommandType = CommandType.StoredProcedure;

            var statusParam = new SqlParameter("@p_status", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            var messageParam = new SqlParameter("@p_message", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);

            var data = new List<AccountSetupOption>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    data.Add(new AccountSetupOption
                    {
                        SetupOptionId = Convert.ToInt32(reader["SetupOptionId"]),
                        Title = reader["Title"].ToString(),
                        DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]),
                        IsActive = Convert.ToInt32(reader["IsActive"]) == 1,
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }
            int status = statusParam.Value != DBNull.Value ? Convert.ToInt32(statusParam.Value) : 0;
            string message = messageParam.Value != DBNull.Value ? messageParam.Value.ToString() : string.Empty;
            return (data, status == 1, message);
        }
    }
}