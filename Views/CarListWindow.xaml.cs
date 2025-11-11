using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class CarListWindow : Window
    {
        private readonly CarService _carService;
    private readonly ApiService _apiService;
        public ObservableCollection<CarDTO> Cars { get; set; } = new ObservableCollection<CarDTO>();
        private CarDTO _selectedCar;

        // Constructor nhận ApiService
        public CarListWindow(ApiService apiService)
        {
            InitializeComponent();

            _apiService = apiService; // ✅ gán để dùng trong AIWindow
            _carService = new CarService(apiService); // ✅ dùng để gọi API xe

            CarDataGrid.ItemsSource = Cars;
            _ = LoadCarsAsync();
        }



        private async Task LoadCarsAsync()
        {
            try
            {
                Cars.Clear();
                var cars = await _carService.GetAllCarsAsync();
                foreach (var car in cars)
                    Cars.Add(car);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load dữ liệu: {ex.Message}");
            }
        }

        private void CarDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarDataGrid.SelectedItem is CarDTO car)
            {
                _selectedCar = car;
                NameTextBox.Text = car.Name;
                ModelTextBox.Text = car.Model;
                SeatsTextBox.Text = car.Seats.ToString();
                SizeTextBox.Text = car.SizeType;
                TrunkTextBox.Text = car.TrunkCapacity.ToString();
                BatteryTypeTextBox.Text = car.BatteryType;
                BatteryDurationTextBox.Text = car.BatteryDuration.ToString();
                RentTextBox.Text = car.RentPricePerDay.ToString(CultureInfo.InvariantCulture);
                RentHourTextBox.Text = car.RentPricePerHour.ToString(CultureInfo.InvariantCulture);
                RentDayDriverTextBox.Text = car.RentPricePerDayWithDriver.ToString(CultureInfo.InvariantCulture);
                RentHourDriverTextBox.Text = car.RentPricePerHourWithDriver.ToString(CultureInfo.InvariantCulture);
                ImageUrlTextBox.Text = car.ImageUrl;
                ImageUrl2TextBox.Text = car.ImageUrl2;
                ImageUrl3TextBox.Text = car.ImageUrl3;
                StatusTextBox.Text = car.Status.ToString();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var car = new CreateCarDTO
            {
                Name = NameTextBox.Text,
                Model = ModelTextBox.Text,
                Seats = int.TryParse(SeatsTextBox.Text, out var s) ? s : 0,
                SizeType = SizeTextBox.Text,
                TrunkCapacity = int.TryParse(TrunkTextBox.Text, out var t) ? t : 0,
                BatteryType = BatteryTypeTextBox.Text,
                BatteryDuration = int.TryParse(BatteryDurationTextBox.Text, out var d) ? d : 0,
                RentPricePerDay = double.TryParse(RentTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rd) ? rd : 0,
                RentPricePerHour = double.TryParse(RentHourTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rh) ? rh : 0,
                RentPricePerDayWithDriver = double.TryParse(RentDayDriverTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rdd) ? rdd : 0,
                RentPricePerHourWithDriver = double.TryParse(RentHourDriverTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rhd) ? rhd : 0,
                ImageUrl = ImageUrlTextBox.Text,
                ImageUrl2 = ImageUrl2TextBox.Text,
                ImageUrl3 = ImageUrl3TextBox.Text,
                Status = int.TryParse(StatusTextBox.Text, out var st) ? st : 0
            };

            try
            {
                var added = await _carService.AddCarAsync(car);
                if (added != null)
                {
                    MessageBox.Show("Thêm xe thành công!");
                    await LoadCarsAsync();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm: {ex.Message}");
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null) { MessageBox.Show("Chọn xe trước!"); return; }

            var car = new UpdateCarDTO
            {
                Id = _selectedCar.Id,
                Name = NameTextBox.Text,
                Model = ModelTextBox.Text,
                Seats = int.TryParse(SeatsTextBox.Text, out var s) ? s : 0,
                SizeType = SizeTextBox.Text,
                TrunkCapacity = int.TryParse(TrunkTextBox.Text, out var t) ? t : 0,
                BatteryType = BatteryTypeTextBox.Text,
                BatteryDuration = int.TryParse(BatteryDurationTextBox.Text, out var d) ? d : 0,
                RentPricePerDay = double.TryParse(RentTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rd) ? rd : 0,
                RentPricePerHour = double.TryParse(RentHourTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rh) ? rh : 0,
                RentPricePerDayWithDriver = double.TryParse(RentDayDriverTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rdd) ? rdd : 0,
                RentPricePerHourWithDriver = double.TryParse(RentHourDriverTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var rhd) ? rhd : 0,
                ImageUrl = ImageUrlTextBox.Text,
                ImageUrl2 = ImageUrl2TextBox.Text,
                ImageUrl3 = ImageUrl3TextBox.Text,
                Status = int.TryParse(StatusTextBox.Text, out var st) ? st : 0
            };

            try
            {
                var updated = await _carService.UpdateCarAsync(car);
                if (updated != null)
                {
                    MessageBox.Show("Cập nhật thành công!");
                    await LoadCarsAsync();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null) { MessageBox.Show("Chọn xe trước!"); return; }

            if (MessageBox.Show($"Xóa {_selectedCar.Name}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _carService.DeleteCarAsync(_selectedCar.Id);
                    if (success)
                    {
                        MessageBox.Show("Xóa thành công!");
                        await LoadCarsAsync();
                        ClearInputs();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa: {ex.Message}");
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
            RentHourTextBox.Clear();
            RentDayDriverTextBox.Clear();
            RentHourDriverTextBox.Clear();
            ImageUrlTextBox.Clear();
            ImageUrl2TextBox.Clear();
            ImageUrl3TextBox.Clear();
            StatusTextBox.Clear();
            _selectedCar = null;
            CarDataGrid.SelectedItem = null;
        }

        

        private void OpenAIWindow_Click(object sender, RoutedEventArgs e)
        {

            if (_apiService == null)
            {
                MessageBox.Show("API Service chưa khởi tạo");
                return;
            }

            var aiWindow = new AIWindow(_apiService);
            aiWindow.ShowDialog();
        }
    }
}
