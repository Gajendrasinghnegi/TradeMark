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
    public class AgentApplicationRepository : IAgentApplicationRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public AgentApplicationRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        private static void AddInput(SqlCommand command, string name, object? value)
        {
            command.Parameters.Add(new SqlParameter(name, value ?? DBNull.Value));
        }

        public async Task<RepositoryResult<int>> SaveApplicationAsync(AgentApplication app)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = connection.CreateCommand();
                command.CommandText = "SP_AGENT_APPLICATION_SAVE";
                command.CommandType = CommandType.StoredProcedure;

                // All parameters -----
                AddInput(command, "@P_APPLICATION_TYPE", app.ApplicationType);
                AddInput(command, "@P_FORM_NUMBER", app.FormNumber);
                AddInput(command, "@P_PARTY_TYPE", app.PartyType);
                AddInput(command, "@P_PARTY_CODE", app.PartyCode);
                AddInput(command, "@P_BRANCH_CODE", app.BranchCode);
                AddInput(command, "@P_FORM_AMOUNT", app.FormAmount);
                AddInput(command, "@P_LAST_UPDATED_BY", app.LastUpdatedBy);
                AddInput(command, "@P_HTML_PREVIEW", app.HtmlPreview);
                AddInput(command, "@P_ENTRY_NO", app.EntryNo);
                AddInput(command, "@P_NAME", app.Name);
                AddInput(command, "@P_ADDRESS", app.Address);
                AddInput(command, "@P_PLACE_OF_BUSINESS", app.PlaceOfBusiness);
                AddInput(command, "@P_NATIONALITY", app.Nationality);
                AddInput(command, "@P_DATE_OF_BIRTH", app.DateOfBirth);
                AddInput(command, "@P_PLACE_OF_BIRTH", app.PlaceOfBirth);
                AddInput(command, "@P_OCCUPATION", app.Occupation);
                AddInput(command, "@P_QUALIFICATION", app.Qualification);
                AddInput(command, "@P_REMOVED_FROM_TM", app.RemovedFromTrademark);

                var removalDateParam = new SqlParameter("@P_REMOVAL_DATE", SqlDbType.Date);
                removalDateParam.Value = (object?)app.RemovalDate ?? DBNull.Value;
                command.Parameters.Add(removalDateParam);

                AddInput(command, "@P_MOBILE_NO", app.MobileNo);
                AddInput(command, "@P_EMAIL", app.Email);
                AddInput(command, "@P_PHOTO_PATH", app.PhotoPath);
                AddInput(command, "@P_AGE_PROOF_PATH", app.AgeProofPath);
                AddInput(command, "@P_QUALIFICATION_CERT_PATH", app.QualificationCertPath);
                AddInput(command, "@P_CITIZENSHIP_PROOF_PATH", app.CitizenshipProofPath);

                var appNumberParam = new SqlParameter("@P_APPLICATION_NUMBER", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 0,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(appNumberParam);
                var statusParam = new SqlParameter("@P_STATUS", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusParam);
                var messageParam = new SqlParameter("@P_MESSAGE", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(messageParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                bool isSuccess = statusParam.Value.ToString() == "SUCCESS";
                return new RepositoryResult<int>
                {
                    IsSuccess = isSuccess,
                    Message = messageParam.Value.ToString() ?? string.Empty,
                    Id = isSuccess ? Convert.ToInt32(appNumberParam.Value.ToString()) : 0
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResult<int>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Id = 0
                };
            }
        }

        public async Task<RepositoryResult<AgentApplication?>> GetApplicationAsync(int applicationNumber)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = connection.CreateCommand();
                command.CommandText = "USP_AGENT_APPLICATION_GET";
                command.CommandType = CommandType.StoredProcedure;
                AddInput(command, "@P_ACTION", "GETDATA");
                AddInput(command, "@P_APPLICATION_NUMBER", applicationNumber);

                var statusParam = new SqlParameter("@P_STATUS", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusParam);
                var messageParam = new SqlParameter("@P_MESSAGE", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(messageParam);
                await connection.OpenAsync();

                AgentApplication? app = null;
                using (var dr = await command.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        app = new AgentApplication
                        {
                            ApplicationNumber = Convert.ToInt32(dr["APPLICATION_NUMBER"]),
                            BranchCode = dr["BRANCH_CODE"].ToString() ?? string.Empty,
                            EntryNo = dr["ENTRY_NO"].ToString() ?? string.Empty,
                            PartyType = dr["PARTY_TYPE"].ToString() ?? string.Empty,
                            PartyCode = dr["PARTY_CODE"].ToString() ?? string.Empty,
                            Name = dr["NAME"].ToString() ?? string.Empty,
                            Address = dr["ADDRESS"].ToString() ?? string.Empty,
                            PlaceOfBusiness = dr["PLACE_OF_BUSINESS"].ToString() ?? string.Empty,
                            Nationality = dr["NATIONALITY"].ToString() ?? string.Empty,
                            DateOfBirth = Convert.ToDateTime(dr["DATE_OF_BIRTH"]),
                            PlaceOfBirth = dr["PLACE_OF_BIRTH"].ToString() ?? string.Empty,
                            Occupation = dr["OCCUPATION"].ToString() ?? string.Empty,
                            Qualification = dr["QUALIFICATION"].ToString() ?? string.Empty,
                            RemovedFromTrademark = dr["REMOVED_FROM_TRADEMARK"].ToString() ?? "N",
                            RemovalDate = dr["REMOVAL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(dr["REMOVAL_DATE"]),
                            MobileNo = dr["MOBILE_NO"].ToString() ?? string.Empty,
                            Email = dr["EMAIL"].ToString() ?? string.Empty
                        };
                    }
                }

                bool isSuccess = statusParam.Value.ToString() == "SUCCESS";

                return new RepositoryResult<AgentApplication?>
                {
                    IsSuccess = isSuccess,
                    Message = messageParam.Value.ToString() ?? string.Empty,
                    Id = isSuccess ? app : null
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResult<AgentApplication?>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Id = null
                };
            }
        }

        public async Task<RepositoryResult<AgentApplication?>> GetApplicationByPartyAsync(string partyCode)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = connection.CreateCommand();

                command.CommandText = "USP_AGENT_APPLICATION_GET";
                command.CommandType = CommandType.StoredProcedure;
                AddInput(command, "@P_ACTION", "GETBYPARTY");
                AddInput(command, "@P_APPLICATION_NUMBER", 0);
                AddInput(command, "@P_PARTY_CODE", partyCode);

                var statusParam = new SqlParameter("@P_STATUS", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusParam);
                var messageParam = new SqlParameter("@P_MESSAGE", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(messageParam);
                await connection.OpenAsync();
                AgentApplication? app = null;

                using (var dr = await command.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        app = new AgentApplication
                        {
                            ApplicationNumber = Convert.ToInt32(dr["APPLICATION_NUMBER"]),
                            BranchCode = dr["BRANCH_CODE"].ToString() ?? string.Empty,
                            EntryNo = dr["ENTRY_NO"].ToString() ?? string.Empty,
                            PartyType = dr["PARTY_TYPE"].ToString() ?? string.Empty,
                            PartyCode = dr["PARTY_CODE"].ToString() ?? string.Empty,
                            Name = dr["NAME"].ToString() ?? string.Empty,
                            Address = dr["ADDRESS"].ToString() ?? string.Empty,
                            PlaceOfBusiness = dr["PLACE_OF_BUSINESS"].ToString() ?? string.Empty,
                            Nationality = dr["NATIONALITY"].ToString() ?? string.Empty,
                            DateOfBirth = Convert.ToDateTime(dr["DATE_OF_BIRTH"]),
                            PlaceOfBirth = dr["PLACE_OF_BIRTH"].ToString() ?? string.Empty,
                            Occupation = dr["OCCUPATION"].ToString() ?? string.Empty,
                            Qualification = dr["QUALIFICATION"].ToString() ?? string.Empty,
                            RemovedFromTrademark = dr["REMOVED_FROM_TRADEMARK"].ToString() ?? "N",
                            RemovalDate = dr["REMOVAL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(dr["REMOVAL_DATE"]),
                            MobileNo = dr["MOBILE_NO"].ToString() ?? string.Empty,
                            Email = dr["EMAIL"].ToString() ?? string.Empty
                        };
                    }
                }

                bool isSuccess = statusParam.Value.ToString() == "SUCCESS";

                return new RepositoryResult<AgentApplication?>
                {
                    IsSuccess = isSuccess,
                    Message = messageParam.Value.ToString() ?? string.Empty,
                    Id = isSuccess ? app : null
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResult<AgentApplication?>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Id = null
                };
            }
        }
    }
}