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
    public partial class CarRentalLocationListWindow : Window
    {
        private readonly CarRentalLocationService _carRentalLocationService;
        private readonly CarService _carService;
        private readonly RentalLocationService _rentalLocationService;

        // ObservableCollection bind DataGrid
        public ObservableCollection<CarRentalLocationDTO> CarRentalLocations { get; set; } = new ObservableCollection<CarRentalLocationDTO>();
        private List<CarRentalLocationDTO> _allLocations = new List<CarRentalLocationDTO>(); // Lưu danh sách gốc để filter
        private CarRentalLocationDTO _selectedLocation;
        private Dictionary<int, string> _carNamesCache = new Dictionary<int, string>(); // Cache tên xe
        private Dictionary<int, string> _locationNamesCache = new Dictionary<int, string>(); // Cache tên địa điểm

        public CarRentalLocationListWindow(ApiService apiService)
        {
            InitializeComponent();
            _carRentalLocationService = new CarRentalLocationService(apiService);
            _carService = new CarService(apiService);
            _rentalLocationService = new RentalLocationService(apiService);

            CarRentalLocationDataGrid.ItemsSource = CarRentalLocations;

            // Load dữ liệu
            this.Loaded += CarRentalLocationListWindow_Loaded;
        }

        private async void CarRentalLocationListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLocations();
        }

        private async Task LoadLocations()
        {
            try
            {
                var locations = await _carRentalLocationService.GetAllAsync();
                
                // Load tên xe và địa điểm
                var carIds = locations.Select(l => l.CarId).Distinct().ToList();
                var locationIds = locations.Select(l => l.LocationId).Distinct().ToList();
                await LoadCarNames(carIds);
                await LoadLocationNames(locationIds);
                
                // Gán tên xe và địa điểm vào từng location
                foreach (var location in locations)
                {
                    if (_carNamesCache.ContainsKey(location.CarId))
                    {
                        location.CarName = _carNamesCache[location.CarId];
                    }
                    else
                    {
                        location.CarName = $"Car #{location.CarId}";
                    }
                    
                    if (_locationNamesCache.ContainsKey(location.LocationId))
                    {
                        location.LocationName = _locationNamesCache[location.LocationId];
                    }
                    else
                    {
                        location.LocationName = $"Location #{location.LocationId}";
                    }
                }
                
                _allLocations = locations.ToList();
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

        private async Task LoadLocationNames(List<int> locationIds)
        {
            try
            {
                var allLocations = await _rentalLocationService.GetAllAsync();
                foreach (var location in allLocations)
                {
                    if (!_locationNamesCache.ContainsKey(location.Id))
                    {
                        _locationNamesCache[location.Id] = location.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading location names: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";
            
            var filtered = _allLocations.Where(location =>
                string.IsNullOrWhiteSpace(searchText) ||
                location.CarId.ToString().Contains(searchText) ||
                (location.CarName != null && location.CarName.ToLower().Contains(searchText)) ||
                location.LocationId.ToString().Contains(searchText) ||
                (location.LocationName != null && location.LocationName.ToLower().Contains(searchText)) ||
                location.Quantity.ToString().Contains(searchText) ||
                location.Id.ToString().Contains(searchText)
            ).ToList();

            CarRentalLocations.Clear();
            foreach (var location in filtered)
                CarRentalLocations.Add(location);

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = CarRentalLocations.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CarRentalLocationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarRentalLocationDataGrid.SelectedItem is CarRentalLocationDTO selectedLocation)
            {
                _selectedLocation = selectedLocation;

                QuantityTextBox.Text = selectedLocation.Quantity.ToString();
                CarIdTextBox.Text = selectedLocation.CarId.ToString();
                LocationIdTextBox.Text = selectedLocation.LocationId.ToString();
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadLocations();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                var newLocation = new CreateCarRentalLocationDTO
                {
                    Quantity = quantity,
                    CarId = carId,
                    LocationId = locationId
                };

                var createdLocation = await _carRentalLocationService.CreateAsync(newLocation);

                if (createdLocation != null)
                {
                    MessageBox.Show("Thêm xe tại địa điểm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadLocations();
                    ClearInputs();
                    SearchTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Thêm xe tại địa điểm thất bại. Kiểm tra dữ liệu hoặc token.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLocation == null)
            {
                MessageBox.Show("Vui lòng chọn xe tại địa điểm cần cập nhật.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Validation
                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var updateDto = new UpdateCarRentalLocationDTO
                {
                    Id = _selectedLocation.Id,
                    Quantity = quantity
                };

                var updatedLocation = await _carRentalLocationService.UpdateAsync(updateDto);

                if (updatedLocation != null)
                {
                    MessageBox.Show("Cập nhật số lượng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadLocations();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Cập nhật số lượng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLocation == null)
            {
                MessageBox.Show("Vui lòng chọn xe tại địa điểm cần xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa xe tại địa điểm ID: {_selectedLocation.Id}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _carRentalLocationService.DeleteAsync(_selectedLocation.Id);

                    if (success)
                    {
                        MessageBox.Show("Xóa xe tại địa điểm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadLocations();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Xóa xe tại địa điểm thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            QuantityTextBox.Clear();
            CarIdTextBox.Clear();
            LocationIdTextBox.Clear();
            _selectedLocation = null;
            CarRentalLocationDataGrid.SelectedItem = null;
        }
    }
}

