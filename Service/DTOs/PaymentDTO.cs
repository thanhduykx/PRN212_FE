using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public string UserFullname { get; set; }
        public string Car { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class CreatePaymentDTO
    {
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public int UserId { get; set; }
    }
    public class UpdatePaymentStatusDTO
    {
        public int Id { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
