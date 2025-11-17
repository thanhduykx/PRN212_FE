using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class CarBookingWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly RentalOrderService _rentalOrderService;
        private readonly RentalLocationService _rentalLocationService;
        private readonly CarDTO _selectedCar;
        private readonly int _userId;
        private double _currentPrice = 0;

        public CarBookingWindow(ApiService apiService, CarDTO car, int userId)
        {
            InitializeComponent();
            _apiService = apiService;
            _rentalOrderService = new RentalOrderService(_apiService);
            _rentalLocationService = new RentalLocationService(_apiService);
            _selectedCar = car;
            _userId = userId;

            InitializeCarInfo();
            LoadLocations();
            SetDefaultDates();
        }

        private void InitializeCarInfo()
        {
            CarNameTextBlock.Text = _selectedCar.Name;
            CarModelTextBlock.Text = _selectedCar.Model;
            CarSeatsTextBlock.Text = _selectedCar.Seats.ToString();
            CarBatteryTextBlock.Text = _selectedCar.BatteryDuration.ToString();
            CarTrunkTextBlock.Text = _selectedCar.TrunkCapacity.ToString();
            CarPriceTextBlock.Text = _selectedCar.RentPricePerDay.ToString("N0");
        }

        private async void LoadLocations()
        {
            try
            {
                var locations = await _rentalLocationService.GetAllAsync();
                LocationComboBox.ItemsSource = locations;
                if (locations.Count > 0)
                    LocationComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải địa điểm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetDefaultDates()
        {
            PickupDatePicker.SelectedDate = DateTime.Now.AddDays(1);
            ReturnDatePicker.SelectedDate = DateTime.Now.AddDays(2);
            CalculatePrice();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void LocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Có thể tính lại giá nếu cần
        }

        private void WithDriverCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CalculatePrice();
        }

        private void WithDriverCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CalculatePrice();
        }

        private void CalculatePrice()
        {
            if (!PickupDatePicker.SelectedDate.HasValue || !ReturnDatePicker.SelectedDate.HasValue)
            {
                TotalPriceTextBlock.Text = "0 VNĐ";
                return;
            }

            var pickupDate = PickupDatePicker.SelectedDate.Value;
            var returnDate = ReturnDatePicker.SelectedDate.Value;

            if (returnDate <= pickupDate)
            {
                TotalPriceTextBlock.Text = "0 VNĐ";
                return;
            }

            int days = (returnDate - pickupDate).Days + 1;
            double pricePerDay = _selectedCar.RentPricePerDay; // Giá không tài xế
            double pricePerDayWithDriver = _selectedCar.RentPricePerDayWithDriver; // Giá có tài xế
            double driverFeePerDay = pricePerDayWithDriver - pricePerDay; // Phí tài xế/ngày
            
            // Tính phí tài xế tổng (nếu có tài xế)
            double totalDriverFee = WithDriverCheckBox.IsChecked == true ? driverFeePerDay * days : 0;
            
            // Tổng tiền = (giá không tài xế * số ngày) + phí tài xế
            _currentPrice = (days * pricePerDay) + totalDriverFee;

            RentalDaysTextBlock.Text = $"Số ngày: {days}";
            PricePerDayTextBlock.Text = $"Giá/ngày: {pricePerDay:N0} VNĐ";
            DriverFeeTextBlock.Text = WithDriverCheckBox.IsChecked == true 
                ? $"Phí tài xế: {totalDriverFee:N0} VNĐ" 
                : "Phí tài xế: 0 VNĐ";
            TotalPriceTextBlock.Text = $"{_currentPrice:N0} VNĐ";
        }

        private async void BookButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (LocationComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn địa điểm nhận xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!PickupDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày nhận xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ReturnDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày trả xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ReturnDatePicker.SelectedDate.Value <= PickupDatePicker.SelectedDate.Value)
                {
                    MessageBox.Show("Ngày trả xe phải sau ngày nhận xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedLocation = LocationComboBox.SelectedItem as RentalLocationDTO;
                if (selectedLocation == null)
                {
                    MessageBox.Show("Địa điểm không hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tính toán giá tiền
                int days = (ReturnDatePicker.SelectedDate.Value - PickupDatePicker.SelectedDate.Value).Days + 1;
                double pricePerDay = _selectedCar.RentPricePerDay;
                double pricePerDayWithDriver = _selectedCar.RentPricePerDayWithDriver;
                double driverFeePerDay = pricePerDayWithDriver - pricePerDay;
                
                // Tính phí tài xế tổng (nếu có tài xế)
                double totalDriverFee = (WithDriverCheckBox.IsChecked == true) ? driverFeePerDay * days : 0;
                
                // SubTotal = (giá không tài xế * số ngày) + phí tài xế
                double subTotal = (days * pricePerDay) + totalDriverFee;
                
                // Deposit = DepositAmount từ Car
                double deposit = _selectedCar.DepositAmount;
                
                // Total = SubTotal + Deposit
                double total = subTotal + deposit;

                // Tạo đơn hàng
                var order = new CreateRentalOrderDTO
                {
                    PhoneNumber = PhoneNumberTextBox.Text.Trim(),
                    PickupTime = PickupDatePicker.SelectedDate.Value,
                    ExpectedReturnTime = ReturnDatePicker.SelectedDate.Value,
                    WithDriver = WithDriverCheckBox.IsChecked ?? false,
                    UserId = _userId,
                    CarId = _selectedCar.Id,
                    RentalLocationId = selectedLocation.Id,
                    SubTotal = subTotal,
                    Deposit = deposit,
                    Total = total
                };

                var createdOrder = await _rentalOrderService.CreateAsync(order);

                if (createdOrder != null)
                {
                    MessageBox.Show(
                        "Đặt xe thành công! Đơn hàng của bạn đang chờ nhân viên xác nhận.\nBạn sẽ nhận được thông báo khi đơn hàng được chấp nhận.",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Đặt xe thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

