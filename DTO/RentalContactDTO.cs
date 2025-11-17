using System;

namespace AssignmentPRN212.DTO
{
    public class RentalContactDTO
    {
        public int Id { get; set; }
        public DateTime RentalDate { get; set; }
        public string RentalPeriod { get; set; } = "";
        public DateTime? ReturnDate { get; set; }
        public string TerminationClause { get; set; } = "";
        public int Status { get; set; } // 0: Draft, 1: Signed, 2: Terminated
        public int RentalOrderId { get; set; }
        public int LesseeId { get; set; } // Customer
        public int LessorId { get; set; } // Staff/Company
        public bool IsDeleted { get; set; }
    }

    public class CreateRentalContactDTO
    {
        public DateTime RentalDate { get; set; }
        public string RentalPeriod { get; set; } = "";
        public DateTime? ReturnDate { get; set; }
        public string TerminationClause { get; set; } = "";
        public int RentalOrderId { get; set; }
        public int LesseeId { get; set; }
        public int LessorId { get; set; }
    }

    public class UpdateRentalContactDTO
    {
        public int Id { get; set; }
        public DateTime RentalDate { get; set; }
        public string RentalPeriod { get; set; } = "";
        public DateTime? ReturnDate { get; set; }
        public string TerminationClause { get; set; } = "";
        public int Status { get; set; }
    }
}

