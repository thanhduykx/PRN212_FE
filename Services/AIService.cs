using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class AIService
    {
        private readonly ApiService _apiService;

        public AIService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/AI/analyze - Phân tích dữ liệu
        public async Task<AIAnalysisResponse?> AnalyzeAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<AIAnalysisResponse>>("AI/analyze");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<AIAnalysisResponse>("AI/analyze");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // GET /api/AI/car-usage - Phân tích sử dụng xe
        public async Task<List<CarUsageAnalysisResponse>> GetCarUsageAnalysisAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarUsageAnalysisResponse>>>("AI/car-usage");
                return response?.Data?.Values ?? new List<CarUsageAnalysisResponse>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<CarUsageAnalysisResponse>>("AI/car-usage");
                    return directResponse ?? new List<CarUsageAnalysisResponse>();
                }
                catch
                {
                    return new List<CarUsageAnalysisResponse>();
                }
            }
        }

        // POST /api/AI/chat - Chat với AI
        public async Task<AIChatResponse?> ChatAsync(string message)
        {
            try
            {
                var request = new AIChatRequest { Message = message };
                var response = await _apiService.PostAsync<AIChatRequest, ApiResponse<AIChatResponse>>("AI/chat", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var request = new AIChatRequest { Message = message };
                    var directResponse = await _apiService.PostAsync<AIChatRequest, AIChatResponse>("AI/chat", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}

