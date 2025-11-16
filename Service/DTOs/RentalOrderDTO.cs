using Repository.Entities;
using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class RentalOrderDTO
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ExpectedReturnTime { get; set; }
        public DateTime? ActualReturnTime { get; set; }
        public double? SubTotal { get; set; }
        public double? Total { get; set; }
        public int? Discount { get; set; }
        public double? ExtraFee { get; set; }
        public double? DamageFee { get; set; }
        public string? DamageNotes { get; set; }
        public bool WithDriver { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int RentalLocationId { get; set; }
        public int? RentalContactId { get; set; }
        public int? CitizenId { get; set; }
        public int? DriverLicenseId { get; set; }
        public int? PaymentId { get; set; }
    }
    public class CreateRentalOrderDTO
    {
        public string PhoneNumber { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ExpectedReturnTime { get; set; }
        public bool WithDriver { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int RentalLocationId { get; set; }
    }
    public class UpdateRentalOrderTotalDTO
    {
        public int OrderId { get; set; }
        public double ExtraFee { get; set; }
        public double DamageFee { get; set; }
        public string DamageNotes { get; set; }
    }
    public class UpdateRentalOrderStatusDTO
    {
        public int OrderId { get; set; }
        public RentalOrderStatus Status { get; set; }
    }
}
