using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class RentalOrder
    {
        [Key]
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
        public RentalOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RentalLocationId { get; set; }
        public RentalLocation RentalLocation { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int? RentalContactId { get; set; }
        public RentalContact? RentalContact { get; set; }
        public int? CitizenId { get; set; }
        public CitizenId? CitizenIdNavigation { get; set; }
        public int? DriverLicenseId { get; set; }
        public DriverLicense? DriverLicense { get; set; }
        public int? PaymentId { get; set; }
        public Payment? Payment { get; set; }
    }
}