using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.DTO
{
    public class CarRentalLocationDTO
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
        public int Quantity { get; set; }
    }
}
