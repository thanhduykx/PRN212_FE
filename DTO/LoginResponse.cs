namespace AssignmentPRN212.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public string Message { get; set; } = "";
        public int UserId { get; set; }
        public int? RentalLocationId { get; set; }
    }
}
