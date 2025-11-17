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
    public partial class RentalLocationListWindow : Window
    {
        private readonly RentalLocationService _rentalLocationService;

        // ObservableCollection bind DataGrid
        public ObservableCollection<RentalLocationDTO> RentalLocations { get; set; } = new ObservableCollection<RentalLocationDTO>();
        private List<RentalLocationDTO> _allLocations = new List<RentalLocationDTO>(); // Lưu danh sách gốc để filter
        private RentalLocationDTO _selectedLocation;

        public RentalLocationListWindow(ApiService apiService)
        {
            InitializeComponent();
            _rentalLocationService = new RentalLocationService(apiService);

            RentalLocationDataGrid.ItemsSource = RentalLocations;

            // Load dữ liệu
            LoadLocations();
        }

        private async Task LoadLocations()
        {
            try
            {
                var locations = await _rentalLocationService.GetAllAsync();
                _allLocations = locations.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";
            
            var filtered = _allLocations.Where(location =>
                string.IsNullOrWhiteSpace(searchText) ||
                location.Name.ToLower().Contains(searchText) ||
                location.Address.ToLower().Contains(searchText) ||
                location.Coordinates.ToLower().Contains(searchText)
            ).ToList();

            RentalLocations.Clear();
            foreach (var location in filtered)
                RentalLocations.Add(location);

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = RentalLocations.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void RentalLocationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RentalLocationDataGrid.SelectedItem is RentalLocationDTO selectedLocation)
            {
                _selectedLocation = selectedLocation;

                NameTextBox.Text = selectedLocation.Name;
                AddressTextBox.Text = selectedLocation.Address;
                CoordinatesTextBox.Text = selectedLocation.Coordinates;
                IsActiveCheckBox.IsChecked = selectedLocation.IsActive;
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
                var newLocation = new CreateRentalLocationDTO
                {
                    Name = NameTextBox.Text.Trim(),
                    Address = AddressTextBox.Text.Trim(),
                    Coordinates = CoordinatesTextBox.Text.Trim(),
                    IsActive = IsActiveCheckBox.IsChecked ?? true
                };

                // Validation
                if (string.IsNullOrWhiteSpace(newLocation.Name))
                {
                    MessageBox.Show("Tên địa điểm không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(newLocation.Address))
                {
                    MessageBox.Show("Địa chỉ không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var addedLocation = await _rentalLocationService.CreateAsync(newLocation);

                if (addedLocation != null)
                {
                    MessageBox.Show("Thêm địa điểm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadLocations();
                    ClearInputs();
                    SearchTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Thêm địa điểm thất bại. Kiểm tra dữ liệu hoặc token.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Vui lòng chọn địa điểm cần cập nhật.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Tên địa điểm không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(AddressTextBox.Text))
                {
                    MessageBox.Show("Địa chỉ không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var updateDto = new UpdateRentalLocationDTO
                {
                    Id = _selectedLocation.Id,
                    Name = NameTextBox.Text.Trim(),
                    Address = AddressTextBox.Text.Trim(),
                    Coordinates = CoordinatesTextBox.Text.Trim(),
                    IsActive = IsActiveCheckBox.IsChecked ?? true
                };

                var updatedLocation = await _rentalLocationService.UpdateAsync(updateDto);

                if (updatedLocation != null)
                {
                    MessageBox.Show("Cập nhật địa điểm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadLocations();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Cập nhật địa điểm thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Vui lòng chọn địa điểm cần xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa địa điểm: {_selectedLocation.Name}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _rentalLocationService.DeleteAsync(_selectedLocation.Id);

                    if (success)
                    {
                        MessageBox.Show("Xóa địa điểm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadLocations();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Xóa địa điểm thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            AddressTextBox.Clear();
            CoordinatesTextBox.Clear();
            IsActiveCheckBox.IsChecked = true;
            _selectedLocation = null;
            RentalLocationDataGrid.SelectedItem = null;
        }
    }
}

