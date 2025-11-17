namespace AssignmentPRN212.DTO
{
    public class AIChatRequest
    {
        public string Message { get; set; } = "";
    }

    public class AIChatResponse
    {
        public string Response { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class AIAnalysisResponse
    {
        public string? Summary { get; set; }
        public string? Analysis { get; set; }
        public int? TotalOrders { get; set; }
        public double? TotalRevenue { get; set; }
        public int? TotalCars { get; set; }
        public int? TotalUsers { get; set; }
        public string? Insights { get; set; }
        public object? Data { get; set; }
    }

    public class CarUsageAnalysisResponse
    {
        public string CarName { get; set; } = "";
        public int RentalCount { get; set; }
        public int TotalRentals { get; set; }
        public double TotalRevenue { get; set; }
        public double AverageRentalDuration { get; set; }
        public double UsageRate { get; set; }
        public double AverageRating { get; set; }
        public string PeakHours { get; set; } = "";
        public string? Notes { get; set; }
    }
}

