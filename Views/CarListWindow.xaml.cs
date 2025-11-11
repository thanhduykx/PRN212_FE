using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class CarListWindow : Window
    {
        private readonly CarService _carService;
        private CarDTO _selectedCar;

        public ObservableCollection<CarDTO> Cars { get; set; } = new ObservableCollection<CarDTO>();

        public CarListWindow(ApiService apiService)
        {
            InitializeComponent();
            _carService = new CarService(apiService);
            CarDataGrid.ItemsSource = Cars;
            LoadCars();
        }

        private async Task LoadCars()
        {
            try
            {
                var response = await _carService.GetAllCarsAsync();
                Cars.Clear();
                foreach (var car in response)
                    Cars.Add(car);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load lỗi: {ex.Message}");
            }
        }

        private void CarDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarDataGrid.SelectedItem is CarDTO selectedCar)
            {
                _selectedCar = selectedCar;
                NameTextBox.Text = selectedCar.Name;
                ModelTextBox.Text = selectedCar.Model;
                SeatsTextBox.Text = selectedCar.Seats.ToString();
                SizeTextBox.Text = selectedCar.SizeType;
                TrunkTextBox.Text = selectedCar.TrunkCapacity.ToString();
                BatteryTypeTextBox.Text = selectedCar.BatteryType;
                BatteryDurationTextBox.Text = selectedCar.BatteryDuration.ToString();
                RentTextBox.Text = selectedCar.RentPricePerDay.ToString();
                StatusTextBox.Text = selectedCar.Status.ToString();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(ModelTextBox.Text))
                {
                    MessageBox.Show("Tên và Model không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(SeatsTextBox.Text, out int seats) || seats <= 0)
                {
                    MessageBox.Show("Số ghế phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TrunkTextBox.Text, out int trunk) || trunk < 0)
                {
                    MessageBox.Show("Dung tích cốp phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(BatteryDurationTextBox.Text, out int batteryDuration) || batteryDuration < 0)
                {
                    MessageBox.Show("Thời lượng pin phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(RentTextBox.Text, out double rentPrice) || rentPrice < 0)
                {
                    MessageBox.Show("Giá thuê phải là số không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(StatusTextBox.Text, out int status) || (status != 0 && status != 1))
                {
                    MessageBox.Show("Status phải là 0 hoặc 1.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newCar = new CarDTO
                {
                    Name = NameTextBox.Text.Trim(),
                    Model = ModelTextBox.Text.Trim(),
                    Seats = seats,
                    SizeType = SizeTextBox.Text.Trim(),
                    TrunkCapacity = trunk,
                    BatteryType = BatteryTypeTextBox.Text.Trim(),
                    BatteryDuration = batteryDuration,
                    RentPricePerDay = rentPrice,
                    Status = status
                };

                var addedCar = await _carService.AddCarAsync(newCar);

                if (addedCar != null)
                {
                    MessageBox.Show("Thêm xe thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadCars();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Thêm xe thất bại. Kiểm tra dữ liệu hoặc token.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần cập nhật.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(ModelTextBox.Text))
                {
                    MessageBox.Show("Tên và Model không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(SeatsTextBox.Text, out int seats) || seats <= 0)
                {
                    MessageBox.Show("Số ghế phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TrunkTextBox.Text, out int trunk) || trunk < 0)
                {
                    MessageBox.Show("Dung tích cốp phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(BatteryDurationTextBox.Text, out int batteryDuration) || batteryDuration < 0)
                {
                    MessageBox.Show("Thời lượng pin phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(RentTextBox.Text, out double rentPrice) || rentPrice < 0)
                {
                    MessageBox.Show("Giá thuê phải là số không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(StatusTextBox.Text, out int status) || (status != 0 && status != 1))
                {
                    MessageBox.Show("Status phải là 0 hoặc 1.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedCar.Name = NameTextBox.Text.Trim();
                _selectedCar.Model = ModelTextBox.Text.Trim();
                _selectedCar.Seats = seats;
                _selectedCar.SizeType = SizeTextBox.Text.Trim();
                _selectedCar.TrunkCapacity = trunk;
                _selectedCar.BatteryType = BatteryTypeTextBox.Text.Trim();
                _selectedCar.BatteryDuration = batteryDuration;
                _selectedCar.RentPricePerDay = rentPrice;
                _selectedCar.Status = status;

                var updatedCar = await _carService.UpdateCarAsync(_selectedCar);

                if (updatedCar != null)
                {
                    MessageBox.Show("Cập nhật xe thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadCars();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Cập nhật xe thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa xe: {_selectedCar.Name} ({_selectedCar.Model})?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _carService.DeleteCarAsync(_selectedCar.Id);

                    if (success)
                    {
                        MessageBox.Show("Xóa xe thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadCars();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Xóa xe thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearInputs()
        {
            NameTextBox.Clear();
            ModelTextBox.Clear();
            SeatsTextBox.Clear();
            SizeTextBox.Clear();
            TrunkTextBox.Clear();
            BatteryTypeTextBox.Clear();
            BatteryDurationTextBox.Clear();
            RentTextBox.Clear();
            StatusTextBox.Clear();
            _selectedCar = null;
            CarDataGrid.SelectedItem = null;
        }
    }
}
