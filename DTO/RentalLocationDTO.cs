namespace AssignmentPRN212.DTO
{
    public class RentalLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Coordinates { get; set; } = "";
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CreateRentalLocationRequest
    {
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Coordinates { get; set; } = "";
    }

    public class UpdateRentalLocationRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Coordinates { get; set; } = "";
        public bool IsActive { get; set; }
    }
}
