using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CarService
    {
        private readonly ApiService _apiService;

        public CarService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET danh sách xe - Sử dụng ApiResponse format
        public async Task<List<CarDTO>> GetAllCarsAsync()
        {
            try
            {
                // Sử dụng ApiResponse<DataWrapper> format
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarDTO>>>("Car");
                if (response?.Data?.Values != null && response.Data.Values.Count > 0)
                    return response.Data.Values;

                // Fallback: ApiResponse<List>
                var listResponse = await _apiService.GetAsync<ApiResponse<List<CarDTO>>>("Car");
                if (listResponse?.Data != null && listResponse.Data.Count > 0)
                    return listResponse.Data;

                return new List<CarDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllCarsAsync error: {ex.Message}");
                return new List<CarDTO>();
            }
        }

        // POST thêm xe - Sử dụng CreateCarDTO (không có Id, có empty arrays cho navigation properties)
        public async Task<CarDTO?> AddCarAsync(CarDTO car, int? locationId = null)
        {
            try
            {
                // Tạo CreateCarDTO với empty arrays cho navigation properties
                var carRequest = new CreateCarDTO
                {
                    Model = car.Model ?? "",
                    Name = car.Name ?? "",
                    Seats = car.Seats,
                    SizeType = car.SizeType ?? "",
                    TrunkCapacity = car.TrunkCapacity,
                    BatteryType = car.BatteryType ?? "",
                    BatteryDuration = car.BatteryDuration,
                    RentPricePerDay = car.RentPricePerDay,
                    RentPricePerHour = car.RentPricePerHour,
                    RentPricePerDayWithDriver = car.RentPricePerDayWithDriver,
                    RentPricePerHourWithDriver = car.RentPricePerHourWithDriver,
                    DepositAmount = car.DepositAmount, // Số tiền đặt cọc (required by database)
                    ImageUrl = car.ImageUrl ?? "",
                    ImageUrl2 = car.ImageUrl2 ?? "",
                    ImageUrl3 = car.ImageUrl3 ?? "",
                    Status = car.Status,
                    IsActive = car.IsActive,
                    IsDeleted = false,
                    // Backend yêu cầu các field này phải có - gửi empty arrays
                    CarRentalLocations = new List<object>(),
                    RentalOrders = new List<object>()
                };

                // Sử dụng ApiResponse format
                var response = await _apiService.PostAsync<CreateCarDTO, ApiResponse<CarDTO>>("Car", carRequest);
                if (response?.Data != null && response.Data.Id > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Car created successfully: Id={response.Data.Id}, Name={response.Data.Name}");
                    // CarRentalLocation sẽ được tạo ở View layer sau khi car được tạo thành công
                    // (giống như UpdateCarRentalLocations)
                    return response.Data;
                }
                
                System.Diagnostics.Debug.WriteLine($"WARNING: AddCarAsync returned null or invalid response. Response: {(response != null ? "not null" : "null")}, Data: {(response?.Data != null ? $"Id={response.Data.Id}" : "null")}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddCarAsync error: {ex.Message}");
                throw; // Re-throw để hiển thị error message chi tiết
            }
        }

        // PUT cập nhật xe - Sử dụng UpdateCarDTO với Id trong body (backend yêu cầu id trong URL phải khớp với body)
        public async Task<CarDTO?> UpdateCarAsync(CarDTO car)
        {
            try
            {
                // Backend nhận Car entity với Id trong body (id trong URL phải khớp với body)
                var carRequest = new UpdateCarDTO
                {
                    Id = car.Id, // Backend yêu cầu Id trong body phải khớp với URL
                    Model = car.Model ?? "",
                    Name = car.Name ?? "",
                    Seats = car.Seats,
                    SizeType = car.SizeType ?? "",
                    TrunkCapacity = car.TrunkCapacity,
                    BatteryType = car.BatteryType ?? "",
                    BatteryDuration = car.BatteryDuration,
                    RentPricePerDay = car.RentPricePerDay,
                    RentPricePerHour = car.RentPricePerHour,
                    RentPricePerDayWithDriver = car.RentPricePerDayWithDriver,
                    RentPricePerHourWithDriver = car.RentPricePerHourWithDriver,
                    DepositAmount = car.DepositAmount, // Số tiền đặt cọc (required by database)
                    ImageUrl = car.ImageUrl ?? "",
                    ImageUrl2 = car.ImageUrl2 ?? "",
                    ImageUrl3 = car.ImageUrl3 ?? "",
                    Status = car.Status,
                    IsActive = car.IsActive,
                    IsDeleted = car.IsDeleted,
                    // Backend yêu cầu các field này phải có - gửi empty arrays
                    CarRentalLocations = new List<object>(),
                    RentalOrders = new List<object>()
                };

                // Sử dụng ApiResponse format
                var response = await _apiService.PutAsync<UpdateCarDTO, ApiResponse<CarDTO>>($"Car/{car.Id}", carRequest);
                if (response?.Data != null && response.Data.Id == car.Id)
                    return response.Data;
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateCarAsync error: {ex.Message}");
                throw; // Re-throw để hiển thị error message chi tiết
            }
        }

        // DELETE xe
        public async Task<bool> DeleteCarAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"Car/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // GET /api/Car/byName/{name} - Lấy xe theo tên
        public async Task<List<CarDTO>> GetCarsByNameAsync(string name)
        {
            try
            {
                // Thử parse với CarResponse format
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<CarDTO>>($"Car/byName/{System.Net.WebUtility.UrlEncode(name)}");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values;
                }
                catch { }

                // Thử parse với DataWrapper format
                try
                {
                    var dataWrapper = await _apiService.GetAsync<DataWrapper<CarDTO>>($"Car/byName/{System.Net.WebUtility.UrlEncode(name)}");
                    if (dataWrapper?.Values != null && dataWrapper.Values.Count > 0)
                        return dataWrapper.Values;
                }
                catch { }

                // Thử parse trực tiếp như List
                try
                {
                    var directResponse = await _apiService.GetAsync<List<CarDTO>>($"Car/byName/{System.Net.WebUtility.UrlEncode(name)}");
                    if (directResponse != null && directResponse.Count > 0)
                        return directResponse;
                }
                catch { }

                return new List<CarDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCarsByNameAsync error: {ex.Message}");
                return new List<CarDTO>();
            }
        }

        // GET /api/Car/TopRented - Lấy top xe được thuê nhiều nhất
        public async Task<List<CarDTO>> GetTopRentedCarsAsync(int topCount = 10)
        {
            try
            {
                // Thử parse với CarResponse format
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<CarDTO>>($"Car/TopRented?topCount={topCount}");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values;
                }
                catch { }

                // Thử parse với DataWrapper format
                try
                {
                    var dataWrapper = await _apiService.GetAsync<DataWrapper<CarDTO>>($"Car/TopRented?topCount={topCount}");
                    if (dataWrapper?.Values != null && dataWrapper.Values.Count > 0)
                        return dataWrapper.Values;
                }
                catch { }

                // Thử parse trực tiếp như List
                try
                {
                    var directResponse = await _apiService.GetAsync<List<CarDTO>>($"Car/TopRented?topCount={topCount}");
                    if (directResponse != null && directResponse.Count > 0)
                        return directResponse;
                }
                catch { }

                // Fallback: thử không có parameter (nếu backend không hỗ trợ topCount)
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<CarDTO>>("Car/TopRented");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values.Take(topCount).ToList();
                }
                catch { }

                return new List<CarDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTopRentedCarsAsync error: {ex.Message}");
                return new List<CarDTO>();
            }
        }

        // GET /api/Car/paged - Lấy xe có phân trang
        public async Task<PagedCarResult> GetPagedCarsAsync(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PagedCarResult>>($"Car/paged?pageIndex={pageIndex}&pageSize={pageSize}");
                return response?.Data ?? new PagedCarResult();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<PagedCarResult>($"Car/paged?pageIndex={pageIndex}&pageSize={pageSize}");
                    return directResponse ?? new PagedCarResult();
                }
                catch
                {
                    return new PagedCarResult();
                }
            }
        }

        // GET /api/Car/{id} - Lấy xe theo ID (lấy từ danh sách vì backend không có endpoint riêng)
        public async Task<CarDTO?> GetCarByIdAsync(int id)
        {
            try
            {
                var allCars = await GetAllCarsAsync();
                return allCars.FirstOrDefault(c => c.Id == id);
            }
            catch
            {
                return null;
            }
        }
    }

    // Helper class cho PagedCarResult
    public class PagedCarResult
    {
        public int Total { get; set; }
        public List<CarDTO> Data { get; set; } = new List<CarDTO>();
    }
}
