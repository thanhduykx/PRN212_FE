using System;

namespace AssignmentPRN212.DTO
{
    // Enum cho PaymentType và PaymentStatus - khớp với backend
    public enum PaymentType
    {
        Deposit,
        OrderPayment,
        RefundPayment
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; } = "";
        public PaymentStatus Status { get; set; }
        public string UserId { get; set; } = "";
        public string OrderId { get; set; } = "";
        public DateTime OrderDate { get; set; }
    }

    public class CreatePaymentDTO
    {
        public DateTime? PaymentDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public double Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BillingImageUrl { get; set; }
        public int? UserId { get; set; }
        public int? RentalOrderId { get; set; }
    }

    public class UpdatePaymentStatusDTO
    {
        public int Id { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class ConfirmDepositPaymentDTO
    {
        public int OrderId { get; set; }
    }
}

