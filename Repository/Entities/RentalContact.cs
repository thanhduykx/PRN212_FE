using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class RentalContact
    {
        [Key]
        public int Id { get; set; }
        public DateTime RentalDate { get; set; }
        public string RentalPeriod { get; set; }
        public DateTime ReturnDate { get; set; }
        public string TerminationClause { get; set; }
        public DocumentStatus Status { get; set; }
        public int? RentalOrderId { get; set; }
        public int LesseeId { get; set; }
        public int? LessorId { get; set; }
        public RentalOrder? RentalOrder { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}