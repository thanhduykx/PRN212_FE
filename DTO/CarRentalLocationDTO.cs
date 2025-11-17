namespace AssignmentPRN212.DTO
{
    public class CarRentalLocationDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
        
        // Display properties (sẽ được set từ code-behind)
        public string? CarName { get; set; }
        public string? LocationName { get; set; }
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

