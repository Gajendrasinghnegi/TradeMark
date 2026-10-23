using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.IRepositories;
using TMR_AgentRegistration.Infrastructure.Common;

namespace TMR_AgentRegistration.Infrastructure.Repositories
{
    public class ProfessionalApplicantRepository : IProfessionalApplicantRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ProfessionalApplicantRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(bool IsSuccess, string Message, string FullName, string Email, string Mobile, string EmailOtp, string MobileOtp)>CheckProfessionalApplicantAsync(string qualificationBasis, string enrolmentNo, string fullName, string email, string mobile)
        {
            var result = await ExecuteMasterSpAsync(query: "CHECK_APPLICANT", qualificationBasis: qualificationBasis, enrolmentNo: enrolmentNo, fullName: fullName, email: email, mobile: mobile);
            return (result.IsSuccess, result.Message, result.OutFullName, result.OutEmail, result.OutMobile, result.OutEmailOtp, result.OutMobileOtp);
        }

        public async Task<(bool IsSuccess, string Message)> VerifyOtpAsync(string email, string otpType, string otpCode)
        {
            var result = await ExecuteMasterSpAsync(query: "VERIFY_OTP", email: email, otpType: otpType, otpCode: otpCode);
            return (result.IsSuccess, result.Message);
        }

        public async Task<(bool IsSuccess, string Message)> CreateCredentialAsync(string email, string userId, string passwordHash)
        {
            var result = await ExecuteMasterSpAsync(query: "CREATE_CREDENTIAL", email: email, userId: userId, passwordHash: passwordHash);
            return (result.IsSuccess, result.Message);
        }

        private async Task<(bool IsSuccess, string Message, string OutFullName, string OutEmail, string OutMobile, string OutEmailOtp, string OutMobileOtp)> ExecuteMasterSpAsync(string query, string qualificationBasis = null, string enrolmentNo = null, string fullName = null, string email = null, string mobile = null, string otpType = null, string otpCode = null, string userId = null, string passwordHash = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "USP_PROFESSIONAL_APPLICANT_MASTER";
                command.CommandType = CommandType.StoredProcedure;

                // ---- IN parameters ----
                command.Parameters.Add(new SqlParameter("@p_query", SqlDbType.VarChar) { Value = query });
                command.Parameters.Add(new SqlParameter("@p_qualification_basis", SqlDbType.VarChar) { Value = (object)qualificationBasis ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_enrolment_no", SqlDbType.VarChar) { Value = (object)enrolmentNo ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_full_name", SqlDbType.VarChar) { Value = (object)fullName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_email", SqlDbType.VarChar) { Value = (object)email ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_mobile", SqlDbType.VarChar) { Value = (object)mobile ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_otp_type", SqlDbType.VarChar) { Value = (object)otpType ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_otp_code", SqlDbType.VarChar) { Value = (object)otpCode ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_user_id", SqlDbType.VarChar) { Value = (object)userId ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@p_password_hash", SqlDbType.VarChar) { Value = (object)passwordHash ?? DBNull.Value });

                // ---- OUT parameters ----
                command.Parameters.Add(new SqlParameter("@p_out_full_name", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_out_email", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_out_mobile", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_out_email_otp", SqlDbType.VarChar, 6) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_out_mobile_otp", SqlDbType.VarChar, 6) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_status", SqlDbType.Int) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@p_message", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                await command.ExecuteNonQueryAsync();

                int status = Convert.ToInt32(command.Parameters["@p_status"].Value?.ToString() ?? "0");
                string message = command.Parameters["@p_message"].Value?.ToString();

                string outFullName = command.Parameters["@p_out_full_name"].Value?.ToString();
                string outEmail = command.Parameters["@p_out_email"].Value?.ToString();
                string outMobile = command.Parameters["@p_out_mobile"].Value?.ToString();
                string outEmailOtp = command.Parameters["@p_out_email_otp"].Value?.ToString();
                string outMobileOtp = command.Parameters["@p_out_mobile_otp"].Value?.ToString();

                return (status == 1, message, outFullName, outEmail, outMobile, outEmailOtp, outMobileOtp);
            }
            catch (Exception ex)
            {
                return (false, "Database error: " + ex.Message, null, null, null, null, null);
            }
        }
    }
}