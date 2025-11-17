using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class CarDeliveryHistoryListWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly CarService _carService;
        private readonly RentalOrderService _rentalOrderService;

        // ObservableCollection bind DataGrid - hiển thị orders status == 3
        public ObservableCollection<RentalOrderDTO> Orders { get; set; } = new ObservableCollection<RentalOrderDTO>();
        private List<RentalOrderDTO> _allOrders = new List<RentalOrderDTO>(); // Lưu danh sách gốc để filter
        private RentalOrderDTO? _selectedOrder;
        private Dictionary<int, string> _carNamesCache = new Dictionary<int, string>(); // Cache tên xe
        private Dictionary<int, CarDTO> _carsCache = new Dictionary<int, CarDTO>(); // Cache thông tin xe để tính phí tài xế

        public CarDeliveryHistoryListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _carService = new CarService(apiService);
            _rentalOrderService = new RentalOrderService(apiService);

            CarDeliveryHistoryDataGrid.ItemsSource = Orders;

            // Load dữ liệu
            this.Loaded += CarDeliveryHistoryListWindow_Loaded;
        }

        private async void CarDeliveryHistoryListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHistories();
        }

        private async Task LoadHistories()
        {
            try
            {
                // Hiển thị orders có status == "3" (Confirmed) - chờ giao xe
                var allOrders = await _rentalOrderService.GetAllAsync();
                var confirmedOrders = allOrders
                    .Where(o => o.Status == "3" || o.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                // Load thông tin xe cho tất cả CarId
                var carIds = confirmedOrders.Select(o => o.CarId).Distinct().ToList();
                await LoadCarNames(carIds);
                await LoadCars(carIds);
                
                // Gán tên xe và tính phí tài xế vào từng order (giống như RentalOrderListWindow)
                foreach (var order in confirmedOrders)
                {
                    if (_carNamesCache.ContainsKey(order.CarId))
                    {
                        order.CarName = _carNamesCache[order.CarId];
                    }
                    else
                    {
                        order.CarName = $"Car #{order.CarId}";
                    }
                    
                    // Tính lại SubTotal và Deposit nếu cần (giống như RentalOrderListWindow)
                    if ((order.SubTotal == null || order.SubTotal == 0) || (order.Deposit == null || order.Deposit == 0))
                    {
                        if (_carsCache.ContainsKey(order.CarId))
                        {
                            var car = _carsCache[order.CarId];
                            int days = (order.ExpectedReturnTime - order.PickupTime).Days + 1;
                            double pricePerDay = car.RentPricePerDay;
                            double pricePerDayWithDriver = car.RentPricePerDayWithDriver;
                            double driverFeePerDay = pricePerDayWithDriver - pricePerDay;
                            double totalDriverFee = order.WithDriver ? driverFeePerDay * days : 0;
                            
                            // Tính phí tài xế để hiển thị
                            order.DriverFeeText = $"{totalDriverFee:N0} VNĐ";
                            
                            if (order.SubTotal == null || order.SubTotal == 0)
                            {
                                // SubTotal = (giá không tài xế * số ngày) + phí tài xế
                                order.SubTotal = (days * pricePerDay) + totalDriverFee;
                            }
                            
                            if (order.Deposit == null || order.Deposit == 0)
                            {
                                // Deposit = DepositAmount từ Car (giống như RentalOrderListWindow)
                                order.Deposit = car.DepositAmount;
                                System.Diagnostics.Debug.WriteLine($"CarDeliveryHistory - Order #{order.Id}: Calculated Deposit from car = {order.Deposit}");
                            }
                        }
                        else
                        {
                            // Nếu không có thông tin xe, chỉ tính phí tài xế nếu có
                            if (order.WithDriver)
                            {
                                order.DriverFeeText = "N/A";
                            }
                            else
                            {
                                order.DriverFeeText = "0 VNĐ";
                            }
                        }
                    }
                    else
                    {
                        // Sử dụng giá trị từ backend (đã được lưu khi đặt hàng)
                        // Vẫn tính phí tài xế để hiển thị
                        if (_carsCache.ContainsKey(order.CarId))
                        {
                            var car = _carsCache[order.CarId];
                            int days = (order.ExpectedReturnTime - order.PickupTime).Days + 1;
                            double pricePerDay = car.RentPricePerDay;
                            double pricePerDayWithDriver = car.RentPricePerDayWithDriver;
                            double driverFeePerDay = pricePerDayWithDriver - pricePerDay;
                            double totalDriverFee = order.WithDriver ? driverFeePerDay * days : 0;
                            order.DriverFeeText = $"{totalDriverFee:N0} VNĐ";
                        }
                        else
                        {
                            if (order.WithDriver)
                            {
                                order.DriverFeeText = "N/A";
                            }
                            else
                            {
                                order.DriverFeeText = "0 VNĐ";
                            }
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"CarDeliveryHistory - Order #{order.Id}: Using backend Deposit = {order.Deposit}");
                    }
                }
                
                _allOrders = confirmedOrders.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadCarNames(List<int> carIds)
        {
            try
            {
                var allCars = await _carService.GetAllCarsAsync();
                foreach (var car in allCars)
                {
                    if (!_carNamesCache.ContainsKey(car.Id))
                    {
                        _carNamesCache[car.Id] = car.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading car names: {ex.Message}");
            }
        }

        private async Task LoadCars(List<int> carIds)
        {
            try
            {
                var allCars = await _carService.GetAllCarsAsync();
                foreach (var car in allCars)
                {
                    if (!_carsCache.ContainsKey(car.Id))
                    {
                        _carsCache[car.Id] = car;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cars: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";
            
            var filtered = _allOrders.Where(order =>
                string.IsNullOrWhiteSpace(searchText) ||
                order.Id.ToString().Contains(searchText) ||
                order.CarId.ToString().Contains(searchText) ||
                (order.CarName != null && order.CarName.ToLower().Contains(searchText)) ||
                order.PhoneNumber.ToLower().Contains(searchText) ||
                order.Status.ToLower().Contains(searchText)
            ).ToList();

            Orders.Clear();
            foreach (var order in filtered)
                Orders.Add(order);

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = Orders.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CarDeliveryHistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarDeliveryHistoryDataGrid.SelectedItem is RentalOrderDTO selectedOrder)
            {
                _selectedOrder = selectedOrder;
                DeliverCarButton.IsEnabled = true;
            }
            else
            {
                _selectedOrder = null;
                DeliverCarButton.IsEnabled = false;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadHistories();
        }

        private async void DeliverCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần giao xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn giao xe cho đơn hàng #{_selectedOrder.Id}?\n\nTên xe: {_selectedOrder.CarName}\n\nĐơn hàng sẽ được chuyển sang trạng thái 'Đang cho thuê'.",
                "Xác nhận giao xe",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DeliverCarButton.IsEnabled = false;
                    DeliverCarButton.Content = "Đang xử lý...";

                    System.Diagnostics.Debug.WriteLine($"Attempting to update order #{_selectedOrder.Id} to Renting status");

                    // Chuyển order status sang 4 (Renting)
                    var updatedOrder = await _rentalOrderService.UpdateOrderStatusAsync(_selectedOrder.Id, RentalOrderStatus.Renting);

                    if (updatedOrder != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Order #{_selectedOrder.Id} updated successfully. New status: {updatedOrder.Status}");
                        MessageBox.Show("Giao xe thành công! Đơn hàng đã được chuyển sang trạng thái 'Đang cho thuê'.", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadHistories();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Order #{_selectedOrder.Id} update returned null");
                        MessageBox.Show("Giao xe thất bại. Không thể cập nhật trạng thái đơn hàng. Vui lòng kiểm tra:\n" +
                            "- Kết nối mạng\n" +
                            "- Quyền truy cập\n" +
                            "- Trạng thái đơn hàng hiện tại", 
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (System.Net.Http.HttpRequestException httpEx)
                {
                    System.Diagnostics.Debug.WriteLine($"HttpRequestException: {httpEx.Message}\n{httpEx.StackTrace}");
                    
                    // Lấy status code từ exception data nếu có
                    int? statusCode = null;
                    if (httpEx.Data.Contains("StatusCode"))
                        statusCode = httpEx.Data["StatusCode"] as int?;
                    
                    string errorMessage = "Lỗi kết nối với server:\n\n";
                    
                    if (statusCode.HasValue)
                    {
                        switch (statusCode.Value)
                        {
                            case 404:
                                errorMessage += "API endpoint không tồn tại.\nVui lòng kiểm tra cấu hình backend.";
                                break;
                            case 401:
                                errorMessage += "Chưa đăng nhập hoặc token hết hạn.\nVui lòng đăng nhập lại.";
                                break;
                            case 403:
                                errorMessage += "Không có quyền thực hiện thao tác này.\nVui lòng kiểm tra quyền truy cập.";
                                break;
                            case 400:
                                errorMessage += "Dữ liệu không hợp lệ.\n" + (httpEx.Data.Contains("ErrorContent") ? httpEx.Data["ErrorContent"]?.ToString() : "");
                                break;
                            case 500:
                                errorMessage += "Lỗi server.\nVui lòng thử lại sau hoặc liên hệ quản trị viên.";
                                break;
                            default:
                                errorMessage += $"HTTP {statusCode.Value}: {httpEx.Message}";
                                break;
                        }
                    }
                    else if (httpEx.Message.Contains("404"))
                    {
                        errorMessage += "API endpoint không tồn tại. Vui lòng kiểm tra cấu hình.";
                    }
                    else if (httpEx.Message.Contains("401") || httpEx.Message.Contains("403"))
                    {
                        errorMessage += "Không có quyền thực hiện thao tác này. Vui lòng đăng nhập lại.";
                    }
                    else if (httpEx.Message.Contains("500"))
                    {
                        errorMessage += "Lỗi server. Vui lòng thử lại sau.";
                    }
                    else
                    {
                        errorMessage += httpEx.Message;
                    }
                    
                    MessageBox.Show(errorMessage, "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in DeliverCarButton_Click: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                    MessageBox.Show($"Lỗi: {ex.Message}\n\nChi tiết: {ex.GetType().Name}", 
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DeliverCarButton.IsEnabled = true;
                    DeliverCarButton.Content = "🚗 Giao xe";
                }
            }
        }
    }
}

