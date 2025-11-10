namespace AssignmentPRN212.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = ""; // Customer / Staff / Admin
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public int? RentalLocationId { get; set; }
    }
}
