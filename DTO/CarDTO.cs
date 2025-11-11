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
            public int BatteryDuration { get; set; } // in km
            public double RentPricePerDay { get; set; }
            public double RentPricePerHour { get; set; }
            public double RentPricePerDayWithDriver { get; set; }
            public double RentPricePerHourWithDriver { get; set; }
            public string ImageUrl { get; set; } = "";
            public string ImageUrl2 { get; set; } = "";
            public string ImageUrl3 { get; set; } = "";
            public int Status { get; set; } // 0: Con xe, 1: Het xe
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsActive { get; set; } = true;
            public bool IsDeleted { get; set; } = false;
        }

        public class CreateCarDTO
        {
            public string Name { get; set; } = "";
            public string Model { get; set; } = "";
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
            public string ImageUrl2 { get; set; } = "";
            public string ImageUrl3 { get; set; } = "";
            public int Status { get; set; } = 0;
            public bool IsActive { get; set; } = true;
            public bool IsDeleted { get; set; } = false;
        }

        public class UpdateCarDTO : CreateCarDTO
        {
            public int Id { get; set; }
        }
    }



