using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
        private readonly IOracleConnectionFactory _connectionFactory;

        public AccountSetupOptionRepository(IOracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(List<AccountSetupOption> Data, bool IsSuccess, string Message)> GetActiveOptionsAsync()
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SP_GET_ACCOUNT_SETUP_OPTIONS";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
            command.Parameters.Add("p_status", OracleDbType.Int32, ParameterDirection.Output);
            command.Parameters.Add("p_message", OracleDbType.Varchar2, 500, null, ParameterDirection.Output);

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

            int status = Convert.ToInt32(((OracleDecimal)command.Parameters["p_status"].Value).Value);
            string message = ((OracleString)command.Parameters["p_message"].Value).Value;

            return (data, status == 1, message);
        }
    }
}