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
    public class AgentLoginRepository : IAgentLoginRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public AgentLoginRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<RepositoryResult<AgentLoginResult>> CheckQualifiedAgentAsync(string rollNumber, string dateOfBirth)
        {
            return await ExecuteMasterSpAsync(query: "CHECK_QUALIFIED_AGENT", rollNumber: rollNumber, dateOfBirth: dateOfBirth);
        }
        public async Task<RepositoryResult<AgentLoginResult>> VerifyOtpAsync(string rollNumber, string otpType, string otpCode)
        {
            return await ExecuteMasterSpAsync(query: "VERIFY_OTP", rollNumber: rollNumber, otpType: otpType, otpCode: otpCode);
        }
        public async Task<RepositoryResult<AgentLoginResult>> CreateLoginCredentialAsync(string rollNumber, string userId, string passwordHash)
        {
            return await ExecuteMasterSpAsync(query: "CREATE_LOGIN_CREDENTIAL", rollNumber: rollNumber, userId: userId, passwordHash: passwordHash);
        }
        public async Task<RepositoryResult<AgentLoginResult>> AgentLoginAsync(string userId)
        {
            return await ExecuteMasterSpAsync(query: "AGENT_LOGIN", userId: userId);
        }
        public async Task<RepositoryResult<AgentLoginResult>> UpdateLastLoginAsync(string userId)
        {
            return await ExecuteMasterSpAsync(query: "UPDATE_LAST_LOGIN", userId: userId);
        }

        private async Task<RepositoryResult<AgentLoginResult>> ExecuteMasterSpAsync(string query, string rollNumber = null, string dateOfBirth = null, string otpType = null, string otpCode = null, string userId = null, string passwordHash = null)
        {
            var result = new RepositoryResult<AgentLoginResult>();
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "USP_AGENT_LOGIN_MASTER";
                        command.CommandType = CommandType.StoredProcedure;

                        // ---- IN parameters ----
                        command.Parameters.Add(new SqlParameter("@p_query", SqlDbType.VarChar) { Value = query });
                        command.Parameters.Add(new SqlParameter("@p_roll_number", SqlDbType.VarChar) { Value = (object)rollNumber ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@p_date_of_birth", SqlDbType.VarChar) { Value = (object)dateOfBirth ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@p_otp_type", SqlDbType.VarChar) { Value = (object)otpType ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@p_otp_code", SqlDbType.VarChar) { Value = (object)otpCode ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@p_user_id", SqlDbType.VarChar) { Value = (object)userId ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@p_password_hash", SqlDbType.VarChar) { Value = (object)passwordHash ?? DBNull.Value });

                        // ---- OUT parameters --
                        command.Parameters.Add(new SqlParameter("@p_full_name", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_email", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_mobile_number", SqlDbType.VarChar, 15) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_is_credential_created", SqlDbType.VarChar, 1) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_email_otp", SqlDbType.VarChar, 6) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_mobile_otp", SqlDbType.VarChar, 6) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_stored_password_hash", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_roll_number_out", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_status", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@p_message", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                        await command.ExecuteNonQueryAsync();

                        string status = command.Parameters["@p_status"].Value?.ToString();
                        string message = command.Parameters["@p_message"].Value?.ToString();
                        result.IsSuccess = status == "SUCCESS";
                        result.Message = message;

                        result.Id = new AgentLoginResult
                        {
                            FullName = command.Parameters["@p_full_name"].Value?.ToString(),
                            Email = command.Parameters["@p_email"].Value?.ToString(),
                            MobileNumber = command.Parameters["@p_mobile_number"].Value?.ToString(),
                            IsCredentialCreated = command.Parameters["@p_is_credential_created"].Value?.ToString(),
                            EmailOtp = command.Parameters["@p_email_otp"].Value?.ToString(),
                            MobileOtp = command.Parameters["@p_mobile_otp"].Value?.ToString(),
                            StoredPasswordHash = command.Parameters["@p_stored_password_hash"].Value?.ToString(),
                            RollNumber = command.Parameters["@p_roll_number_out"].Value?.ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Database error: " + ex.Message;
            }

            return result;
        }
    }
}