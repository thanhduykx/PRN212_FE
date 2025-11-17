using System;

namespace AssignmentPRN212.DTO
{
    public class FeedbackDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int RentalOrderId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CreateFeedbackDTO
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int RentalOrderId { get; set; }
    }

    public class UpdateFeedbackDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
    }
}

