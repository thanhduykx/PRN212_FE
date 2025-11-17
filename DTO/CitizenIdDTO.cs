using System;

namespace AssignmentPRN212.DTO
{
    public class CitizenIdDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string CitizenIdNumber { get; set; } = "";
        public DateTime? BirthDate { get; set; }
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
        public int Status { get; set; } // 0: Pending, 1: Approved, 2: Rejected
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? RentalOrderId { get; set; }
    }

    public class CreateCitizenIdDTO
    {
        public string Name { get; set; } = "";
        public string CitizenIdNumber { get; set; } = "";
        public DateTime? BirthDate { get; set; }
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
        public int? RentalOrderId { get; set; }
    }

    public class UpdateCitizenIdStatusDTO
    {
        public int Id { get; set; }
        public int Status { get; set; } // 0: Pending, 1: Approved, 2: Rejected
    }

    public class UpdateCitizenIdInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string CitizenIdNumber { get; set; } = "";
        public DateTime? BirthDate { get; set; }
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
    }
}

