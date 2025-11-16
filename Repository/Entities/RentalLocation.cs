using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class RentalLocation
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<CarRentalLocation> CarRentalLocations { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<RentalOrder> RentalOrders { get; set; }
        //public int LocationId { get; set; } 
        public bool IsDeleted { get; set; }
    }
}