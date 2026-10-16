using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Domain.Entities
{
    public class AgentApplication
    {
        public int ApplicationNumber { get; set; }
        public string ApplicationType { get; set; } = string.Empty;
        public string FormNumber { get; set; } = string.Empty;
        public string PartyType { get; set; } = string.Empty;
        public string PartyCode { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public decimal FormAmount { get; set; }
        public string LastUpdatedBy { get; set; } = string.Empty;
        public string HtmlPreview { get; set; } = string.Empty;
        public string EntryNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PlaceOfBusiness { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string RemovedFromTrademark { get; set; } = string.Empty;
        public DateTime? RemovalDate { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public string AgeProofPath { get; set; } = string.Empty;
        public string QualificationCertPath { get; set; } = string.Empty;
        public string CitizenshipProofPath { get; set; } = string.Empty;
    }
}
