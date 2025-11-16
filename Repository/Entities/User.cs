    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Repository.Entities
    {
        public class User
        {
            [Key]
            public int Id { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string PasswordHash { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; } // 1. Customer, 2. Staff, 3. Admin
            public string? ConfirmEmailToken { get; set; }
            public bool IsEmailConfirmed { get; set; } = false;
            public string? ResetPasswordToken { get; set; }
            public DateTime? ResetPasswordTokenExpiry { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsActive { get; set; }
            public int? RentalLocationId { get; set; }
            public RentalLocation? RentalLocation { get; set; }
            public ICollection<Feedback> Feedback { get; set; }
            public ICollection<RentalOrder> RentalOrders { get; set; }
            public ICollection<Payment> Payments { get; set; }
            public ICollection<CarDeliveryHistory> CarDeliveryHistories { get; set; }
            public ICollection<RentalContact> RentalContacts { get; set; }
        }
    }