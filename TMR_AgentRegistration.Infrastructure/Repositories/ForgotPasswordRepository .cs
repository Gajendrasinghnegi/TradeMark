using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.Entities;
using TMR_AgentRegistration.Domain.Entities.Common;
using TMR_AgentRegistration.Domain.IRepositories;
using TMR_AgentRegistration.Infrastructure.Common;

namespace TMR_AgentRegistration.Infrastructure.Repositories
{
    public class ForgotPasswordRepository : IForgotPasswordRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public ForgotPasswordRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<RepositoryResult<ForgotPasswordResult>> RequestResetOtpAsync(string userId)
             => await ExecuteMasterSpAsync("REQUEST_RESET_OTP", userId: userId);

        public async Task<RepositoryResult<ForgotPasswordResult>> VerifyResetOtpAsync(string userId, string otpCode)
            => await ExecuteMasterSpAsync("VERIFY_RESET_OTP", userId: userId, otpCode: otpCode);

        public async Task<RepositoryResult<ForgotPasswordResult>> ResetPasswordAsync(string userId, string newPasswordHash)
            => await ExecuteMasterSpAsync("RESET_PASSWORD", userId: userId, newPasswordHash: newPasswordHash);

        private async Task<RepositoryResult<ForgotPasswordResult>> ExecuteMasterSpAsync( string query, string userId = null, string otpCode = null, string newPasswordHash = null)
        {
            var result = new RepositoryResult<ForgotPasswordResult>();
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "USP_FORGOT_PASSWORD_MASTER";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@p_query", SqlDbType.VarChar) { Value = query });
                command.Parameters.Add(new SqlParameter("@p_user_id", SqlDbType.VarChar) { Value = (object)userId ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_otp_code", SqlDbType.VarChar) { Value = (object)otpCode ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_new_password_hash", SqlDbType.VarChar) { Value = (object)newPasswordHash ?? DBNull.Value });

                command.Parameters.Add(new SqlParameter("@p_email_out", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_otp_out", SqlDbType.VarChar, 6) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_status", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_message", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                await command.ExecuteNonQueryAsync();

                string status = command.Parameters["@p_status"].Value?.ToString();
                result.IsSuccess = status == "SUCCESS";
                result.Message = command.Parameters["@p_message"].Value?.ToString();
                result.Id = new ForgotPasswordResult
                {
                    Email = command.Parameters["@p_email_out"].Value?.ToString(),
                    OtpCode = command.Parameters["@p_otp_out"].Value?.ToString()
                };
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