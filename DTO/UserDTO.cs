namespace AssignmentPRN212.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = ""; // Customer / Staff / Admin
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public int? RentalLocationId { get; set; }
    }

    public class CreateStaffUserDTO
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "Staff"; // mặc định
    }

    public class UpdateUserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCustomerNameDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
    }

    public class UpdateCustomerPasswordDTO
    {
        public Guid Id { get; set; }
        public string NewPassword { get; set; } = "";
    }
}
