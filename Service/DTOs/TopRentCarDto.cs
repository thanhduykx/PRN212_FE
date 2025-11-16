namespace Service.DTOs
{
    public class TopRentCarDto
    {
        public int CarId { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Seats { get; set; }
        public string SizeType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RentalCount { get; set; }
    }
}
