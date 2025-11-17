using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Linq;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class StaffDashboardWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly int _userId;
        private readonly int? _locationId;

        public StaffDashboardWindow(ApiService apiService, int userId, int? locationId = null)
        {
            InitializeComponent();
            _apiService = apiService;
            _userId = userId;
            _locationId = locationId;
            LoadQuickInfo();
        }

        private async void LoadQuickInfo()
        {
            try
            {
                var rentalOrderService = new RentalOrderService(_apiService);
                var allOrders = await rentalOrderService.GetAllAsync();
                
                var today = DateTime.Now.Date;
                var todayDeliveries = allOrders.Count(o => o.PickupTime.Date == today);
                var todayReturns = allOrders.Count(o => o.ExpectedReturnTime.Date == today);
                
                TodayDeliveriesTextBlock.Text = $"Số lượt giao: {todayDeliveries}";
                TodayReturnsTextBlock.Text = $"Số lượt nhận: {todayReturns}";
                
                // Đếm xe có sẵn (cần lấy từ CarRentalLocationService)
                if (_locationId.HasValue)
                {
                    var carLocationService = new CarRentalLocationService(_apiService);
                    var carLocations = await carLocationService.GetByRentalLocationIdAsync(_locationId.Value);
                    int availableCars = carLocations.Sum(cl => cl.Quantity);
                    AvailableCarsTextBlock.Text = $"Xe có sẵn: {availableCars}";
                }
                
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
                
                // Đơn chờ xử lý
                var pendingOrders = allOrders.Where(o => o.Status == "Pending").OrderByDescending(o => o.OrderDate).Take(5).ToList();
                PendingOrdersListBox.ItemsSource = pendingOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            var deliveryWindow = new CarDeliveryHistoryListWindow(_apiService);
            deliveryWindow.ShowDialog();
        }

        private void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            var returnWindow = new CarReturnHistoryListWindow(_apiService);
            returnWindow.ShowDialog();
        }

        private void ManageCarsButton_Click(object sender, RoutedEventArgs e)
        {
            var carLocationWindow = new CarRentalLocationListWindow(_apiService);
            carLocationWindow.ShowDialog();
        }

        private void VerifyCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở window quản lý đơn hàng (có thể xem thông tin khách hàng ở đây)
            var orderWindow = new StaffOrderManagementWindow(_apiService);
            orderWindow.ShowDialog();
        }

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new StaffOrderManagementWindow(_apiService);
            orderWindow.ShowDialog();
        }

        private void ActiveRentalsButton_Click(object sender, RoutedEventArgs e)
        {
            var activeRentalsWindow = new ActiveRentalListWindow(_apiService);
            activeRentalsWindow.ShowDialog();
        }
    }
}

