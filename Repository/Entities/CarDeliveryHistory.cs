using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class CarDeliveryHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public RentalOrder Order { get; set; }
        public User Customer { get; set; }
        public User Staff { get; set; }
        public Car Car { get; set; }
        public int LocationId { get; set; }
        public RentalLocation Location { get; set; }
    }
}