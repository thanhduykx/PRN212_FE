using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public int? UserId { get; set; }
        public int? RentalOrderId { get; set; }
        public User? User { get; set; }
        public RentalOrder? RentalOrder { get; set; }
    }
}