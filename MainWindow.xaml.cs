using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Linq;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly string _role;

        public MainWindow(ApiService apiService, string role)
        {
            InitializeComponent();
            _apiService = apiService;
            _role = role; // ✅ Gán role

            // Nếu không phải Admin, ẩn nút xem user
            if (_role != "Admin")
            {
                ViewUsersButton.Visibility = Visibility.Collapsed;
            }
            
            // Load doanh thu khi khởi tạo
            LoadRevenue();
        }
        
        private async void LoadRevenue()
        {
            try
            {
                var rentalOrderService = new RentalOrderService(_apiService);
                var allOrders = await rentalOrderService.GetAllAsync();
                
                // Tính doanh thu từ orders có status == 8 (Completed)
                var completedOrders = allOrders
                    .Where(o => o.Status == "8" || o.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) 
                             || o.GetStatusEnum() == RentalOrderStatus.Completed)
                    .ToList();
                
                // Doanh thu = Deposit + SubTotal (theo yêu cầu)
                double totalRevenue = completedOrders
                    .Sum(o => (o.Deposit ?? 0) + (o.SubTotal ?? 0));
                
                var thisMonthOrders = completedOrders
                    .Where(o => o.OrderDate.Year == DateTime.Now.Year && o.OrderDate.Month == DateTime.Now.Month)
                    .ToList();
                
                double thisMonthRevenue = thisMonthOrders
                    .Sum(o => (o.Deposit ?? 0) + (o.SubTotal ?? 0));
                
                RevenueTextBlock.Text = $"Tổng doanh thu: {totalRevenue:N0} VNĐ";
                ThisMonthRevenueTextBlock.Text = $"Doanh thu tháng này: {thisMonthRevenue:N0} VNĐ";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading revenue: {ex.Message}");
            }
        }


        private void ViewCarsButton_Click(object sender, RoutedEventArgs e)
        {
            var carWindow = new CarListWindow(_apiService);
            carWindow.ShowDialog();
        }

        private void ViewUsersButton_Click(object sender, RoutedEventArgs e)
        {
            var userWindow = new UserListWindow(_apiService);
            userWindow.ShowDialog();
        }

        private void ViewRentalLocationsButton_Click(object sender, RoutedEventArgs e)
        {
            var locationWindow = new RentalLocationListWindow(_apiService);
            locationWindow.ShowDialog();
        }

        private void ViewRentalOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new RentalOrderListWindow(_apiService);
            orderWindow.ShowDialog();
        }

        private void ViewCarDeliveryHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new CarDeliveryHistoryListWindow(_apiService);
            historyWindow.ShowDialog();
        }

        private void ViewCarReturnHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var returnHistoryWindow = new CarReturnHistoryListWindow(_apiService);
            returnHistoryWindow.ShowDialog();
        }

        private void ViewCarRentalLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var carRentalLocationWindow = new CarRentalLocationListWindow(_apiService);
            carRentalLocationWindow.ShowDialog();
        }
    }
}
