using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;

namespace AssignmentPRN212.Views
{
    /// <summary>
    /// Staff Dashboard - Main window for station staff
    /// Shows orders and vehicles at the station
    /// </summary>
    public partial class StaffDashboardWindow : Window
    {
        private readonly StaffService _staffService;
        private readonly UserService _userService;
        private readonly Guid _staffId;
        private readonly string _staffName;
        private int? _staffLocationId;

        public StaffDashboardWindow(Guid staffId, string staffName, string apiBaseUrl = "https://localhost:7240/")
        {
            InitializeComponent();

            // Initialize services
            var apiService = new ApiService(apiBaseUrl);
            _staffService = new StaffService(apiService);
            _userService = new UserService(apiService);

            _staffId = staffId;
            _staffName = staffName;

            // Display staff name
            txtStaffInfo.Text = $"Welcome, {staffName}";

            // Load initial data
            LoadStaffInfo();
        }

        #region Data Loading

        /// <summary>
        /// Load staff information to get their assigned location
        /// </summary>
        private async void LoadStaffInfo()
        {
            try
            {
                // Note: Your UserService.GetUserByIdAsync takes int, but User.Id is Guid
                // We need to handle this - for now, we'll load all data without location filter
                await LoadAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading staff info: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load all dashboard data
        /// </summary>
        private async Task LoadAllData()
        {
            await LoadOrders();
            await LoadVehicles();
        }

        /// <summary>
        /// Load rental orders
        /// API: GET /api/RentalOrder/GetAll
        /// </summary>
        private async Task LoadOrders(string? statusFilter = null)
        {
            try
            {
                // Get all orders from API
                var allOrders = await _staffService.GetAllOrdersAsync();

                // Apply status filter if selected
                var filteredOrders = allOrders;
                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
                {
                    filteredOrders = allOrders.Where(o => o.Status == statusFilter).ToList();
                }

                // If staff has location, filter by that
                if (_staffLocationId.HasValue)
                {
                    filteredOrders = filteredOrders.Where(o => o.RentalLocationId == _staffLocationId.Value).ToList();
                }

                // Bind to DataGrid
                dgOrders.ItemsSource = filteredOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load vehicles at station
        /// API: GET /api/CarRentalLocation/GetByRentalLocationId
        /// </summary>
        private async Task LoadVehicles()
        {
            try
            {
                if (_staffLocationId.HasValue)
                {
                    var vehicles = await _staffService.GetVehiclesByLocationAsync(_staffLocationId.Value);
                    dgVehicles.ItemsSource = vehicles;
                }
                else
                {
                    // Load all if no location assigned
                    // For now, just show empty
                    dgVehicles.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Status filter changed
        /// </summary>
        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbStatus.SelectedItem == null) return;

            var selectedStatus = ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();
            _ = LoadOrders(selectedStatus);
        }

        /// <summary>
        /// Refresh button clicked
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _ = LoadAllData();
        }

        /// <summary>
        /// Refresh vehicles button clicked
        /// </summary>
        private void BtnRefreshVehicles_Click(object sender, RoutedEventArgs e)
        {
            _ = LoadVehicles();
        }

        /// <summary>
        /// Handover button clicked
        /// </summary>
        private void BtnHandover_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem == null)
            {
                MessageBox.Show("Please select an order first", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var order = (RentalOrderDTO)dgOrders.SelectedItem;

            if (order.Status != "Confirmed")
            {
                MessageBox.Show("Only confirmed orders can be handed over", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Open handover window
            var handoverWindow = new VehicleHandoverWindow(_staffId, order.Id, order.UserId, order.CarId);
            if (handoverWindow.ShowDialog() == true)
            {
                _ = LoadAllData();
            }
        }

        /// <summary>
        /// Return button clicked
        /// </summary>
        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem == null)
            {
                MessageBox.Show("Please select an order first", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var order = (RentalOrderDTO)dgOrders.SelectedItem;

            if (order.Status != "InProgress")
            {
                MessageBox.Show("Only in-progress orders can be returned", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Open return window
            var returnWindow = new VehicleReturnWindow(_staffId, order.Id, order.UserId, order.CarId);
            if (returnWindow.ShowDialog() == true)
            {
                _ = LoadAllData();
            }
        }

        /// <summary>
        /// Payment button clicked
        /// </summary>
        private void BtnPayment_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem == null)
            {
                MessageBox.Show("Please select an order first", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var order = (RentalOrderDTO)dgOrders.SelectedItem;

            // Open payment window
            var paymentWindow = new PaymentWindow(_staffId, order.Id, order.UserId, order.Total);
            if (paymentWindow.ShowDialog() == true)
            {
                _ = LoadAllData();
            }
        }

        /// <summary>
        /// Logout button clicked
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        #endregion
    }
}