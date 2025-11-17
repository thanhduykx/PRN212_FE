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
        public string Analysis { get; set; } = "";
        public object? Data { get; set; }
    }

    public class CarUsageAnalysisResponse
    {
        public string CarName { get; set; } = "";
        public int TotalRentals { get; set; }
        public double TotalRevenue { get; set; }
        public double AverageRentalDuration { get; set; }
        public string PeakHours { get; set; } = "";
    }
}

