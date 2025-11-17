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
    public partial class CarReturnHistoryListWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly CarService _carService;
        private readonly RentalOrderService _rentalOrderService;

        // ObservableCollection bind DataGrid - hiển thị orders status == 5 (Returned)
        public ObservableCollection<RentalOrderDTO> Orders { get; set; } = new ObservableCollection<RentalOrderDTO>();
        private List<RentalOrderDTO> _allOrders = new List<RentalOrderDTO>(); // Lưu danh sách gốc để filter
        private RentalOrderDTO? _selectedOrder;
        private Dictionary<int, string> _carNamesCache = new Dictionary<int, string>(); // Cache tên xe
        private Dictionary<int, CarDTO> _carsCache = new Dictionary<int, CarDTO>(); // Cache thông tin xe để tính phí tài xế

        public CarReturnHistoryListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _carService = new CarService(apiService);
            _rentalOrderService = new RentalOrderService(apiService);

            CarReturnHistoryDataGrid.ItemsSource = Orders;

            // Load dữ liệu
            this.Loaded += CarReturnHistoryListWindow_Loaded;
        }

        private async void CarReturnHistoryListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHistories();
        }

        private async Task LoadHistories()
        {
            try
            {
                // Hiển thị orders có status == "5" (Returned) - đã trả xe
                var allOrders = await _rentalOrderService.GetAllAsync();
                var returnedOrders = allOrders
                    .Where(o => 
                        o.Status == "5" || 
                        o.Status.Equals("Returned", StringComparison.OrdinalIgnoreCase) ||
                        o.GetStatusEnum() == RentalOrderStatus.Returned)
                    .ToList();
                
                // Load thông tin xe cho tất cả CarId
                var carIds = returnedOrders.Select(o => o.CarId).Distinct().ToList();
                await LoadCarNames(carIds);
                await LoadCars(carIds);
                
                // Gán tên xe và tính phí tài xế vào từng order
                foreach (var order in returnedOrders)
                {
                    if (_carNamesCache.ContainsKey(order.CarId))
                    {
                        order.CarName = _carNamesCache[order.CarId];
                    }
                    else
                    {
                        order.CarName = $"Car #{order.CarId}";
                    }
                    
                    // Tính phí tài xế để hiển thị (không ghi đè SubTotal và Deposit từ backend)
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
                
                _allOrders = returnedOrders.ToList();
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

        private void CarReturnHistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarReturnHistoryDataGrid.SelectedItem is RentalOrderDTO selectedOrder)
            {
                _selectedOrder = selectedOrder;
                if (PayButton != null) PayButton.IsEnabled = true;
                
                // Load giá trị hiện tại vào các TextBox
                if (ExtraFeeTextBox != null) ExtraFeeTextBox.Text = (selectedOrder.ExtraFee ?? 0).ToString();
                if (DiscountTextBox != null) DiscountTextBox.Text = (selectedOrder.Discount ?? 0).ToString();
                if (DamageFeeTextBox != null) DamageFeeTextBox.Text = (selectedOrder.DamageFee ?? 0).ToString();
                if (DamageNotesTextBox != null) DamageNotesTextBox.Text = selectedOrder.DamageNotes ?? "";
                
                // Tính toán lại tổng tiền
                CalculateTotal();
            }
            else
            {
                _selectedOrder = null;
                if (PayButton != null) PayButton.IsEnabled = false;
                
                // Clear các TextBox
                if (ExtraFeeTextBox != null) ExtraFeeTextBox.Text = "0";
                if (DiscountTextBox != null) DiscountTextBox.Text = "0";
                if (DamageFeeTextBox != null) DamageFeeTextBox.Text = "0";
                if (DamageNotesTextBox != null) DamageNotesTextBox.Text = "";
                if (CalculatedTotalTextBlock != null) CalculatedTotalTextBlock.Text = "0 VNĐ";
            }
        }
        
        private void FeeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal();
        }
        
        private void CalculateTotal()
        {
            // Kiểm tra null để tránh lỗi khi window chưa được khởi tạo hoàn toàn
            if (CalculatedTotalTextBlock == null)
            {
                return;
            }
            
            if (_selectedOrder == null)
            {
                CalculatedTotalTextBlock.Text = "0 VNĐ";
                return;
            }
            
            // Parse các giá trị từ TextBox
            double extraFee = 0;
            if (ExtraFeeTextBox != null)
            {
                double.TryParse(ExtraFeeTextBox.Text, out extraFee);
            }
            
            int discount = 0;
            if (DiscountTextBox != null)
            {
                int.TryParse(DiscountTextBox.Text, out discount);
            }
            
            double damageFee = 0;
            if (DamageFeeTextBox != null)
            {
                double.TryParse(DamageFeeTextBox.Text, out damageFee);
            }
            
            // Lấy giá trị gốc từ order
            double subTotal = _selectedOrder.SubTotal ?? 0;
            double deposit = _selectedOrder.Deposit ?? 0;
            
            // Tính tổng tiền sau khi áp dụng các phí và giảm giá
            // Tổng tiền = (Deposit + SubTotal + ExtraFee + DamageFee) * (1 - Discount/100)
            double totalBeforeDiscount = deposit + subTotal + extraFee + damageFee;
            double discountAmount = totalBeforeDiscount * (discount / 100.0);
            double finalTotal = totalBeforeDiscount - discountAmount;
            
            CalculatedTotalTextBlock.Text = $"{finalTotal:N0} VNĐ";
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadHistories();
        }

        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần thanh toán.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse các giá trị từ TextBox
            double extraFee = 0;
            if (ExtraFeeTextBox != null)
            {
                double.TryParse(ExtraFeeTextBox.Text, out extraFee);
            }
            
            int discount = 0;
            if (DiscountTextBox != null)
            {
                int.TryParse(DiscountTextBox.Text, out discount);
            }
            
            double damageFee = 0;
            if (DamageFeeTextBox != null)
            {
                double.TryParse(DamageFeeTextBox.Text, out damageFee);
            }
            
            string damageNotes = "";
            if (DamageNotesTextBox != null)
            {
                damageNotes = DamageNotesTextBox.Text.Trim();
            }

            // Tính tổng tiền để hiển thị trong xác nhận
            double subTotal = _selectedOrder.SubTotal ?? 0;
            double deposit = _selectedOrder.Deposit ?? 0;
            double totalBeforeDiscount = deposit + subTotal + extraFee + damageFee;
            double discountAmount = totalBeforeDiscount * (discount / 100.0);
            double finalTotal = totalBeforeDiscount - discountAmount;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn thanh toán cho đơn hàng #{_selectedOrder.Id}?\n\n" +
                $"Tên xe: {_selectedOrder.CarName}\n" +
                $"Tạm tính: {subTotal:N0} VNĐ\n" +
                $"Đặt cọc: {deposit:N0} VNĐ\n" +
                $"Phí phát sinh: {extraFee:N0} VNĐ\n" +
                $"Phí hư hỏng: {damageFee:N0} VNĐ\n" +
                $"Giảm giá: {discount}%\n" +
                $"Tổng tiền: {finalTotal:N0} VNĐ\n\n" +
                $"Đơn hàng sẽ được chuyển sang trạng thái 'PaymentPending'.",
                "Xác nhận thanh toán",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    PayButton.IsEnabled = false;
                    PayButton.Content = "Đang xử lý...";

                    // Cập nhật ExtraFee, DamageFee, DamageNotes, Discount trước
                    var updateTotalRequest = new UpdateRentalOrderTotalDTO
                    {
                        OrderId = _selectedOrder.Id,
                        ExtraFee = extraFee,
                        DamageFee = damageFee,
                        DamageNotes = damageNotes,
                        Discount = discount
                    };
                    
                    var updatedTotalOrder = await _rentalOrderService.UpdateTotalAsync(updateTotalRequest);
                    
                    if (updatedTotalOrder == null)
                    {
                        MessageBox.Show("Cập nhật phí thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Sau đó chuyển status sang PaymentPending
                    var updatedOrder = await _rentalOrderService.UpdateOrderStatusAsync(_selectedOrder.Id, RentalOrderStatus.PaymentPending);

                    if (updatedOrder != null)
                    {
                        MessageBox.Show("Thanh toán thành công! Đơn hàng đã được chuyển sang trạng thái 'PaymentPending'.", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadHistories();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật phí thành công nhưng chuyển trạng thái thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    PayButton.IsEnabled = true;
                    PayButton.Content = "💰 Thanh toán";
                }
            }
        }
    }
}

