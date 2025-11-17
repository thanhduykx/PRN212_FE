namespace AssignmentPRN212.DTO
{
    public class UpdateCustomerPasswordDTO
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}

