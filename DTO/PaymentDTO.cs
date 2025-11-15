using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.DTO
{

    public class PaymentDTO
    {
        public int Id { get; set; }
        public int RentalOrderId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }



    public class CreatePaymentDTO
    {
        public int RentalOrderId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash";
        public string Status { get; set; } = "Completed";
        public DateTime TransactionDate { get; set; }
    }
}
