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
                var newCar = new CarDTO
                {
                    Name = NameTextBox.Text.Trim(),
                    Model = ModelTextBox.Text.Trim(),
                    Seats = int.TryParse(SeatsTextBox.Text, out int s) ? s : 0,
                    SizeType = SizeTextBox.Text.Trim(),
                    TrunkCapacity = int.TryParse(TrunkTextBox.Text, out int t) ? t : 0,
                    BatteryType = BatteryTypeTextBox.Text.Trim(),
                    BatteryDuration = int.TryParse(BatteryDurationTextBox.Text, out int bd) ? bd : 0,
                    RentPricePerDay = double.TryParse(RentTextBox.Text, out double rp) ? rp : 0,
                    Status = int.TryParse(StatusTextBox.Text, out int st) ? st : 0
                };

                // Check dữ liệu cơ bản
                if (string.IsNullOrWhiteSpace(newCar.Name) || string.IsNullOrWhiteSpace(newCar.Model))
                {
                    MessageBox.Show("Tên và Model không được để trống.");
                    return;
                }

                var addedCar = await _carService.AddCarAsync(newCar);

                if (addedCar != null)
                {
                    MessageBox.Show("Thêm xe thành công!");
                    await LoadCars(); // Load lại danh sách
                }
                else
                {
                    MessageBox.Show("Thêm xe thất bại. Kiểm tra dữ liệu hoặc token.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }


        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null) return;

            _selectedCar.Name = NameTextBox.Text;
            _selectedCar.Model = ModelTextBox.Text;
            _selectedCar.Seats = int.Parse(SeatsTextBox.Text);
            _selectedCar.SizeType = SizeTextBox.Text;
            _selectedCar.TrunkCapacity = int.Parse(TrunkTextBox.Text);
            _selectedCar.BatteryType = BatteryTypeTextBox.Text;
            _selectedCar.BatteryDuration = int.Parse(BatteryDurationTextBox.Text);
            _selectedCar.RentPricePerDay = double.Parse(RentTextBox.Text);
            _selectedCar.Status = int.Parse(StatusTextBox.Text);

            await _carService.UpdateCarAsync(_selectedCar);
            await LoadCars();
            ClearInputs();
            MessageBox.Show("Cập nhật thành công!");
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null) return;

            await _carService.DeleteCarAsync(_selectedCar.Id);
            await LoadCars();
            ClearInputs();
            MessageBox.Show("Xóa thành công!");
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
        }
    }
}
