using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class CarReturnHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        //  Thêm LocationId để xác định chi nhánh trả xe
        public int LocationId { get; set; }
        public RentalLocation Location { get; set; }  // Navigation property

        public RentalOrder Order { get; set; }
        public User Customer { get; set; }
        public User Staff { get; set; }
        public Car Car { get; set; }
    }
}