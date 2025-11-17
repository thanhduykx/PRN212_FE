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
    public partial class ActiveRentalListWindow : Window
    {
        private readonly RentalOrderService _rentalOrderService;
        private readonly CarService _carService;
        private readonly UserService _userService;

        public ObservableCollection<RentalOrderDTO> RentalOrders { get; set; } = new ObservableCollection<RentalOrderDTO>();
        private List<RentalOrderDTO> _allOrders = new List<RentalOrderDTO>();
        private Dictionary<int, string> _carNamesCache = new Dictionary<int, string>();
        private Dictionary<int, string> _userNamesCache = new Dictionary<int, string>();

        public ActiveRentalListWindow(ApiService apiService)
        {
            InitializeComponent();
            _rentalOrderService = new RentalOrderService(apiService);
            _carService = new CarService(apiService);
            _userService = new UserService(apiService);
            RentalOrderDataGrid.ItemsSource = RentalOrders;
            Loaded += ActiveRentalListWindow_Loaded;
        }

        private async void ActiveRentalListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            try
            {
                // Load tất cả orders
                var allOrders = await _rentalOrderService.GetAllAsync();
                
                // Chỉ lấy orders có status == "4" (Renting)
                var rentingOrders = allOrders
                    .Where(o => o.Status == "4" || o.Status.Equals("Renting", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Load tên xe và tên khách hàng
                var carIds = rentingOrders.Select(o => o.CarId).Distinct().ToList();
                var userIds = rentingOrders.Select(o => o.UserId).Distinct().ToList();
                
                await LoadCarNames(carIds);
                await LoadUserNames(userIds);

                // Gán tên xe và tên khách hàng vào từng order
                foreach (var order in rentingOrders)
                {
                    if (_carNamesCache.ContainsKey(order.CarId))
                    {
                        order.CarName = _carNamesCache[order.CarId];
                    }
                    else
                    {
                        order.CarName = $"Car #{order.CarId}";
                    }

                    if (_userNamesCache.ContainsKey(order.UserId))
                    {
                        order.UserName = _userNamesCache[order.UserId];
                    }
                    else
                    {
                        order.UserName = $"User #{order.UserId}";
                    }
                }

                _allOrders = rentingOrders.ToList();
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

        private async Task LoadUserNames(List<int> userIds)
        {
            try
            {
                var allUsers = await _userService.GetAllUsersAsync();
                foreach (var user in allUsers)
                {
                    if (!_userNamesCache.ContainsKey(user.Id))
                    {
                        _userNamesCache[user.Id] = user.FullName ?? user.Email ?? $"User #{user.Id}";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user names: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";

            var filtered = _allOrders.Where(order =>
                string.IsNullOrWhiteSpace(searchText) ||
                order.Id.ToString().Contains(searchText) ||
                (order.CarName != null && order.CarName.ToLower().Contains(searchText)) ||
                (order.UserName != null && order.UserName.ToLower().Contains(searchText)) ||
                order.PhoneNumber.ToLower().Contains(searchText) ||
                order.Status.ToLower().Contains(searchText)
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
            // Có thể thêm logic xử lý khi chọn order
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }
    }
}

