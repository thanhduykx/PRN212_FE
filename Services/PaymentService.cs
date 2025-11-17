using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class PaymentService
    {
        private readonly ApiService _apiService;

        public PaymentService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/Payment/GetAll - Lấy tất cả payment (Admin, Staff)
        public async Task<List<PaymentDTO>> GetAllAsync()
        {
            try
            {
                // Thử parse với CarResponse format
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<PaymentDTO>>("Payment/GetAll");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values;
                }
                catch { }

                // Thử parse với DataWrapper format
                try
                {
                    var dataWrapper = await _apiService.GetAsync<DataWrapper<PaymentDTO>>("Payment/GetAll");
                    if (dataWrapper?.Values != null && dataWrapper.Values.Count > 0)
                        return dataWrapper.Values;
                }
                catch { }

                // Thử parse với ApiResponse<DataWrapper> format
                try
                {
                    var response = await _apiService.GetAsync<ApiResponse<DataWrapper<PaymentDTO>>>("Payment/GetAll");
                    if (response?.Data?.Values != null && response.Data.Values.Count > 0)
                        return response.Data.Values;
                }
                catch { }

                // Thử parse trực tiếp như List
                try
                {
                    var directResponse = await _apiService.GetAsync<List<PaymentDTO>>("Payment/GetAll");
                    if (directResponse != null && directResponse.Count > 0)
                        return directResponse;
                }
                catch { }

                return new List<PaymentDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.Message}");
                return new List<PaymentDTO>();
            }
        }

        // GET /api/Payment/GetAllByUserId - Lấy payment theo UserId
        public async Task<List<PaymentDTO>> GetAllByUserIdAsync(int userId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<PaymentDTO>>>($"Payment/GetAllByUserId?userId={userId}");
                return response?.Data?.Values ?? new List<PaymentDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<PaymentDTO>>($"Payment/GetAllByUserId?userId={userId}");
                    return directResponse ?? new List<PaymentDTO>();
                }
                catch
                {
                    return new List<PaymentDTO>();
                }
            }
        }

        // GET /api/Payment/GetByPaymentId - Lấy payment theo Id
        public async Task<PaymentDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PaymentDTO>>($"Payment/GetByPaymentId?id={id}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<PaymentDTO>($"Payment/GetByPaymentId?id={id}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // POST /api/Payment/CreateFromOrder - Tạo payment mới
        public async Task<PaymentDTO?> CreateAsync(CreatePaymentDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CreatePaymentDTO, ApiResponse<PaymentDTO>>("Payment/CreateFromOrder", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CreatePaymentDTO, PaymentDTO>("Payment/CreateFromOrder", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/Payment/UpdatePaymentStatus - Cập nhật status payment
        public async Task<PaymentDTO?> UpdatePaymentStatusAsync(UpdatePaymentStatusDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdatePaymentStatusDTO, ApiResponse<PaymentDTO>>("Payment/UpdatePaymentStatus", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdatePaymentStatusDTO, PaymentDTO>("Payment/UpdatePaymentStatus", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/Payment/ConfirmDepositPayment - Xác nhận thanh toán cọc
        public async Task<bool> ConfirmDepositPaymentAsync(int orderId)
        {
            try
            {
                var response = await _apiService.PutAsync<object, ApiResponse<object>>($"Payment/ConfirmDepositPayment?orderId={orderId}", new { });
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // GET /api/Payment/GetDepositByOrderId - Lấy payment cọc theo OrderId (nếu có endpoint này)
        public async Task<PaymentDTO?> GetDepositByOrderIdAsync(int orderId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PaymentDTO>>($"Payment/GetDepositByOrderId?orderId={orderId}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<PaymentDTO>($"Payment/GetDepositByOrderId?orderId={orderId}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // GET /api/Payment/GetOrderPaymentByOrderId - Lấy payment đơn hàng theo OrderId (nếu có endpoint này)
        public async Task<PaymentDTO?> GetOrderPaymentByOrderIdAsync(int orderId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PaymentDTO>>($"Payment/GetOrderPaymentByOrderId?orderId={orderId}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<PaymentDTO>($"Payment/GetOrderPaymentByOrderId?orderId={orderId}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // GET /api/Payment/GetByRentalLocation - Lấy payment theo địa điểm (cho báo cáo doanh thu)
        public async Task<List<PaymentDTO>> GetByRentalLocationAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<PaymentDTO>>>("Payment/GetByRentalLocation");
                return response?.Data?.Values ?? new List<PaymentDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<PaymentDTO>>("Payment/GetByRentalLocation");
                    return directResponse ?? new List<PaymentDTO>();
                }
                catch
                {
                    return new List<PaymentDTO>();
                }
            }
        }
    }
}

