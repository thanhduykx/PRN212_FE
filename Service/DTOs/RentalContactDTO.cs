using Repository.Entities.Enum;
using System;

namespace Service.DTOs
{
    public class RentalContactDTO
    {
        public int Id { get; set; }
        public DateTime RentalDate { get; set; }
        public string RentalPeriod { get; set; }
        public DateTime ReturnDate { get; set; }
        public string TerminationClause { get; set; }
        public DocumentStatus Status { get; set; }
        public int? RentalOrderId { get; set; }
        public int LesseeId { get; set; }
        public int? LessorId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class RentalContactCreateDTO
    {
        public DateTime RentalDate { get; set; }
        public string RentalPeriod { get; set; }
        public DateTime ReturnDate { get; set; }
        public string TerminationClause { get; set; }
        public DocumentStatus Status { get; set; }
        public int? RentalOrderId { get; set; }
        public int LesseeId { get; set; }
        public int? LessorId { get; set; }
    }

    public class RentalContactUpdateDTO : RentalContactCreateDTO
    {
        public int Id { get; set; }
    }
}
