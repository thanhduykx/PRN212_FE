using System;

namespace AssignmentPRN212.DTO
{
    public class CarReturnHistoryDTO
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; } = "";
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
        
        // Display properties (sẽ được set từ code-behind)
        public string? CarName { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public string? TotalText { get; set; } // Tổng tiền = Deposit + SubTotal từ order
    }

    public class CarReturnHistoryCreateDTO
    {
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; } = "";
        public int OrderId { get; set; }
    }

    public class CarReturnHistoryUpdateDTO
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; } = "";
        public int OrderId { get; set; }
    }
}

