using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.DTO
{
    public class CitizenIdDTO
    {
        public int Id { get; set; }
        public string IdNumber { get; set; } = "";
        public string FullName { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        // Helper to check if valid
        public bool IsValid => ExpiryDate > DateTime.Now;
    }
}
