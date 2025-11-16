using Repository.Entities;
using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class DriverLicenseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl2 { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int RentalOrderId { get; set; }
    }
    public class CreateDriverLicenseDTO
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl2 { get; set; }
        public int RentalOrderId { get; set; }
    }
    public class UpdateDriverLicenseStatusDTO
    {
        public int DriverLicenseId { get; set; }
        public DocumentStatus Status { get; set; }
    }
    public class UpdateDriverLicenseInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl2 { get; set; }
    }
}
