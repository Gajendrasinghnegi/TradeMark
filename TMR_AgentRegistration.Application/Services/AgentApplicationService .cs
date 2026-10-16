using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;
using TMR_AgentRegistration.Application.IServices;
using TMR_AgentRegistration.Domain.Entities;
using TMR_AgentRegistration.Domain.Entities.Common;
using TMR_AgentRegistration.Domain.IRepositories;

namespace TMR_AgentRegistration.Application.Services
{
    public class AgentApplicationService : IAgentApplicationService
    {
        private readonly IAgentApplicationRepository _repository;
        private const string UploadRoot = "wwwroot/UploadedFiles";

        public AgentApplicationService(IAgentApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<int>> SaveApplicationAsync(AgentApplicationRequest request)
        {
            var (photoPath, photoError) = await SaveDocumentAsync(request.Photo, "photo", "Photos");
            if (photoError != null) return Fail(photoError);

            var (ageProofPath, ageProofError) = await SaveDocumentAsync(request.AgeProof, "ageProof", "AgeProofs");
            if (ageProofError != null) return Fail(ageProofError);

            var (qualPath, qualError) = await SaveDocumentAsync(request.QualificationCert, "qualificationCert", "QualificationCerts");
            if (qualError != null) return Fail(qualError);

            var (citizenPath, citizenError) = await SaveDocumentAsync(request.CitizenshipProof, "citizenshipProof", "CitizenshipProofs");
            if (citizenError != null) return Fail(citizenError);
            var application = new AgentApplication
            {
                ApplicationType = "X",
                FormNumber = "TM-G",
                FormAmount = 4500,              // TEMP: Master table 
                LastUpdatedBy = "SYSTEM",       
                HtmlPreview = string.Empty, 
                // ---- Frontend 
                BranchCode = request.BranchCode,
                EntryNo = request.EntryNo,
                PartyType = request.PartyType,
                PartyCode = request.PartyCode,
                Name = request.Name,
                Address = request.Address,
                PlaceOfBusiness = request.PlaceOfBusiness,
                Nationality = request.Nationality,
                DateOfBirth = request.DateOfBirth,
                PlaceOfBirth = request.PlaceOfBirth,
                Occupation = request.Occupation,
                Qualification = request.Qualification,
                RemovedFromTrademark = request.IsRemovedFromTrademark ? "Y" : "N",
                RemovalDate = request.IsRemovedFromTrademark ? request.RemovalDate : null,
                MobileNo = request.MobileNo,
                Email = request.Email ,
                PhotoPath = photoPath,
                AgeProofPath = ageProofPath,
                QualificationCertPath = qualPath,
                CitizenshipProofPath = citizenPath
            };

            var result = await _repository.SaveApplicationAsync(application);
            return new ApiResponse<int>
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Id = result.Id
            };
        }

        // Validates the file against DocumentUploadRules, saves it to disk,
        // and returns the relative path to store in the DB.
        private async Task<(string Path, string? Error)> SaveDocumentAsync(IFormFile? file, string docKey, string subFolder)
        {
            var rule = DocumentUploadRules.Rules[docKey];

            if (file == null || file.Length == 0)
                return (string.Empty, $"{rule.Label} is required");

            if (!rule.AllowedTypes.Contains(file.ContentType))
                return (string.Empty, $"{rule.Label}: invalid file format. Allowed types: {string.Join(", ", rule.AllowedTypes)}");

            if (file.Length > rule.MaxSizeBytes)
                return (string.Empty, $"{rule.Label}: file exceeds max size of {rule.MaxSizeBytes / 1024} KB");

            var folder = Path.Combine(UploadRoot, subFolder);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Store only the relative path in the DB, e.g. UploadedFiles/Photos/xxxx.jpg
            return ($"UploadedFiles/{subFolder}/{fileName}", null);
        }

        private static ApiResponse<int> Fail(string message) =>
            new() { IsSuccess = false, Message = message, Id = 0 };

        public async Task<ApiResponse<AgentApplicationRequest?>> GetApplicationAsync(int applicationNumber)
        {
            var result = await _repository.GetApplicationAsync(applicationNumber);

            AgentApplicationRequest? response = null;

            if (result.IsSuccess && result.Id != null)
            {
                var app = result.Id;
                response = new AgentApplicationRequest
                {
                    ApplicationNumber = app.ApplicationNumber,
                    BranchCode = app.BranchCode,
                    EntryNo = app.EntryNo,
                    PartyType = app.PartyType,
                    PartyCode = app.PartyCode,
                    Name = app.Name,
                    Address = app.Address,
                    PlaceOfBusiness = app.PlaceOfBusiness,
                    Nationality = app.Nationality,
                    DateOfBirth = app.DateOfBirth,
                    PlaceOfBirth = app.PlaceOfBirth,
                    Occupation = app.Occupation,
                    Qualification = app.Qualification,
                    IsRemovedFromTrademark = app.RemovedFromTrademark == "Y",
                    RemovalDate = app.RemovalDate,
                    MobileNo = app.MobileNo,
                    Email = app.Email
                };
            }

            return new ApiResponse<AgentApplicationRequest?>
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Id = response
            };
        }
        public async Task<ApiResponse<AgentApplicationRequest?>> GetApplicationByPartyAsync(string partyCode)
        {
            var result = await _repository.GetApplicationByPartyAsync(partyCode);

            AgentApplicationRequest? response = null;

            if (result.IsSuccess && result.Id != null)
            {
                var app = result.Id;
                response = new AgentApplicationRequest
                {
                    ApplicationNumber = app.ApplicationNumber,
                    BranchCode = app.BranchCode,
                    EntryNo = app.EntryNo,
                    PartyType = app.PartyType,
                    PartyCode = app.PartyCode,
                    Name = app.Name,
                    Address = app.Address,
                    PlaceOfBusiness = app.PlaceOfBusiness,
                    Nationality = app.Nationality,
                    DateOfBirth = app.DateOfBirth,
                    PlaceOfBirth = app.PlaceOfBirth,
                    Occupation = app.Occupation,
                    Qualification = app.Qualification,
                    IsRemovedFromTrademark = app.RemovedFromTrademark == "Y",
                    RemovalDate = app.RemovalDate,
                    MobileNo = app.MobileNo,
                    Email = app.Email
                };
            }

            return new ApiResponse<AgentApplicationRequest?>
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Id = response
            };
        }
    }
}
