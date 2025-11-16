namespace Service.DTOs
{
    public class CarDeliveryHistoryDTO
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; }
        public int OrderId { get; set; }
    }

    public class CarDeliveryHistoryCreateDTO
    {
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; } = string.Empty;
        public int OrderId { get; set; } 
    }

    public class CarDeliveryHistoryUpdateDTO : CarDeliveryHistoryCreateDTO
    {
        public int Id { get; set; }
    }
}
