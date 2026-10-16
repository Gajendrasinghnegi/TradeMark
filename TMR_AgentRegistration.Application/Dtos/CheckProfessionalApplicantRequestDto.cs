using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Application.Dtos
{
    public  class CheckProfessionalApplicantRequestDto
    {
        [Required(ErrorMessage = "Qualification Basis is required.")]
        public string QualificationBasis { get; set; }
        public string? EnrolmentNo { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only alphabets are allowed in Full Name.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string Mobile { get; set; }
    }
}
