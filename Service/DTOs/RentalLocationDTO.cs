using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class RentalLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public bool IsActive { get; set; }
    }
    public class CreateRentalLocationDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdateRentalLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public bool IsActive { get; set; }
    }
}
