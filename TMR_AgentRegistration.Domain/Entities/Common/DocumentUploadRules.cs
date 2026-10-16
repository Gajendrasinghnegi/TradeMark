using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Domain.Entities.Common
{
    public static class DocumentUploadRules
    {
        public static readonly Dictionary<string, (string[] AllowedTypes, long MaxSizeBytes, string Label)> Rules = new()
        {
            ["photo"] = (new[] { "image/jpeg", "image/png" }, 100 * 1024, "Passport-size Photograph"),
            ["ageProof"] = (new[] { "application/pdf", "image/jpeg", "image/png" }, 2 * 1024 * 1024, "Proof of Age / Date of Birth"),
            ["qualificationCert"] = (new[] { "application/pdf" }, 5 * 1024 * 1024, "Qualification / Degree Certificate"),
            ["citizenshipProof"] = (new[] { "application/pdf", "image/jpeg", "image/png" }, 2 * 1024 * 1024, "Proof of Citizenship"),
        };
    }
}
