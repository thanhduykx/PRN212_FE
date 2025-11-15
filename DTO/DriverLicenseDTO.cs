using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.DTO
{
    public class DriverLicenseDTO
    {
        public int Id { get; set; }
        public string LicenseNumber { get; set; } = "";
        public string FullName { get; set; } = "";
        public string LicenseClass { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        // Helper property
        public bool IsValid => ExpiryDate > DateTime.Now;
    }
}
