using System;

namespace AssignmentPRN212.DTO
{
    public class RentalOrderDTO
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ExpectedReturnTime { get; set; }
        public DateTime? ActualReturnTime { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("subTotal")]
        public double? SubTotal { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("deposit")]
        public double? Deposit { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("total")]
        public double? Total { get; set; }
        public int? Discount { get; set; }
        public double? ExtraFee { get; set; }
        public double? DamageFee { get; set; }
        public string? DamageNotes { get; set; }
        public bool WithDriver { get; set; }
        public string Status { get; set; } = ""; // String từ API
        public RentalOrderStatus? StatusEnum { get; set; } // Enum để xử lý
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int RentalLocationId { get; set; }
        public int? RentalContactId { get; set; }
        public int? CitizenId { get; set; }
        public int? DriverLicenseId { get; set; }
        public int? PaymentId { get; set; }

        // Display properties (sẽ được set từ code-behind)
        [System.Text.Json.Serialization.JsonIgnore]
        public string? CarName { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public string? UserName { get; set; }
        
        public string StatusText => Status;
        public string WithDriverText => WithDriver ? "Có" : "Không";
        public string TotalText => $"{(Deposit ?? 0) + (SubTotal ?? 0):N0} VNĐ";
        
        // Phí tài xế (sẽ được set từ code-behind)
        [System.Text.Json.Serialization.JsonIgnore]
        public string? DriverFeeText { get; set; }
        
        // Helper để convert string status sang enum
        public RentalOrderStatus GetStatusEnum()
        {
            if (Enum.TryParse<RentalOrderStatus>(Status, out var statusEnum))
                return statusEnum;
            return RentalOrderStatus.Pending;
        }
    }

    public class CreateRentalOrderDTO
    {
        public string PhoneNumber { get; set; } = "";
        public DateTime PickupTime { get; set; }
        public DateTime ExpectedReturnTime { get; set; }
        public bool WithDriver { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int RentalLocationId { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("subTotal")]
        public double? SubTotal { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("deposit")]
        public double? Deposit { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("total")]
        public double? Total { get; set; }
    }

    public class UpdateRentalOrderTotalDTO
    {
        public int OrderId { get; set; }
        public double ExtraFee { get; set; }
        public double DamageFee { get; set; }
        public string DamageNotes { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("discount")]
        public int? Discount { get; set; }
    }

    public class UpdateRentalOrderStatusDTO
    {
        public int OrderId { get; set; }
        public RentalOrderStatus Status { get; set; }
    }
}

