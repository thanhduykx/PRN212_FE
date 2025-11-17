using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;

namespace AssignmentPRN212.Views
{
    public partial class HomeWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly CarService _carService;
        private List<CarDTO> _allCars = new List<CarDTO>();
        private string? _currentUserRole;
        private int? _currentUserId;

        public HomeWindow()
        {
            InitializeComponent();
            string apiBaseUrl = "https://localhost:7200/api/";
            _apiService = new ApiService(apiBaseUrl);
            _carService = new CarService(_apiService);
            
            // Đợi window load xong mới gọi LoadCars
            this.Loaded += HomeWindow_Loaded;
            CheckLoginStatus();
        }

        public HomeWindow(ApiService apiService, string? role = null, int? userId = null)
        {
            InitializeComponent();
            _apiService = apiService;
            _carService = new CarService(_apiService);
            _currentUserRole = role;
            _currentUserId = userId;
            
            // Đợi window load xong mới gọi LoadCars
            this.Loaded += HomeWindow_Loaded;
            CheckLoginStatus();
        }

        private void HomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCars();
        }

        // Helper method để convert status sang color
        private Color GetStatusColor(int status)
        {
            return status == 0 ? Colors.Green : Colors.Red;
        }

        private void CheckLoginStatus()
        {
            if (!string.IsNullOrEmpty(_currentUserRole))
            {
                UserInfoTextBlock.Text = $"Xin chào! ({_currentUserRole})";
                LoginButton.Visibility = Visibility.Collapsed;
                DashboardButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Visible;
            }
            else
            {
                UserInfoTextBlock.Text = "Chưa đăng nhập";
                LoginButton.Visibility = Visibility.Visible;
                DashboardButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void LoadCars()
        {
            try
            {
                _allCars = await _carService.GetAllCarsAsync();
                
                // Load feedback count cho mỗi xe
                var feedbackService = new FeedbackService(_apiService);
                foreach (var car in _allCars)
                {
                    try
                    {
                        var feedbacks = await feedbackService.GetByCarNameAsync(car.Name);
                        // Có thể thêm property FeedbackCount vào CarDTO nếu cần
                    }
                    catch { }
                }
                
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách xe: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            // Kiểm tra null trước khi sử dụng
            if (CarsItemsControl == null || TotalCarsTextBlock == null)
                return;

            string searchText = SearchTextBox?.Text?.ToLower() ?? "";
            string selectedSize = (SizeTypeComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả kích thước";
            
            var filtered = _allCars.Where(car =>
                (string.IsNullOrWhiteSpace(searchText) ||
                 car.Name.ToLower().Contains(searchText) ||
                 car.Model.ToLower().Contains(searchText) ||
                 car.SizeType.ToLower().Contains(searchText) ||
                 car.BatteryType.ToLower().Contains(searchText)) &&
                (selectedSize == "Tất cả kích thước" || car.SizeType.Contains(selectedSize)) &&
                car.IsActive && !car.IsDeleted && car.Status == 0 // Chỉ hiển thị xe còn trống
            ).ToList();

            CarsItemsControl.ItemsSource = filtered;
            TotalCarsTextBlock.Text = $"Tìm thấy {filtered.Count} xe";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void CarCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245));
                border.BorderBrush = new SolidColorBrush(Colors.Blue);
            }
        }

        private void CarCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
            }
        }

        private void CarCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Xem chi tiết xe - có thể mở CarDetailWindow
        }

        private void ViewDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CarDTO car)
            {
                var detailWindow = new CarDetailWindow(_apiService, car, _currentUserId, _currentUserRole);
                detailWindow.ShowDialog();
                LoadCars(); // Reload để cập nhật feedback count
            }
        }

        private void FeedbackPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Tag is CarDTO car)
            {
                var detailWindow = new CarDetailWindow(_apiService, car, _currentUserId, _currentUserRole);
                detailWindow.ShowDialog();
                LoadCars();
            }
        }

        private void RentCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CarDTO car)
            {
                if (string.IsNullOrEmpty(_currentUserRole))
                {
                    MessageBox.Show("Vui lòng đăng nhập để thuê xe.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoginButton_Click(sender, e);
                    return;
                }

                // Mở window thuê xe
                var bookingWindow = new CarBookingWindow(_apiService, car, _currentUserId ?? 0);
                bookingWindow.ShowDialog();
                
                // Reload sau khi đóng window
                LoadCars();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(this);
            loginWindow.ShowDialog();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUserRole))
            {
                MessageBox.Show("Vui lòng đăng nhập.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_currentUserRole == "Admin")
            {
                var adminWindow = new MainWindow(_apiService, "Admin");
                adminWindow.Show();
            }
            else if (_currentUserRole == "Staff")
            {
                var staffWindow = new StaffDashboardWindow(_apiService, _currentUserId ?? 0);
                staffWindow.Show();
            }
            else // Customer
            {
                var customerWindow = new CustomerDashboardWindow(_apiService, _currentUserId ?? 0);
                customerWindow.Show();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _apiService.SetToken("");
            _currentUserRole = null;
            _currentUserId = null;
            CheckLoginStatus();
            MessageBox.Show("Đã đăng xuất thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

