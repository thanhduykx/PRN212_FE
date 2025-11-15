using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.DTO
{
    public class RentalOrderDTO
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = "";
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int RentalLocationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PickupTime { get; set; }
        public string Status { get; set; } = "";
        public decimal Total { get; set; }
        public bool WithDriver { get; set; }
        public int? CitizenId { get; set; }
        public int? DriverLicenseId { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class UpdateRentalOrderStatusDTO
    {
        public int Id { get; set; }
        public string Status { get; set; } = "";
    }

}
