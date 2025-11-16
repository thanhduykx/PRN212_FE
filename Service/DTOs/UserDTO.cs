using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class UserDTO
    {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; } // 1. Customer, 2. Staff, 3. Admin
            public bool IsActive { get; set; }
    }

    public class CreateStaffUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; } 
        public int RentalLocationId { get; set; }
    }

    public class UpdateUserDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }
    public class UpdateCustomerNameDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
    }
    public class UpdateCustomerPasswordDTO
    {
        public int UserId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
