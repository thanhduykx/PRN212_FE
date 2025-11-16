using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class CarRentalLocationDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
    }
    public class CreateCarRentalLocationDTO
    {
        public int Quantity { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
    }
    public class UpdateCarRentalLocationDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
