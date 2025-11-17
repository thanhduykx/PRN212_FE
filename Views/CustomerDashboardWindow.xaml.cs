using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class CustomerDashboardWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly RentalOrderService _rentalOrderService;
        private readonly int _userId;

        public CustomerDashboardWindow(ApiService apiService, int userId)
        {
            InitializeComponent();
            _apiService = apiService;
            _rentalOrderService = new RentalOrderService(_apiService);
            _userId = userId;
            LoadQuickInfo();
        }

        private async void LoadQuickInfo()
        {
            try
            {
                var orders = await _rentalOrderService.GetByUserIdAsync(_userId);
                TotalRentalsTextBlock.Text = $"Tổng số chuyến: {orders.Count}";
                
                double totalSpent = orders.Where(o => o.Total.HasValue).Sum(o => o.Total.Value);
                TotalSpentTextBlock.Text = $"Tổng chi phí: {totalSpent:N0} VNĐ";
                
                int activeOrders = orders.Count(o => o.Status != "Completed" && o.Status != "Cancelled");
                ActiveRentalsTextBlock.Text = $"Đơn đang hoạt động: {activeOrders}";

                // Hiển thị đơn gần đây
                var recentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList();
                RecentOrdersListBox.ItemsSource = recentOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FindLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new HomeWindow(_apiService, "Customer", _userId);
            homeWindow.Show();
            this.Close();
        }

        private void BookCarButton_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new HomeWindow(_apiService, "Customer", _userId);
            homeWindow.Show();
            this.Close();
        }

        private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new CustomerOrderHistoryWindow(_apiService, _userId);
            historyWindow.ShowDialog();
            LoadQuickInfo();
        }

        private void ViewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new CustomerProfileWindow(_apiService, _userId);
            profileWindow.ShowDialog();
        }

        private void ViewActiveRentalsButton_Click(object sender, RoutedEventArgs e)
        {
            var activeRentalsWindow = new CustomerActiveRentalWindow(_apiService, _userId);
            activeRentalsWindow.ShowDialog();
            LoadQuickInfo(); // Reload sau khi đóng window
        }
    }
}

