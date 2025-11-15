using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class StaffService
    {
        private readonly ApiService _api;
        public StaffService(ApiService api)
        {
            _api = api;
        }

        #region Rental Orders

        /// <summary>
        /// Get all rental orders
        /// API: GET /api/RentalOrder/GetAll
        /// </summary>
        public async Task<List<RentalOrderDTO>> GetAllOrdersAsync()
        {
            var response = await _api.GetAsync<ApiResponse<DataWrapper<RentalOrderDTO>>>("RentalOrder/GetAll");
            return response?.Data?.Values ?? new List<RentalOrderDTO>();
        }

        /// <summary>
        /// Get rental order by ID
        /// API: GET /api/RentalOrder/GetById?id={id}
        /// </summary>
        public async Task<RentalOrderDTO?> GetOrderByIdAsync(int orderId)
        {
            var response = await _api.GetAsync<ApiResponse<RentalOrderDTO>>($"RentalOrder/GetById?id={orderId}");
            return response?.Data;
        }

        #endregion

        #region Document Verification

        /// <summary>
        /// Get Citizen ID by order ID
        /// API: GET /api/CitizenId/GetByOrderId?orderId={orderId}
        /// </summary>
        public async Task<CitizenIdDTO?> GetCitizenIdByOrderAsync(int orderId)
        {
            var response = await _api.GetAsync<ApiResponse<CitizenIdDTO>>($"CitizenId/GetByOrderId?orderId={orderId}");
            return response?.Data;
        }

        /// <summary>
        /// Get Driver License by order ID
        /// API: GET /api/DriverLicense/GetByOrderId?orderId={orderId}
        /// </summary>
        public async Task<DriverLicenseDTO?> GetDriverLicenseByOrderAsync(int orderId)
        {
            var response = await _api.GetAsync<ApiResponse<DriverLicenseDTO>>($"DriverLicense/GetByOrderId?orderId={orderId}");
            return response?.Data;
        }

        #endregion

        #region Vehicle Handover

        /// <summary>
        /// Create vehicle delivery/handover record
        /// API: POST /api/CarDeliveryHistory
        /// </summary>
        public async Task<CarDeliveryHistoryDTO?> CreateDeliveryHistoryAsync(CreateCarDeliveryHistoryDTO dto)
        {
            var response = await _api.PostAsync<CreateCarDeliveryHistoryDTO, ApiResponse<CarDeliveryHistoryDTO>>(
                "CarDeliveryHistory", dto);
            return response?.Data;
        }

        #endregion

        #region Vehicle Return

        /// <summary>
        /// Create vehicle return record
        /// API: POST /api/CarReturnHistory
        /// </summary>
        public async Task<CarReturnHistoryDTO?> CreateReturnHistoryAsync(CreateCarReturnHistoryDTO dto)
        {
            var response = await _api.PostAsync<CreateCarReturnHistoryDTO, ApiResponse<CarReturnHistoryDTO>>(
                "CarReturnHistory", dto);
            return response?.Data;
        }

        #endregion

        #region Payment

        /// <summary>
        /// Create payment record
        /// API: POST /api/Payment/Create
        /// </summary>
        public async Task<PaymentDTO?> CreatePaymentAsync(CreatePaymentDTO dto)
        {
            var response = await _api.PostAsync<CreatePaymentDTO, ApiResponse<PaymentDTO>>(
                "Payment/Create", dto);
            return response?.Data;
        }

        #endregion

        #region Car Rental Location

        /// <summary>
        /// Get vehicles at a specific rental location
        /// API: GET /api/CarRentalLocation/GetByRentalLocationId?locationId={locationId}
        /// </summary>
        public async Task<List<CarRentalLocationDTO>> GetVehiclesByLocationAsync(int locationId)
        {
            var response = await _api.GetAsync<ApiResponse<DataWrapper<CarRentalLocationDTO>>>(
                $"CarRentalLocation/GetByRentalLocationId?locationId={locationId}");
            return response?.Data?.Values ?? new List<CarRentalLocationDTO>();
        }

        #endregion
    }
}

