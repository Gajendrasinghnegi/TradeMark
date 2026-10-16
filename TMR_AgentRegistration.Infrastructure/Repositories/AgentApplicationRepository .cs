using Oracle.ManagedDataAccess.Client;
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
        private readonly IOracleConnectionFactory _connectionFactory;

        public AgentApplicationRepository(IOracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<RepositoryResult<int>> SaveApplicationAsync(AgentApplication app)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = connection.CreateCommand();
                command.CommandText = "SP_AGENT_APPLICATION_SAVE";
                command.CommandType = CommandType.StoredProcedure;

                // All INPUT parameters ----------
                command.Parameters.Add(new OracleParameter("P_APPLICATION_TYPE", app.ApplicationType));
                command.Parameters.Add(new OracleParameter("P_FORM_NUMBER", app.FormNumber));
                command.Parameters.Add(new OracleParameter("P_PARTY_TYPE", app.PartyType));
                command.Parameters.Add(new OracleParameter("P_PARTY_CODE", app.PartyCode));
                command.Parameters.Add(new OracleParameter("P_BRANCH_CODE", app.BranchCode));
                command.Parameters.Add(new OracleParameter("P_FORM_AMOUNT", app.FormAmount));
                command.Parameters.Add(new OracleParameter("P_LAST_UPDATED_BY", app.LastUpdatedBy));
                command.Parameters.Add(new OracleParameter("P_HTML_PREVIEW", app.HtmlPreview));
                command.Parameters.Add(new OracleParameter("P_ENTRY_NO", app.EntryNo));
                command.Parameters.Add(new OracleParameter("P_NAME", app.Name));
                command.Parameters.Add(new OracleParameter("P_ADDRESS", app.Address));
                command.Parameters.Add(new OracleParameter("P_PLACE_OF_BUSINESS", app.PlaceOfBusiness));
                command.Parameters.Add(new OracleParameter("P_NATIONALITY", app.Nationality));
                command.Parameters.Add(new OracleParameter("P_DATE_OF_BIRTH", app.DateOfBirth));
                command.Parameters.Add(new OracleParameter("P_PLACE_OF_BIRTH", app.PlaceOfBirth));
                command.Parameters.Add(new OracleParameter("P_OCCUPATION", app.Occupation));
                command.Parameters.Add(new OracleParameter("P_QUALIFICATION", app.Qualification));
                command.Parameters.Add(new OracleParameter("P_REMOVED_FROM_TM", app.RemovedFromTrademark));

                var removalDateParam = new OracleParameter("P_REMOVAL_DATE", OracleDbType.Date);
                removalDateParam.Value = (object?)app.RemovalDate ?? DBNull.Value;
                command.Parameters.Add(removalDateParam);

                command.Parameters.Add(new OracleParameter("P_MOBILE_NO", app.MobileNo));
                command.Parameters.Add(new OracleParameter("P_EMAIL", app.Email));

                command.Parameters.Add(new OracleParameter("P_PHOTO_PATH", app.PhotoPath));
                command.Parameters.Add(new OracleParameter("P_AGE_PROOF_PATH", app.AgeProofPath));
                command.Parameters.Add(new OracleParameter("P_QUALIFICATION_CERT_PATH", app.QualificationCertPath));
                command.Parameters.Add(new OracleParameter("P_CITIZENSHIP_PROOF_PATH", app.CitizenshipProofPath));

                var appNumberParam = new OracleParameter("P_APPLICATION_NUMBER", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(appNumberParam);

                var statusParam = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 50)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusParam);

                var messageParam = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500)
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

                command.CommandText = "SP_AGENT_APPLICATION_GET";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("P_ACTION", "GETDATA"));
                command.Parameters.Add(new OracleParameter("P_APPLICATION_NUMBER", applicationNumber));
                var cursorParam = new OracleParameter("P_RESULT", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParam);
                var statusParam = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 50)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusParam);
                var messageParam = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(messageParam);

                await connection.OpenAsync();

                AgentApplication? app = null;
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        app = new AgentApplication
                        {
                            ApplicationNumber = Convert.ToInt32(reader["APPLICATION_NUMBER"]),
                            BranchCode = reader["BRANCH_CODE"].ToString() ?? string.Empty,
                            EntryNo = reader["ENTRY_NO"].ToString() ?? string.Empty,
                            PartyType = reader["PARTY_TYPE"].ToString() ?? string.Empty,
                            PartyCode = reader["PARTY_CODE"].ToString() ?? string.Empty,
                            Name = reader["NAME"].ToString() ?? string.Empty,
                            Address = reader["ADDRESS"].ToString() ?? string.Empty,
                            PlaceOfBusiness = reader["PLACE_OF_BUSINESS"].ToString() ?? string.Empty,
                            Nationality = reader["NATIONALITY"].ToString() ?? string.Empty,
                            DateOfBirth = Convert.ToDateTime(reader["DATE_OF_BIRTH"]),
                            PlaceOfBirth = reader["PLACE_OF_BIRTH"].ToString() ?? string.Empty,
                            Occupation = reader["OCCUPATION"].ToString() ?? string.Empty,
                            Qualification = reader["QUALIFICATION"].ToString() ?? string.Empty,
                            RemovedFromTrademark = reader["REMOVED_FROM_TRADEMARK"].ToString() ?? "N",
                            RemovalDate = reader["REMOVAL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["REMOVAL_DATE"]),
                            MobileNo = reader["MOBILE_NO"].ToString() ?? string.Empty,
                            Email = reader["EMAIL"].ToString() ?? string.Empty
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

                command.CommandText = "SP_AGENT_APPLICATION_GET";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("P_ACTION", "GETBYPARTY"));
                command.Parameters.Add(new OracleParameter("P_APPLICATION_NUMBER", 0));
                command.Parameters.Add(new OracleParameter("P_PARTY_CODE", partyCode));

                var cursorParam = new OracleParameter("P_RESULT", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParam);

                var statusParam = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 50)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(statusParam);

                var messageParam = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(messageParam);

                await connection.OpenAsync();

                AgentApplication? app = null;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        app = new AgentApplication
                        {
                            ApplicationNumber = Convert.ToInt32(reader["APPLICATION_NUMBER"]),
                            BranchCode = reader["BRANCH_CODE"].ToString() ?? string.Empty,
                            EntryNo = reader["ENTRY_NO"].ToString() ?? string.Empty,
                            PartyType = reader["PARTY_TYPE"].ToString() ?? string.Empty,
                            PartyCode = reader["PARTY_CODE"].ToString() ?? string.Empty,
                            Name = reader["NAME"].ToString() ?? string.Empty,
                            Address = reader["ADDRESS"].ToString() ?? string.Empty,
                            PlaceOfBusiness = reader["PLACE_OF_BUSINESS"].ToString() ?? string.Empty,
                            Nationality = reader["NATIONALITY"].ToString() ?? string.Empty,
                            DateOfBirth = Convert.ToDateTime(reader["DATE_OF_BIRTH"]),
                            PlaceOfBirth = reader["PLACE_OF_BIRTH"].ToString() ?? string.Empty,
                            Occupation = reader["OCCUPATION"].ToString() ?? string.Empty,
                            Qualification = reader["QUALIFICATION"].ToString() ?? string.Empty,
                            RemovedFromTrademark = reader["REMOVED_FROM_TRADEMARK"].ToString() ?? "N",
                            RemovalDate = reader["REMOVAL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["REMOVAL_DATE"]),
                            MobileNo = reader["MOBILE_NO"].ToString() ?? string.Empty,
                            Email = reader["EMAIL"].ToString() ?? string.Empty
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
