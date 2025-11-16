using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using AssignmentPRN212.Services;
using AssignmentPRN212.DTO;

namespace AssignmentPRN212.Views
{
    public partial class RentalLocationListWindow : Window
    {
        private readonly ApiService _apiService;
        public ObservableCollection<RentalLocationDTO> RentalLocations { get; set; } = new ObservableCollection<RentalLocationDTO>();
        private RentalLocationDTO _selectedRentalLocation;

        public RentalLocationListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            RentalLocationDataGrid.ItemsSource = RentalLocations;
            _ = LoadRentalLocationsAsync();
        }

        private async Task LoadRentalLocationsAsync()
        {
            try
            {
                RentalLocations.Clear();
                var rentalLocations = await _apiService.GetAllRentalLocationsAsync();
                foreach (var location in rentalLocations)
                {
                    RentalLocations.Add(location);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rental locations: {ex.Message}");
            }
        }

        private void RentalLocationDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RentalLocationDataGrid.SelectedItem is RentalLocationDTO location)
            {
                _selectedRentalLocation = location;
                NameTextBox.Text = location.Name;
                AddressTextBox.Text = location.Address;
                CoordinatesTextBox.Text = location.Coordinates;
                IsActiveCheckBox.IsChecked = location.IsActive;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var location = new RentalLocationDTO
            {
                Name = NameTextBox.Text,
                Address = AddressTextBox.Text,
                Coordinates = CoordinatesTextBox.Text,
                IsActive = IsActiveCheckBox.IsChecked ?? false
            };

            try
            {
                var added = await _apiService.AddRentalLocationAsync(location);
                if (added != null)
                {
                    MessageBox.Show("Rental location added successfully!");
                    await LoadRentalLocationsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding rental location: {ex.Message}");
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRentalLocation == null)
            {
                MessageBox.Show("Select a rental location first!");
                return;
            }

            _selectedRentalLocation.Name = NameTextBox.Text;
            _selectedRentalLocation.Address = AddressTextBox.Text;
            _selectedRentalLocation.Coordinates = CoordinatesTextBox.Text;
            _selectedRentalLocation.IsActive = IsActiveCheckBox.IsChecked ?? false;

            try
            {
                var updated = await _apiService.UpdateRentalLocationAsync(_selectedRentalLocation);
                if (updated != null)
                {
                    MessageBox.Show("Rental location updated successfully!");
                    await LoadRentalLocationsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating rental location: {ex.Message}");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRentalLocation == null)
            {
                MessageBox.Show("Select a rental location first!");
                return;
            }

            try
            {
                var success = await _apiService.DeleteRentalLocationAsync(_selectedRentalLocation.Id);
                if (success)
                {
                    MessageBox.Show("Rental location deleted successfully!");
                    await LoadRentalLocationsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting rental location: {ex.Message}");
            }
        }
    }
}