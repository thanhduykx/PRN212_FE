using System;
using System.Text.Json.Serialization;

namespace AssignmentPRN212.DTO
{
    public class CarDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
        public string Model { get; set; } = "";
        public string Name { get; set; } = "";
        public int Seats { get; set; }
        public string SizeType { get; set; } = "";
        public int TrunkCapacity { get; set; }
        public string BatteryType { get; set; } = "";
        public int BatteryDuration { get; set; } // in km
        public double RentPricePerDay { get; set; }
        public double RentPricePerHour { get; set; }
        public double RentPricePerDayWithDriver { get; set; }
        public double RentPricePerHourWithDriver { get; set; }
        [JsonPropertyName("depositAmount")] // Backend trả về camelCase
        public double DepositAmount { get; set; } = 0; // Số tiền đặt cọc (required by database)
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
        public string ImageUrl3 { get; set; } = "";
        public int Status { get; set; } // 0: Con xe, 1: Het xe
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)] // Không gửi CreatedAt khi tạo mới
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)] // Không gửi UpdatedAt (backend tự set)
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // Không gửi IsDeleted khi = false
        public bool IsDeleted { get; set; } = false;
        
        // Nested objects - Backend yêu cầu các field này phải có (empty arrays khi tạo mới)
        // Khi nhận từ API: bỏ qua navigation properties (chúng ta không cần parse chúng)
        // Khi gửi lên: không dùng CarDTO, dùng CreateCarDTO/UpdateCarDTO thay thế
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<object> CarRentalLocations { get; set; } = new List<object>();
        
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<object> RentalOrders { get; set; } = new List<object>();
        
        [JsonIgnore]
        public string StatusText => Status == 1 ? "1 - Hết xe" : "0 - Còn xe";
        
        [JsonIgnore]
        public string? LocationName { get; set; } // Tên địa điểm (sẽ được set từ code-behind)
    }

    // DTO để tạo xe mới (không có Id và nested objects)
    // Backend nhận Car entity, nhưng không cần navigation properties
    public class CreateCarDTO
    {
        public string Model { get; set; } = "";
        public string Name { get; set; } = "";
        public int Seats { get; set; }
        public string SizeType { get; set; } = "";
        public int TrunkCapacity { get; set; }
        public string BatteryType { get; set; } = "";
        public int BatteryDuration { get; set; }
        public double RentPricePerDay { get; set; }
        public double RentPricePerHour { get; set; }
        public double RentPricePerDayWithDriver { get; set; }
        public double RentPricePerHourWithDriver { get; set; }
        public double DepositAmount { get; set; } = 0; // Số tiền đặt cọc
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
        public string ImageUrl3 { get; set; } = "";
        public int Status { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        
        // Backend yêu cầu các field này phải có - gửi empty arrays (camelCase để match với Swagger)
        [JsonPropertyName("carRentalLocations")]
        public List<object> CarRentalLocations { get; set; } = new List<object>();
        
        [JsonPropertyName("rentalOrders")]
        public List<object> RentalOrders { get; set; } = new List<object>();
    }

    // DTO để cập nhật xe (có Id trong body để khớp với URL)
    public class UpdateCarDTO
    {
        public int Id { get; set; } // Backend yêu cầu Id trong body phải khớp với URL
        public string Model { get; set; } = "";
        public string Name { get; set; } = "";
        public int Seats { get; set; }
        public string SizeType { get; set; } = "";
        public int TrunkCapacity { get; set; }
        public string BatteryType { get; set; } = "";
        public int BatteryDuration { get; set; }
        public double RentPricePerDay { get; set; }
        public double RentPricePerHour { get; set; }
        public double RentPricePerDayWithDriver { get; set; }
        public double RentPricePerHourWithDriver { get; set; }
        public double DepositAmount { get; set; } = 0; // Số tiền đặt cọc
        public string ImageUrl { get; set; } = "";
        public string ImageUrl2 { get; set; } = "";
        public string ImageUrl3 { get; set; } = "";
        public int Status { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        
        // Backend yêu cầu các field này phải có - gửi empty arrays (camelCase để match với Swagger)
        [JsonPropertyName("carRentalLocations")]
        public List<object> CarRentalLocations { get; set; } = new List<object>();
        
        [JsonPropertyName("rentalOrders")]
        public List<object> RentalOrders { get; set; } = new List<object>();
    }

    //public class RentalOrderDTO
    //{
    //    public int Id { get; set; }
    //    public string CustomerName { get; set; } = "";
    //    public DateTime RentalDate { get; set; }
    //}
}
