using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class FeedbackService
    {
        private readonly ApiService _apiService;

        public FeedbackService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/Feedback - Lấy tất cả feedback
        public async Task<List<FeedbackDTO>> GetAllAsync()
        {
            try
            {
                // Thử parse với CarResponse format
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<FeedbackDTO>>("Feedback");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values;
                }
                catch { }

                // Thử parse với DataWrapper format
                try
                {
                    var dataWrapper = await _apiService.GetAsync<DataWrapper<FeedbackDTO>>("Feedback");
                    if (dataWrapper?.Values != null && dataWrapper.Values.Count > 0)
                        return dataWrapper.Values;
                }
                catch { }

                // Thử parse với ApiResponse<DataWrapper> format
                try
                {
                    var response = await _apiService.GetAsync<ApiResponse<DataWrapper<FeedbackDTO>>>("Feedback");
                    if (response?.Data?.Values != null && response.Data.Values.Count > 0)
                        return response.Data.Values;
                }
                catch { }

                // Thử parse trực tiếp như List
                try
                {
                    var directResponse = await _apiService.GetAsync<List<FeedbackDTO>>("Feedback");
                    if (directResponse != null && directResponse.Count > 0)
                        return directResponse;
                }
                catch { }

                return new List<FeedbackDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.Message}");
                return new List<FeedbackDTO>();
            }
        }

        // GET /api/Feedback/paged - Lấy feedback có phân trang
        public async Task<PagedFeedbackResult> GetPagedAsync(int pageIndex = 1, int pageSize = 5, string? keyword = null)
        {
            try
            {
                string url = $"Feedback/paged?pageIndex={pageIndex}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(keyword))
                    url += $"&keyword={System.Net.WebUtility.UrlEncode(keyword)}";

                var response = await _apiService.GetAsync<ApiResponse<PagedFeedbackResult>>(url);
                return response?.Data ?? new PagedFeedbackResult();
            }
            catch
            {
                try
                {
                    string url = $"Feedback/paged?pageIndex={pageIndex}&pageSize={pageSize}";
                    if (!string.IsNullOrWhiteSpace(keyword))
                        url += $"&keyword={System.Net.WebUtility.UrlEncode(keyword)}";

                    var directResponse = await _apiService.GetAsync<PagedFeedbackResult>(url);
                    return directResponse ?? new PagedFeedbackResult();
                }
                catch
                {
                    return new PagedFeedbackResult();
                }
            }
        }

        // GET /api/Feedback/byCar/{carName} - Lấy feedback theo tên xe
        public async Task<List<FeedbackDTO>> GetByCarNameAsync(string carName)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<FeedbackDTO>>>($"Feedback/byCar/{System.Net.WebUtility.UrlEncode(carName)}");
                return response?.Data?.Values ?? new List<FeedbackDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<FeedbackDTO>>($"Feedback/byCar/{System.Net.WebUtility.UrlEncode(carName)}");
                    return directResponse ?? new List<FeedbackDTO>();
                }
                catch
                {
                    return new List<FeedbackDTO>();
                }
            }
        }

        // POST /api/Feedback - Tạo feedback mới
        public async Task<FeedbackDTO?> CreateAsync(CreateFeedbackDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CreateFeedbackDTO, ApiResponse<FeedbackDTO>>("Feedback", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CreateFeedbackDTO, FeedbackDTO>("Feedback", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/Feedback/{id} - Cập nhật feedback
        public async Task<FeedbackDTO?> UpdateAsync(UpdateFeedbackDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateFeedbackDTO, ApiResponse<FeedbackDTO>>($"Feedback/{request.Id}", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateFeedbackDTO, FeedbackDTO>($"Feedback/{request.Id}", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // DELETE /api/Feedback/{id} - Xóa feedback
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"Feedback/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }
    }

    // Helper class cho PagedFeedbackResult
    public class PagedFeedbackResult
    {
        public int Total { get; set; }
        public List<FeedbackDTO> Data { get; set; } = new List<FeedbackDTO>();
    }
}

