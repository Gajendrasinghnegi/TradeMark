using Microsoft.Data.SqlClient;
using System.Data;
using TMR_AgentRegistration.Domain.Entities;
using TMR_AgentRegistration.Domain.Entities.Common;
using TMR_AgentRegistration.Domain.IRepositories;
using TMR_AgentRegistration.Infrastructure.Common;

namespace TMR_AgentRegistration.Infrastructure.Repositories
{
    public class ForgotUserIdRepository : IForgotUserIdRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public ForgotUserIdRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<RepositoryResult<ForgotPasswordResult>> RequestUserIdOtpAsync(string email, string mobileNumber)
                => await ExecuteMasterSpAsync("REQUEST_USERID_OTP", email: email, mobileNumber: mobileNumber);

        public async Task<RepositoryResult<ForgotPasswordResult>> VerifyUserIdOtpAsync(string email, string mobileNumber, string otpType, string otpCode)
            => await ExecuteMasterSpAsync("VERIFY_USERID_OTP", email: email, mobileNumber: mobileNumber, otpType: otpType, otpCode: otpCode);

        public async Task<RepositoryResult<ForgotPasswordResult>> GetUserIdAfterVerificationAsync(string email, string mobileNumber)
            => await ExecuteMasterSpAsync("GET_USERID_AFTER_VERIFICATION", email: email, mobileNumber: mobileNumber);

        private async Task<RepositoryResult<ForgotPasswordResult>> ExecuteMasterSpAsync(string query, string email = null, string mobileNumber = null, string otpType = null, string otpCode = null)
        {
            var result = new RepositoryResult<ForgotPasswordResult>();
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "USP_FORGOT_USERID_MASTER";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@p_query", SqlDbType.VarChar) { Value = query });
                command.Parameters.Add(new SqlParameter("@p_email", SqlDbType.VarChar) { Value = (object)email ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_mobile_number", SqlDbType.VarChar) { Value = (object)mobileNumber ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_otp_type", SqlDbType.VarChar) { Value = (object)otpType ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_otp_code", SqlDbType.VarChar) { Value = (object)otpCode ?? DBNull.Value });

                command.Parameters.Add(new SqlParameter("@p_email_otp_out", SqlDbType.VarChar, 6) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_mobile_otp_out", SqlDbType.VarChar, 6) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_user_id_out", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_status", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_message", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                await command.ExecuteNonQueryAsync();

                string status = command.Parameters["@p_status"].Value?.ToString();
                result.IsSuccess = status == "SUCCESS";
                result.Message = command.Parameters["@p_message"].Value?.ToString();
                result.Id = new ForgotPasswordResult
                {
                    EmailOtp = command.Parameters["@p_email_otp_out"].Value?.ToString(),
                    MobileOtp = command.Parameters["@p_mobile_otp_out"].Value?.ToString(),
                    UserId = command.Parameters["@p_user_id_out"].Value?.ToString()
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