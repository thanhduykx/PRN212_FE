using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class CarRentalLocation
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
        public Car Car { get; set; }
        public RentalLocation Location { get; set; }
        public bool IsDeleted { get; set; }

    }
}