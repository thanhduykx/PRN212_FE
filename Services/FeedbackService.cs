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
                // Thử parse với SuccessResponse format (isSuccess, message, data với $values)
                try
                {
                    var successResponse = await _apiService.GetAsync<SuccessResponse<DataWrapper<FeedbackDTO>>>($"Feedback/byCar/{System.Net.WebUtility.UrlEncode(carName)}");
                    if (successResponse?.Data?.Values != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"GetByCarNameAsync: Loaded {successResponse.Data.Values.Count} feedbacks using SuccessResponse format");
                        return successResponse.Data.Values;
                    }
                }
                catch (Exception ex1)
                {
                    System.Diagnostics.Debug.WriteLine($"GetByCarNameAsync - SuccessResponse format failed: {ex1.Message}");
                }

                // Fallback: Thử ApiResponse format
                try
                {
                    var response = await _apiService.GetAsync<ApiResponse<DataWrapper<FeedbackDTO>>>($"Feedback/byCar/{System.Net.WebUtility.UrlEncode(carName)}");
                    if (response?.Data?.Values != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"GetByCarNameAsync: Loaded {response.Data.Values.Count} feedbacks using ApiResponse format");
                        return response.Data.Values;
                    }
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"GetByCarNameAsync - ApiResponse format failed: {ex2.Message}");
                }

                // Fallback: Thử direct List format
                try
                {
                    var directResponse = await _apiService.GetAsync<List<FeedbackDTO>>($"Feedback/byCar/{System.Net.WebUtility.UrlEncode(carName)}");
                    if (directResponse != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"GetByCarNameAsync: Loaded {directResponse.Count} feedbacks using direct List format");
                        return directResponse;
                    }
                }
                catch (Exception ex3)
                {
                    System.Diagnostics.Debug.WriteLine($"GetByCarNameAsync - Direct List format failed: {ex3.Message}");
                }

                return new List<FeedbackDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetByCarNameAsync error: {ex.Message}");
                return new List<FeedbackDTO>();
            }
        }

        // POST /api/Feedback/Create - Tạo feedback mới
        public async Task<FeedbackDTO?> CreateAsync(CreateFeedbackDTO request)
        {
            try
            {
                // Backend trả về Ok(result.Data) nên có thể là ApiResponse<FeedbackDTO> hoặc FeedbackDTO trực tiếp
                // Thử ApiResponse format trước
                try
                {
                    var response = await _apiService.PostAsync<CreateFeedbackDTO, ApiResponse<FeedbackDTO>>("Feedback/Create", request);
                    if (response?.Data != null)
                        return response.Data;
                }
                catch (Exception ex1)
                {
                    System.Diagnostics.Debug.WriteLine($"Feedback CreateAsync - ApiResponse format failed: {ex1.Message}");
                    // Fallback: Thử direct FeedbackDTO
                    try
                    {
                        var directResponse = await _apiService.PostAsync<CreateFeedbackDTO, FeedbackDTO>("Feedback/Create", request);
                        if (directResponse != null)
                            return directResponse;
                    }
                    catch (Exception ex2)
                    {
                        System.Diagnostics.Debug.WriteLine($"Feedback CreateAsync - Direct format failed: {ex2.Message}");
                        throw; // Re-throw để caller có thể xử lý
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Feedback CreateAsync error: {ex.Message}");
                throw; // Re-throw để caller có thể hiển thị lỗi chi tiết
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

