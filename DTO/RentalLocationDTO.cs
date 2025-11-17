namespace AssignmentPRN212.DTO
{
    public class RentalLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Coordinates { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public class CreateRentalLocationDTO
    {
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Coordinates { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class UpdateRentalLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Coordinates { get; set; } = "";
        public bool IsActive { get; set; }
    }
}
