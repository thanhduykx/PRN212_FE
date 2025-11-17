using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class RentalOrderService
    {
        private readonly ApiService _apiService;

        public RentalOrderService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/RentalOrder/GetAll - Lấy tất cả đơn hàng (Admin, Staff)
        public async Task<List<RentalOrderDTO>> GetAllAsync()
        {
            try
            {
                // Backend trả về format có IsSuccess
                try
                {
                    var response = await _apiService.GetAsync<ApiResponse<DataWrapper<RentalOrderDTO>>>("RentalOrder/GetAll");
                    if (response?.Data?.Values != null)
                        return response.Data.Values;
                }
                catch { }

                // Thử parse với CarResponse format
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<RentalOrderDTO>>("RentalOrder/GetAll");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values;
                }
                catch { }

                // Thử parse trực tiếp như List
                try
                {
                    var directResponse = await _apiService.GetAsync<List<RentalOrderDTO>>("RentalOrder/GetAll");
                    if (directResponse != null && directResponse.Count > 0)
                        return directResponse;
                }
                catch { }

                return new List<RentalOrderDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.Message}");
                return new List<RentalOrderDTO>();
            }
        }

        // GET /api/RentalOrder/GetById - Lấy đơn hàng theo Id
        public async Task<RentalOrderDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<RentalOrderDTO>>($"RentalOrder/GetById?id={id}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<RentalOrderDTO>($"RentalOrder/GetById?id={id}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // GET /api/RentalOrder/GetByUserId - Lấy đơn hàng theo UserId
        public async Task<List<RentalOrderDTO>> GetByUserIdAsync(int userId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<RentalOrderDTO>>>($"RentalOrder/GetByUserId?userId={userId}");
                return response?.Data?.Values ?? new List<RentalOrderDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<RentalOrderDTO>>($"RentalOrder/GetByUserId?userId={userId}");
                    return directResponse ?? new List<RentalOrderDTO>();
                }
                catch
                {
                    return new List<RentalOrderDTO>();
                }
            }
        }

        // POST /api/RentalOrder/Create - Tạo đơn hàng mới
        public async Task<RentalOrderDTO?> CreateAsync(CreateRentalOrderDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CreateRentalOrderDTO, ApiResponse<RentalOrderDTO>>("RentalOrder/Create", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CreateRentalOrderDTO, RentalOrderDTO>("RentalOrder/Create", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/RentalOrder/UpdateTotal - Cập nhật tổng tiền (ExtraFee, DamageFee, DamageNotes)
        public async Task<RentalOrderDTO?> UpdateTotalAsync(UpdateRentalOrderTotalDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateRentalOrderTotalDTO, ApiResponse<RentalOrderDTO>>("RentalOrder/UpdateTotal", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateRentalOrderTotalDTO, RentalOrderDTO>("RentalOrder/UpdateTotal", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/RentalOrder/ConfirmTotal - Xác nhận tổng tiền
        public async Task<bool> ConfirmTotalAsync(int orderId)
        {
            try
            {
                var response = await _apiService.PutAsync<object, ApiResponse<object>>($"RentalOrder/ConfirmTotal?orderId={orderId}", new { });
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // PUT /api/RentalOrder/ConfirmPayment - Xác nhận thanh toán
        public async Task<bool> ConfirmPaymentAsync(int orderId)
        {
            try
            {
                var response = await _apiService.PutAsync<object, ApiResponse<object>>($"RentalOrder/ConfirmPayment?orderId={orderId}", new { });
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // PUT /api/RentalOrder/UpdateStatus - Cập nhật status đơn hàng
        public async Task<RentalOrderDTO?> UpdateOrderStatusAsync(int orderId, RentalOrderStatus status)
        {
            try
            {
                var updateDto = new UpdateRentalOrderStatusDTO 
                { 
                    OrderId = orderId, 
                    Status = status 
                };
                
                string expectedStatus = status.ToString();
                bool apiCallSucceeded = false;
                
                // Thử với endpoint UpdateStatus - format ApiResponse<RentalOrderDTO>
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Calling UpdateStatus API: OrderId={orderId}, Status={status} ({expectedStatus})");
                    var response = await _apiService.PutAsync<UpdateRentalOrderStatusDTO, ApiResponse<RentalOrderDTO>>("RentalOrder/UpdateStatus", updateDto);
                    if (response?.Data != null)
                    {
                        apiCallSucceeded = true;
                        System.Diagnostics.Debug.WriteLine($"UpdateStatus API response received. Status: {response.Data.Status}");
                        // Verify status matches
                        if (string.Equals(response.Data.Status, expectedStatus, StringComparison.OrdinalIgnoreCase))
                        {
                            System.Diagnostics.Debug.WriteLine($"Status matches! Returning order.");
                            return response.Data;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Status mismatch! Expected: {expectedStatus}, Got: {response.Data.Status}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("UpdateStatus API response is null or Data is null");
                    }
                }
                catch (Exception ex1)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateStatus ApiResponse format failed: {ex1.GetType().Name} - {ex1.Message}\n{ex1.StackTrace}");
                }

                // Thử format trực tiếp RentalOrderDTO
                try
                {
                    System.Diagnostics.Debug.WriteLine("Retrying with direct RentalOrderDTO format...");
                    var directResponse = await _apiService.PutAsync<UpdateRentalOrderStatusDTO, RentalOrderDTO>("RentalOrder/UpdateStatus", updateDto);
                    if (directResponse != null)
                    {
                        apiCallSucceeded = true;
                        System.Diagnostics.Debug.WriteLine($"Direct format response received. Status: {directResponse.Status}");
                        // Verify status matches
                        if (string.Equals(directResponse.Status, expectedStatus, StringComparison.OrdinalIgnoreCase))
                        {
                            System.Diagnostics.Debug.WriteLine($"Status matches! Returning order.");
                            return directResponse;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Status mismatch! Expected: {expectedStatus}, Got: {directResponse.Status}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Direct format response is null");
                    }
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateStatus direct format failed: {ex2.GetType().Name} - {ex2.Message}\n{ex2.StackTrace}");
                }

                // Nếu API call thành công (không throw exception) nhưng parse fail
                // Hoặc nếu cả 2 format đều fail, thử reload order để kiểm tra
                // Retry ngay lập tức (không delay)
                for (int retry = 0; retry < 3; retry++)
                {
                    var reloadedOrder = await GetByIdAsync(orderId);
                    if (reloadedOrder != null)
                    {
                        // So sánh case-insensitive và trim whitespace
                        string reloadedStatus = reloadedOrder.Status?.Trim() ?? "";
                        if (string.Equals(reloadedStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
                        {
                            return reloadedOrder;
                        }
                    }
                }
                
                // Nếu không có endpoint UpdateStatus, thử dùng endpoint khác
                // Có thể backend dùng ConfirmTotal để chuyển sang Confirmed
                if (status == RentalOrderStatus.Confirmed && !apiCallSucceeded)
                {
                    var confirmResult = await ConfirmTotalAsync(orderId);
                    if (confirmResult)
                    {
                        // Reload order để lấy status mới
                        return await GetByIdAsync(orderId);
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateOrderStatusAsync error: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }
    }
}

