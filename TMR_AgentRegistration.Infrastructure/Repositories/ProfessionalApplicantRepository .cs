using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Domain.IRepositories;
using TMR_AgentRegistration.Infrastructure.Common;

namespace TMR_AgentRegistration.Infrastructure.Repositories
{
    public class ProfessionalApplicantRepository: IProfessionalApplicantRepository
    {
        private readonly IOracleConnectionFactory _connectionFactory;

        public ProfessionalApplicantRepository(IOracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<(bool IsSuccess, string Message, string FullName, string Email, string Mobile, string EmailOtp, string MobileOtp)>
           CheckProfessionalApplicantAsync(string qualificationBasis, string enrolmentNo, string fullName, string email, string mobile)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SP_CHECK_PROFESSIONAL_APPLICANT";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_qualification_basis", OracleDbType.Varchar2, 30, qualificationBasis, ParameterDirection.Input);
            command.Parameters.Add("p_enrolment_no", OracleDbType.Varchar2, 100, enrolmentNo, ParameterDirection.Input);
            command.Parameters.Add("p_full_name", OracleDbType.Varchar2, 150, fullName, ParameterDirection.Input);
            command.Parameters.Add("p_email", OracleDbType.Varchar2, 150, email, ParameterDirection.Input);
            command.Parameters.Add("p_mobile", OracleDbType.Varchar2, 10, mobile, ParameterDirection.Input);

            // NEW: OUT parameters to receive the data back from the SP
            command.Parameters.Add("p_out_full_name", OracleDbType.Varchar2, 150, null, ParameterDirection.Output);
            command.Parameters.Add("p_out_email", OracleDbType.Varchar2, 150, null, ParameterDirection.Output);
            command.Parameters.Add("p_out_mobile", OracleDbType.Varchar2, 10, null, ParameterDirection.Output);
            command.Parameters.Add("p_out_email_otp", OracleDbType.Varchar2, 6, null, ParameterDirection.Output);
            command.Parameters.Add("p_out_mobile_otp", OracleDbType.Varchar2, 6, null, ParameterDirection.Output);

            command.Parameters.Add("p_status", OracleDbType.Int32, ParameterDirection.Output);
            command.Parameters.Add("p_message", OracleDbType.Varchar2, 500, null, ParameterDirection.Output);

            await command.ExecuteNonQueryAsync();

            int status = Convert.ToInt32(((OracleDecimal)command.Parameters["p_status"].Value).Value);
            string message = ((OracleString)command.Parameters["p_message"].Value).Value;

            // If the procedure returned early (already registered), OUT fields will be null - guard with IsNull check
            string outFullName = ((OracleString)command.Parameters["p_out_full_name"].Value).IsNull ? null : ((OracleString)command.Parameters["p_out_full_name"].Value).Value;
            string outEmail = ((OracleString)command.Parameters["p_out_email"].Value).IsNull ? null : ((OracleString)command.Parameters["p_out_email"].Value).Value;
            string outMobile = ((OracleString)command.Parameters["p_out_mobile"].Value).IsNull ? null : ((OracleString)command.Parameters["p_out_mobile"].Value).Value;
            string outEmailOtp = ((OracleString)command.Parameters["p_out_email_otp"].Value).IsNull ? null : ((OracleString)command.Parameters["p_out_email_otp"].Value).Value;
            string outMobileOtp = ((OracleString)command.Parameters["p_out_mobile_otp"].Value).IsNull ? null : ((OracleString)command.Parameters["p_out_mobile_otp"].Value).Value;

            return (status == 1, message, outFullName, outEmail, outMobile, outEmailOtp, outMobileOtp);
        }
        public async Task<(bool IsSuccess, string Message)> VerifyOtpAsync(string email, string otpType, string otpCode)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SP_VERIFY_PROFESSIONAL_OTP";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_email", OracleDbType.Varchar2, 150, email, ParameterDirection.Input);
            command.Parameters.Add("p_otp_type", OracleDbType.Varchar2, 10, otpType, ParameterDirection.Input);
            command.Parameters.Add("p_otp_code", OracleDbType.Varchar2, 6, otpCode, ParameterDirection.Input);

            command.Parameters.Add("p_status", OracleDbType.Int32, ParameterDirection.Output);
            command.Parameters.Add("p_message", OracleDbType.Varchar2, 500, null, ParameterDirection.Output);

            await command.ExecuteNonQueryAsync();

            int status = Convert.ToInt32(((OracleDecimal)command.Parameters["p_status"].Value).Value);
            string message = ((OracleString)command.Parameters["p_message"].Value).Value;

            return (status == 1, message);
        }

        public async Task<(bool IsSuccess, string Message)> CreateCredentialAsync(string email, string userId, string password)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SP_CREATE_PROFESSIONAL_CREDENTIAL";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_email", OracleDbType.Varchar2, 150, email, ParameterDirection.Input);
            command.Parameters.Add("p_user_id", OracleDbType.Varchar2, 100, userId, ParameterDirection.Input);
            command.Parameters.Add("p_password", OracleDbType.Varchar2, 200, password, ParameterDirection.Input);

            command.Parameters.Add("p_status", OracleDbType.Int32, ParameterDirection.Output);
            command.Parameters.Add("p_message", OracleDbType.Varchar2, 500, null, ParameterDirection.Output);

            await command.ExecuteNonQueryAsync();

            int status = Convert.ToInt32(((OracleDecimal)command.Parameters["p_status"].Value).Value);
            string message = ((OracleString)command.Parameters["p_message"].Value).Value;

            return (status == 1, message);
        }
    }
}
