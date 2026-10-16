using Oracle.ManagedDataAccess.Client;
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
        private readonly IOracleConnectionFactory _connectionFactory;

        public AgentLoginRepository(IOracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        // Check Roll Number + DOB, triggers OTP generation in the SP
        public async Task<RepositoryResult<AgentLoginResult>> CheckQualifiedAgentAsync(string rollNumber, string dateOfBirth)
        {
            return await ExecuteMasterSpAsync( query: "CHECK_QUALIFIED_AGENT", rollNumber: rollNumber, dateOfBirth: dateOfBirth);
        }

        //  Verify OTP - called once for EMAIL, once for MOBILE
        public async Task<RepositoryResult<AgentLoginResult>> VerifyOtpAsync(string rollNumber, string otpType, string otpCode)
        {
            return await ExecuteMasterSpAsync(query: "VERIFY_OTP",  rollNumber: rollNumber,  otpType: otpType, otpCode: otpCode);
        }
        // Create login credential (UserID + hashed Password)
        public async Task<RepositoryResult<AgentLoginResult>> CreateLoginCredentialAsync(string rollNumber, string userId, string passwordHash)
        {
            return await ExecuteMasterSpAsync(query: "CREATE_LOGIN_CREDENTIAL",  rollNumber: rollNumber, userId: userId,passwordHash: passwordHash);
        }
       
        //Fetch stored password hash by UserID (C# will verify with BCrypt)
        public async Task<RepositoryResult<AgentLoginResult>> AgentLoginAsync(string userId)
        {
            return await ExecuteMasterSpAsync( query: "AGENT_LOGIN", userId: userId);
        }
        //Update last login timestamp (called only after password verified)
       
        public async Task<RepositoryResult<AgentLoginResult>> UpdateLastLoginAsync(string userId)
        {
            return await ExecuteMasterSpAsync( query: "UPDATE_LAST_LOGIN",userId: userId);
        }
      
        private async Task<RepositoryResult<AgentLoginResult>> ExecuteMasterSpAsync( string query, string rollNumber = null, string dateOfBirth = null, string otpType = null,  string otpCode = null, string userId = null,string passwordHash = null)
        {
            var result = new RepositoryResult<AgentLoginResult>();
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SP_AGENT_LOGIN_MASTER";
                        command.CommandType = CommandType.StoredProcedure;
                        command.BindByName = true;

                        // ---- IN parameters ----
                        command.Parameters.Add(new OracleParameter("p_query", OracleDbType.Varchar2) { Value = query });
                        command.Parameters.Add(new OracleParameter("p_roll_number", OracleDbType.Varchar2) { Value = (object)rollNumber ?? DBNull.Value });
                        command.Parameters.Add(new OracleParameter("p_date_of_birth", OracleDbType.Varchar2){Value = (object)dateOfBirth ?? DBNull.Value});
                        command.Parameters.Add(new OracleParameter("p_otp_type", OracleDbType.Varchar2) { Value = (object)otpType ?? DBNull.Value });
                        command.Parameters.Add(new OracleParameter("p_otp_code", OracleDbType.Varchar2) { Value = (object)otpCode ?? DBNull.Value });
                        command.Parameters.Add(new OracleParameter("p_user_id", OracleDbType.Varchar2) { Value = (object)userId ?? DBNull.Value });
                        command.Parameters.Add(new OracleParameter("p_password_hash", OracleDbType.Varchar2) { Value = (object)passwordHash ?? DBNull.Value });

                        // ---- OUT parameters --
                        command.Parameters.Add(new OracleParameter("p_full_name", OracleDbType.Varchar2, 150) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_email", OracleDbType.Varchar2, 150) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_mobile_number", OracleDbType.Varchar2, 15) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_is_credential_created", OracleDbType.Varchar2, 1) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_email_otp", OracleDbType.Varchar2, 6) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_mobile_otp", OracleDbType.Varchar2, 6) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_stored_password_hash", OracleDbType.Varchar2, 255) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_roll_number_out", OracleDbType.Varchar2, 50) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_status", OracleDbType.Varchar2, 20) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new OracleParameter("p_message", OracleDbType.Varchar2, 255) { Direction = ParameterDirection.Output });

                        await command.ExecuteNonQueryAsync();

                        // Read back the OUT parameter values after execution
                        string status = command.Parameters["p_status"].Value?.ToString();
                        string message = command.Parameters["p_message"].Value?.ToString();
                        result.IsSuccess = status == "SUCCESS";
                        result.Message = message;

                        result.Id = new AgentLoginResult
                        {
                            FullName = command.Parameters["p_full_name"].Value?.ToString(),
                            Email = command.Parameters["p_email"].Value?.ToString(),
                            MobileNumber = command.Parameters["p_mobile_number"].Value?.ToString(),
                            IsCredentialCreated = command.Parameters["p_is_credential_created"].Value?.ToString(),
                            EmailOtp = command.Parameters["p_email_otp"].Value?.ToString(),
                            MobileOtp = command.Parameters["p_mobile_otp"].Value?.ToString(),
                            StoredPasswordHash = command.Parameters["p_stored_password_hash"].Value?.ToString(),
                            RollNumber = command.Parameters["p_roll_number_out"].Value?.ToString()
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