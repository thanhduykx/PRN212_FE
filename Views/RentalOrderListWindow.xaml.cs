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
    public partial class RentalOrderListWindow : Window
    {
        private readonly RentalOrderService _rentalOrderService;
        private readonly ApiService _apiService;
        private readonly CarService _carService;

        // ObservableCollection bind DataGrid
        public ObservableCollection<RentalOrderDTO> RentalOrders { get; set; } = new ObservableCollection<RentalOrderDTO>();
        private List<RentalOrderDTO> _allOrders = new List<RentalOrderDTO>(); // Lưu danh sách gốc để filter
        private RentalOrderDTO _selectedOrder;
        private Dictionary<int, CarDTO> _carsCache = new Dictionary<int, CarDTO>();

        public RentalOrderListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _rentalOrderService = new RentalOrderService(apiService);
            _carService = new CarService(apiService);

            RentalOrderDataGrid.ItemsSource = RentalOrders;

            // Set default dates
            PickupDatePicker.SelectedDate = DateTime.Now.AddDays(1);
            ReturnDatePicker.SelectedDate = DateTime.Now.AddDays(2);

            // Load dữ liệu
            LoadOrders();
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
                                System.Diagnostics.Debug.WriteLine($"RentalOrderList - Order #{order.Id}: Calculated Deposit from car = {order.Deposit}");
                            }
                        }
                    }
                    else
                    {
                        // Sử dụng giá trị từ backend (đã được lưu khi đặt hàng)
                        System.Diagnostics.Debug.WriteLine($"RentalOrderList - Order #{order.Id}: Using backend Deposit = {order.Deposit}");
                    }
                }
                
                _allOrders = orders.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                order.PhoneNumber.ToLower().Contains(searchText) ||
                order.Status.ToLower().Contains(searchText) ||
                order.UserId.ToString().Contains(searchText) ||
                order.CarId.ToString().Contains(searchText) ||
                order.Id.ToString().Contains(searchText)
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

                // Populate update fields
                ExtraFeeTextBox.Text = selectedOrder.ExtraFee?.ToString() ?? "0";
                DamageFeeTextBox.Text = selectedOrder.DamageFee?.ToString() ?? "0";
                DamageNotesTextBox.Text = selectedOrder.DamageNotes ?? "";

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

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
                {
                    MessageBox.Show("Số điện thoại không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!PickupDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày lấy xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ReturnDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày trả xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (PickupDatePicker.SelectedDate.Value >= ReturnDatePicker.SelectedDate.Value)
                {
                    MessageBox.Show("Ngày trả xe phải sau ngày lấy xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(UserIdTextBox.Text, out int userId) || userId <= 0)
                {
                    MessageBox.Show("User ID phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(CarIdTextBox.Text, out int carId) || carId <= 0)
                {
                    MessageBox.Show("Car ID phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(LocationIdTextBox.Text, out int locationId) || locationId <= 0)
                {
                    MessageBox.Show("Location ID phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lấy thông tin xe để tính giá
                var car = await _carService.GetCarByIdAsync(carId);
                if (car == null)
                {
                    MessageBox.Show("Không tìm thấy xe với ID này.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tính toán giá tiền
                int days = (ReturnDatePicker.SelectedDate.Value - PickupDatePicker.SelectedDate.Value).Days + 1;
                double pricePerDay = car.RentPricePerDay;
                double pricePerDayWithDriver = car.RentPricePerDayWithDriver;
                double driverFeePerDay = pricePerDayWithDriver - pricePerDay;
                
                // Tính phí tài xế tổng (nếu có tài xế)
                double totalDriverFee = (WithDriverCheckBox.IsChecked == true) ? driverFeePerDay * days : 0;
                
                // SubTotal = (giá không tài xế * số ngày) + phí tài xế
                double subTotal = (days * pricePerDay) + totalDriverFee;
                
                // Deposit = DepositAmount từ Car
                double deposit = car.DepositAmount;
                
                // Total = SubTotal + Deposit
                double total = subTotal + deposit;

                var newOrder = new CreateRentalOrderDTO
                {
                    PhoneNumber = PhoneNumberTextBox.Text.Trim(),
                    PickupTime = PickupDatePicker.SelectedDate.Value,
                    ExpectedReturnTime = ReturnDatePicker.SelectedDate.Value,
                    WithDriver = WithDriverCheckBox.IsChecked ?? false,
                    UserId = userId,
                    CarId = carId,
                    RentalLocationId = locationId,
                    SubTotal = subTotal,
                    Deposit = deposit,
                    Total = total
                };

                // Debug log để kiểm tra
                System.Diagnostics.Debug.WriteLine($"Creating order with SubTotal={subTotal}, Deposit={deposit}, Total={total}");

                var createdOrder = await _rentalOrderService.CreateAsync(newOrder);

                if (createdOrder != null)
                {
                    MessageBox.Show("Tạo đơn hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadOrders();
                    ClearCreateInputs();
                    SearchTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Tạo đơn hàng thất bại. Kiểm tra dữ liệu hoặc token.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateTotalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần cập nhật.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!double.TryParse(ExtraFeeTextBox.Text, out double extraFee))
                {
                    MessageBox.Show("Phí phát sinh phải là số.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(DamageFeeTextBox.Text, out double damageFee))
                {
                    MessageBox.Show("Phí hư hỏng phải là số.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var updateDto = new UpdateRentalOrderTotalDTO
                {
                    OrderId = _selectedOrder.Id,
                    ExtraFee = extraFee,
                    DamageFee = damageFee,
                    DamageNotes = DamageNotesTextBox.Text.Trim()
                };

                var updatedOrder = await _rentalOrderService.UpdateTotalAsync(updateDto);

                if (updatedOrder != null)
                {
                    MessageBox.Show("Cập nhật tổng tiền thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadOrders();
                }
                else
                {
                    MessageBox.Show("Cập nhật tổng tiền thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ConfirmTotalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xác nhận.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xác nhận tổng tiền cho đơn hàng #{_selectedOrder.Id}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _rentalOrderService.ConfirmTotalAsync(_selectedOrder.Id);

                    if (success)
                    {
                        MessageBox.Show("Xác nhận tổng tiền thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadOrders();
                    }
                    else
                    {
                        MessageBox.Show("Xác nhận tổng tiền thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xác nhận.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xác nhận thanh toán cho đơn hàng #{_selectedOrder.Id}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _rentalOrderService.ConfirmPaymentAsync(_selectedOrder.Id);

                    if (success)
                    {
                        MessageBox.Show("Xác nhận thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadOrders();
                    }
                    else
                    {
                        MessageBox.Show("Xác nhận thanh toán thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ClearCreateInputs()
        {
            PhoneNumberTextBox.Clear();
            PickupDatePicker.SelectedDate = DateTime.Now.AddDays(1);
            ReturnDatePicker.SelectedDate = DateTime.Now.AddDays(2);
            WithDriverCheckBox.IsChecked = false;
            UserIdTextBox.Clear();
            CarIdTextBox.Clear();
            LocationIdTextBox.Clear();
        }
    }
}

