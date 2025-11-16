using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class DriverLicense
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl2 { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int RentalOrderId { get; set; }
        public RentalOrder RentalOrder { get; set; }

    }
}
