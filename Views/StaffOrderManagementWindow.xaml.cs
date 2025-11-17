using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class StaffOrderManagementWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly RentalOrderService _rentalOrderService;
        private readonly CarService _carService;
        private RentalOrderDTO? _selectedOrder;
        public ObservableCollection<RentalOrderDTO> Orders { get; set; } = new ObservableCollection<RentalOrderDTO>();
        private System.Collections.Generic.List<RentalOrderDTO> _allOrders = new System.Collections.Generic.List<RentalOrderDTO>();
        private System.Collections.Generic.Dictionary<int, CarDTO> _carsCache = new System.Collections.Generic.Dictionary<int, CarDTO>();

        public StaffOrderManagementWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _rentalOrderService = new RentalOrderService(_apiService);
            _carService = new CarService(_apiService);
            OrdersDataGrid.ItemsSource = Orders;
            
            // Đợi window load xong mới gọi LoadOrders
            this.Loaded += StaffOrderManagementWindow_Loaded;
        }

        private async void StaffOrderManagementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            try
            {
                var orders = await _rentalOrderService.GetAllAsync();
                
                // Load thông tin xe cho tất cả CarId
                var carIds = orders.Select(o => o.CarId).Distinct().ToList();
                await LoadCars(carIds);
                
                // Tính lại SubTotal và Deposit nếu cần (giống như bên Giao xe)
                foreach (var order in orders)
                {
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
                            
                            if (order.SubTotal == null || order.SubTotal == 0)
                            {
                                // SubTotal = (giá không tài xế * số ngày) + phí tài xế
                                order.SubTotal = (days * pricePerDay) + totalDriverFee;
                            }
                            
                            if (order.Deposit == null || order.Deposit == 0)
                            {
                                // Deposit = DepositAmount từ Car (giống như bên Giao xe)
                                order.Deposit = car.DepositAmount;
                                System.Diagnostics.Debug.WriteLine($"StaffOrderManagement - Order #{order.Id}: Calculated Deposit from car = {order.Deposit}");
                            }
                        }
                    }
                    else
                    {
                        // Sử dụng giá trị từ backend (đã được lưu khi đặt hàng)
                        System.Diagnostics.Debug.WriteLine($"StaffOrderManagement - Order #{order.Id}: Using backend Deposit = {order.Deposit}");
                    }
                }
                
                _allOrders = orders.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadCars(System.Collections.Generic.List<int> carIds)
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
            string selectedStatus = (StatusFilterComboBox?.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Tất cả";
            
            var filtered = _allOrders.Where(order =>
                (string.IsNullOrWhiteSpace(searchText) ||
                 order.PhoneNumber.ToLower().Contains(searchText) ||
                 order.Id.ToString().Contains(searchText)) &&
                (selectedStatus == "Tất cả" || order.Status == selectedStatus)
            ).OrderByDescending(o => o.OrderDate).ToList();

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

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is RentalOrderDTO selectedOrder)
            {
                _selectedOrder = selectedOrder;
                SelectedOrderInfoTextBlock.Text = $"Đơn #{selectedOrder.Id} - {selectedOrder.PhoneNumber} - {selectedOrder.Status}";
                
                // Set status trong ComboBox
                if (StatusUpdateComboBox != null)
                {
                    foreach (ComboBoxItem item in StatusUpdateComboBox.Items)
                    {
                        if (item.Tag?.ToString() == selectedOrder.Status)
                        {
                            StatusUpdateComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private async void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần cập nhật trạng thái.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StatusUpdateComboBox?.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái mới.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedItem = StatusUpdateComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem?.Tag == null)
            {
                MessageBox.Show("Trạng thái không hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string statusString = selectedItem.Tag.ToString();
            if (!Enum.TryParse<RentalOrderStatus>(statusString, out var newStatus))
            {
                MessageBox.Show("Trạng thái không hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedOrder.Status == statusString)
            {
                MessageBox.Show("Đơn hàng đã ở trạng thái này rồi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn cập nhật trạng thái đơn hàng #{_selectedOrder.Id} từ '{_selectedOrder.Status}' sang '{statusString}'?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Lưu status ban đầu để so sánh
                    string oldStatus = _selectedOrder.Status;
                    
                    // Gọi API update
                    var updatedOrder = await _rentalOrderService.UpdateOrderStatusAsync(_selectedOrder.Id, newStatus);
                    
                    // Reload orders để cập nhật UI
                    await LoadOrders();
                    
                    // Reload order cụ thể để kiểm tra status
                    var reloadedOrder = await _rentalOrderService.GetByIdAsync(_selectedOrder.Id);
                    
                    // Kiểm tra kết quả: So sánh case-insensitive
                    bool isSuccess = false;
                    if (reloadedOrder != null)
                    {
                        string reloadedStatus = reloadedOrder.Status?.Trim() ?? "";
                        isSuccess = string.Equals(reloadedStatus, statusString, StringComparison.OrdinalIgnoreCase);
                    }
                    else if (updatedOrder != null)
                    {
                        string updatedStatus = updatedOrder.Status?.Trim() ?? "";
                        isSuccess = string.Equals(updatedStatus, statusString, StringComparison.OrdinalIgnoreCase);
                    }
                    
                    if (isSuccess)
                    {
                        MessageBox.Show($"Đã cập nhật trạng thái đơn hàng #{_selectedOrder.Id} từ '{oldStatus}' sang '{statusString}' thành công!", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        // Kiểm tra xem có thay đổi gì không (có thể status khác với mong đợi nhưng vẫn thay đổi)
                        if (reloadedOrder != null && !string.Equals(reloadedOrder.Status?.Trim(), oldStatus, StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show($"Trạng thái đơn hàng #{_selectedOrder.Id} đã được cập nhật thành '{reloadedOrder.Status}' (khác với trạng thái mong đợi '{statusString}').", 
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Không thể cập nhật trạng thái đơn hàng. Vui lòng thử lại hoặc kiểm tra lại sau.", 
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Vẫn reload để đảm bảo UI được cập nhật
                    await LoadOrders();
                }
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }
    }
}

