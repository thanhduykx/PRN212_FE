using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.DTO
{
    public class CarDTO
    {
        public int Id { get; set; }
        public string Model { get; set; } = "";
        public string Name { get; set; } = "";
        public int Seats { get; set; }
        public string SizeType { get; set; } = "";
        public int TrunkCapacity { get; set; }
        public string BatteryType { get; set; } = "";
        public int BatteryDuration { get; set; }
        public double RentPricePerDay { get; set; }
        public double RentPricePerHour { get; set; }
        public double RentPricePerDayWithDriver { get; set; }
        public double RentPricePerHourWithDriver { get; set; }
        public string ImageUrl { get; set; } = "";
        public int Status { get; set; }
    }


    public class CarRentalLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RentalOrderDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime RentalDate { get; set; }
    }
}
