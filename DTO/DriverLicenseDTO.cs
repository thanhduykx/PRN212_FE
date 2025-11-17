using System;

namespace AssignmentPRN212.DTO
{
    public class DriverLicenseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
        public int Status { get; set; } // 0: Pending, 1: Approved, 2: Rejected
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? RentalOrderId { get; set; }
    }

    public class CreateDriverLicenseDTO
    {
        public string Name { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
        public int? RentalOrderId { get; set; }
    }

    public class UpdateDriverLicenseStatusDTO
    {
        public int Id { get; set; }
        public int Status { get; set; } // 0: Pending, 1: Approved, 2: Rejected
    }

    public class UpdateDriverLicenseInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
    }
}

