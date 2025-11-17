using System;

namespace AssignmentPRN212.DTO
{
    public class CarDeliveryHistoryDTO
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; } = "";
        public int OrderId { get; set; }
        public int CarId { get; set; }
        
        // Display properties (sẽ được set từ code-behind)
        public string? CarName { get; set; }
    }

    public class CarDeliveryHistoryCreateDTO
    {
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; } = "";
        public int OrderId { get; set; }
    }

    public class CarDeliveryHistoryUpdateDTO
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; } = "";
        public int OrderId { get; set; }
    }
}

