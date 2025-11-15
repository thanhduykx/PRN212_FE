using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.DTO
{
    public class CarDeliveryHistoryDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryLocation { get; set; }
        public string? VehicleCondition { get; set; }
        public int? BatteryLevel { get; set; }
        public int? Mileage { get; set; }
        public string? PhotoUrls { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateCarDeliveryHistoryDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryLocation { get; set; }
        public string? VehicleCondition { get; set; }
        public int? BatteryLevel { get; set; }
        public int? Mileage { get; set; }
        public string? PhotoUrls { get; set; }
        public string? Notes { get; set; }
    }

}
