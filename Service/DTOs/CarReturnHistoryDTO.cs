using System;

namespace Service.DTOs
{
    // 🔹 DTO trả về (đầy đủ thông tin)
    public class CarReturnHistoryDTO
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; } = string.Empty;

        // 🔗 Khóa ngoại chính
        public int OrderId { get; set; }

        // 🔹 Các trường dưới có thể được ánh xạ từ Order nếu muốn hiển thị thêm thông tin
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
    }

    // 🔹 DTO tạo mới (chỉ cần thông tin thực tế khi trả xe)
    public class CarReturnHistoryCreateDTO
    {
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; } = string.Empty;

        // 🔗 chỉ cần OrderId là đủ
        public int OrderId { get; set; }
    }

    // 🔹 DTO cập nhật
    public class CarReturnHistoryUpdateDTO : CarReturnHistoryCreateDTO
    {
        public int Id { get; set; }
    }
}
