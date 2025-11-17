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
    public partial class CustomerActiveRentalWindow : Window
    {
        private readonly RentalOrderService _rentalOrderService;
        private readonly CarService _carService;
        private readonly CarReturnHistoryService _carReturnHistoryService;
        private readonly ApiService _apiService;
        private readonly int _userId;

        public ObservableCollection<RentalOrderDTO> RentalOrders { get; set; } = new ObservableCollection<RentalOrderDTO>();
        private List<RentalOrderDTO> _allOrders = new List<RentalOrderDTO>();
        private RentalOrderDTO _selectedOrder;
        private Dictionary<int, string> _carNamesCache = new Dictionary<int, string>();
        private Dictionary<int, CarDTO> _carsCache = new Dictionary<int, CarDTO>();

        public CustomerActiveRentalWindow(ApiService apiService, int userId)
        {
            InitializeComponent();
            _apiService = apiService;
            _rentalOrderService = new RentalOrderService(apiService);
            _carService = new CarService(apiService);
            _carReturnHistoryService = new CarReturnHistoryService(apiService);
            _userId = userId;
            RentalOrderDataGrid.ItemsSource = RentalOrders;
            Loaded += CustomerActiveRentalWindow_Loaded;
        }

        private async void CustomerActiveRentalWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            try
            {
                // Load orders của user này có status == "4" (Renting)
                var allOrders = await _rentalOrderService.GetByUserIdAsync(_userId);
                var rentingOrders = allOrders
                    .Where(o => o.Status == "4" || o.Status.Equals("Renting", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Load thông tin xe cho tất cả CarId
                var carIds = rentingOrders.Select(o => o.CarId).Distinct().ToList();
                await LoadCarNames(carIds);
                await LoadCars(carIds);

                // Gán tên xe và tính toán giá tiền vào từng order
                foreach (var order in rentingOrders)
                {
                    if (_carNamesCache.ContainsKey(order.CarId))
                    {
                        order.CarName = _carNamesCache[order.CarId];
                    }
                    else
                    {
                        order.CarName = $"Car #{order.CarId}";
                    }
                    
                    // Tính phí tài xế và đảm bảo SubTotal/Deposit được tính đúng
                    if (_carsCache.ContainsKey(order.CarId))
                    {
                        var car = _carsCache[order.CarId];
                        int days = (order.ExpectedReturnTime - order.PickupTime).Days + 1;
                        double pricePerDay = car.RentPricePerDay;
                        double pricePerDayWithDriver = car.RentPricePerDayWithDriver;
                        double driverFeePerDay = pricePerDayWithDriver - pricePerDay;
                        
                        // Tính phí tài xế tổng (nếu có tài xế) - chỉ để hiển thị
                        double totalDriverFee = order.WithDriver ? driverFeePerDay * days : 0;
                        order.DriverFeeText = $"{totalDriverFee:N0} VNĐ";
                        
                        // Kiểm tra nếu SubTotal hoặc Deposit từ backend là null/0 thì tính lại từ thông tin xe
                        // (Trường hợp backend không trả về giá trị)
                        if ((order.SubTotal == null || order.SubTotal == 0) || (order.Deposit == null || order.Deposit == 0))
                        {
                            // Tính lại SubTotal và Deposit từ thông tin xe
                            // SubTotal = (giá không tài xế * số ngày) + phí tài xế
                            order.SubTotal = (days * pricePerDay) + totalDriverFee;
                            
                            // Deposit = DepositAmount từ Car
                            order.Deposit = car.DepositAmount;
                            
                            System.Diagnostics.Debug.WriteLine($"Order #{order.Id}: Calculated from car - SubTotal = {order.SubTotal}, Deposit = {order.Deposit}, TotalText = {order.TotalText}");
                        }
                        else
                        {
                            // Sử dụng giá trị từ backend (đã được lưu khi đặt hàng)
                            System.Diagnostics.Debug.WriteLine($"Order #{order.Id}: Using backend values - SubTotal = {order.SubTotal}, Deposit = {order.Deposit}, TotalText = {order.TotalText}");
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

                _allOrders = rentingOrders.ToList();
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
                (order.CarName != null && order.CarName.ToLower().Contains(searchText))
            ).ToList();

            RentalOrders.Clear();
            foreach (var order in filtered)
                RentalOrders.Add(order);

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = RentalOrders.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void RentalOrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RentalOrderDataGrid.SelectedItem is RentalOrderDTO selectedOrder)
            {
                _selectedOrder = selectedOrder;
                ReturnCarButton.IsEnabled = true;
            }
            else
            {
                _selectedOrder = null;
                ReturnCarButton.IsEnabled = false;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }

        private async void ReturnCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần trả xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn trả xe cho đơn hàng #{_selectedOrder.Id}?\n\nTên xe: {_selectedOrder.CarName}",
                "Xác nhận trả xe",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ReturnCarButton.IsEnabled = false;
                    ReturnCarButton.Content = "Đang xử lý...";

                    // Customer không có quyền update status trực tiếp (403 Forbidden)
                    // Thử tạo CarReturnHistory trước, backend có thể tự động chuyển status
                    System.Diagnostics.Debug.WriteLine($"Attempting to return car for order #{_selectedOrder.Id}, current status: {_selectedOrder.Status}");
                    
                    // Tạo CarReturnHistory với thông tin tối thiểu
                    var returnHistory = new CarReturnHistoryCreateDTO
                    {
                        OrderId = _selectedOrder.Id,
                        ReturnDate = DateTime.Now,
                        OdometerEnd = 0, // Customer có thể không biết số km chính xác
                        BatteryLevelEnd = 0, // Customer có thể không biết pin chính xác
                        VehicleConditionEnd = "Đang chờ kiểm tra" // Staff sẽ kiểm tra sau
                    };

                    var createdHistory = await _carReturnHistoryService.CreateAsync(returnHistory);
                    
                    if (createdHistory != null)
                    {
                        // Sau khi tạo history, thử update status
                        var updatedOrder = await _rentalOrderService.UpdateOrderStatusAsync(_selectedOrder.Id, RentalOrderStatus.Returned);
                        
                        if (updatedOrder != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Return car successful. New status: {updatedOrder.Status}");
                            MessageBox.Show("Trả xe thành công! Đơn hàng đã được chuyển sang trạng thái 'Đã trả xe'.\n\nNhân viên sẽ kiểm tra xe và xác nhận thông tin trả xe.", 
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadOrders();
                        }
                        else
                        {
                            // History đã tạo nhưng status chưa update - có thể Staff cần xác nhận
                            MessageBox.Show("Yêu cầu trả xe đã được ghi nhận.\n\nNhân viên sẽ kiểm tra xe và xác nhận thông tin trả xe.", 
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadOrders();
                        }
                    }
                    else
                    {
                        // Nếu không tạo được history, thử update status trực tiếp
                        var updatedOrder = await _rentalOrderService.UpdateOrderStatusAsync(_selectedOrder.Id, RentalOrderStatus.Returned);
                        
                        if (updatedOrder != null)
                        {
                            MessageBox.Show("Trả xe thành công! Đơn hàng đã được chuyển sang trạng thái 'Đã trả xe'.", 
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadOrders();
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Return car failed: both history creation and status update failed");
                            MessageBox.Show("Trả xe thất bại. Bạn không có quyền tự trả xe.\n\nVui lòng liên hệ nhân viên để được hỗ trợ trả xe.", 
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException httpEx) when (httpEx.Message.Contains("403"))
                {
                    System.Diagnostics.Debug.WriteLine($"Return car 403 Forbidden: {httpEx.Message}");
                    MessageBox.Show("Bạn không có quyền tự trả xe.\n\nVui lòng liên hệ nhân viên để được hỗ trợ trả xe tại điểm thuê.", 
                        "Không có quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Return car exception: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                    MessageBox.Show($"Lỗi khi trả xe: {ex.Message}\n\nVui lòng liên hệ nhân viên để được hỗ trợ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    ReturnCarButton.IsEnabled = true;
                    ReturnCarButton.Content = "🚗 Trả xe";
                }
            }
        }
    }
}

