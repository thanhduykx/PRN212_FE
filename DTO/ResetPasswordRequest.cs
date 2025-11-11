namespace AssignmentPRN212.DTO
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = "";
        public string OTP { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}

