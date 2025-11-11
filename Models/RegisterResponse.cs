namespace AssignmentPRN212.Models
{
    public class RegisterResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Message { get; set; } = "";
    }
}

