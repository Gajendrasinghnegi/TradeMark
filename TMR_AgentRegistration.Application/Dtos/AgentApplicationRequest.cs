using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos
{
    public class AgentApplicationRequest
    {
        [Required(ErrorMessage = "Please select a branch")]
        [StringLength(1, ErrorMessage = "Branch Code must be a single character")]
        public string BranchCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a request type")]
        [StringLength(2, ErrorMessage = "Entry No cannot exceed 2 characters")]
        public string EntryNo { get; set; } = string.Empty;

        // Auth/Login system 
        [Required(ErrorMessage = "Party Type is required")]
        [StringLength(1, ErrorMessage = "Party Type must be a single character")]
        public string PartyType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Party Code is required")]
        [StringLength(20, ErrorMessage = "Party Code cannot exceed 20 characters")]
        public string PartyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place of Business is required")]
        [StringLength(100, ErrorMessage = "Place of Business cannot exceed 100 characters")]
        public string PlaceOfBusiness { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select nationality")]
        [StringLength(50, ErrorMessage = "Nationality cannot exceed 50 characters")]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Place of Birth is required")]
        [StringLength(100, ErrorMessage = "Place of Birth cannot exceed 100 characters")]
        public string PlaceOfBirth { get; set; } = string.Empty;

        [Required(ErrorMessage = "Occupation is required")]
        [StringLength(50, ErrorMessage = "Occupation cannot exceed 50 characters")]
        public string Occupation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Qualification is required")]
        [StringLength(100, ErrorMessage = "Qualification cannot exceed 100 characters")]
        public string Qualification { get; set; } = string.Empty;

        public bool IsRemovedFromTrademark { get; set; }

        public DateTime? RemovalDate { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;
        public int? ApplicationNumber { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? AgeProof { get; set; }
        public IFormFile? QualificationCert { get; set; }
        public IFormFile? CitizenshipProof { get; set; }
    }
}
