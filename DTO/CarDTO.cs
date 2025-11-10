using System;

namespace AssignmentPRN212.DTO
{
    public class CarDTO
    {
        public int Id { get; set; }

        public string Model { get; set; } = "";   // string -> OK
        public string Name { get; set; } = "";    // string -> OK

        public int Seats { get; set; }             // int -> OK
        public string SizeType { get; set; } = ""; // string -> OK
        public int TrunkCapacity { get; set; }    // int -> OK

        public string BatteryType { get; set; } = ""; // string -> OK
        public int BatteryDuration { get; set; }      // int -> OK

        public double RentPricePerDay { get; set; }          // double -> backend kiểu double
        public double RentPricePerHour { get; set; }         // double
        public double RentPricePerDayWithDriver { get; set; }// double
        public double RentPricePerHourWithDriver { get; set; }// double

        public int Status { get; set; }                      // int -> 0 hoặc 1
        public string StatusText => Status == 1 ? "Đã thuê" : "Còn";

   
    }

    public class CarRentalLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class RentalOrderDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public DateTime RentalDate { get; set; }
    }
}
