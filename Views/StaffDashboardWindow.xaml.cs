using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class StaffDashboardWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly int _userId;
        private readonly int? _locationId;
        private readonly CarService _carService;
        private Dictionary<int, CarDTO> _carsCache = new Dictionary<int, CarDTO>();

        public StaffDashboardWindow(ApiService apiService, int userId, int? locationId = null)
        {
            InitializeComponent();
            _apiService = apiService;
            _userId = userId;
            _locationId = locationId;
            _carService = new CarService(_apiService);
            
            // Reload khi window được focus lại (sau khi đóng các window con)
            this.Activated += StaffDashboardWindow_Activated;
            
            LoadQuickInfo();
        }

        private void StaffDashboardWindow_Activated(object sender, EventArgs e)
        {
            // Reload thông tin khi window được focus lại để cập nhật doanh thu
            LoadQuickInfo();
        }

        private async void LoadQuickInfo()
        {
            try
            {
                var rentalOrderService = new RentalOrderService(_apiService);
                var allOrders = await rentalOrderService.GetAllAsync();
                
                // Load TẤT CẢ thông tin xe (không chỉ từ orders) để tính xe có sẵn chính xác
                await LoadCars(new List<int>()); // Load tất cả xe
                
                // Tính lại SubTotal và Deposit cho tất cả orders nếu cần (giống như RentalOrderListWindow)
                foreach (var order in allOrders)
                {
                    if ((order.SubTotal == null || order.SubTotal == 0) || (order.Deposit == null || order.Deposit == 0))
                    {
                        if (_carsCache.ContainsKey(order.CarId))
                        {
                            var car = _carsCache[order.CarId];
                            int days = (order.ExpectedReturnTime - order.PickupTime).Days + 1;
                            double pricePerDay = car.RentPricePerDay;
                            double pricePerDayWithDriver = car.RentPricePerDayWithDriver;
                            double driverFeePerDay = pricePerDayWithDriver - pricePerDay;
                            double totalDriverFee = order.WithDriver ? driverFeePerDay * days : 0;
                            
                            if (order.SubTotal == null || order.SubTotal == 0)
                            {
                                order.SubTotal = (days * pricePerDay) + totalDriverFee;
                            }
                            
                            if (order.Deposit == null || order.Deposit == 0)
                            {
                                order.Deposit = car.DepositAmount;
                            }
                        }
                    }
                }
                
                var today = DateTime.Now.Date;
                var todayDeliveries = allOrders.Count(o => o.PickupTime.Date == today);
                var todayReturns = allOrders.Count(o => o.ExpectedReturnTime.Date == today);
                
                TodayDeliveriesTextBlock.Text = $"Số lượt giao: {todayDeliveries}";
                TodayReturnsTextBlock.Text = $"Số lượt nhận: {todayReturns}";
                
                // Đếm xe có sẵn: Tổng số xe trừ đi số xe đang được thuê (status = Renting/4)
                int totalCars = _carsCache.Count;
                var rentingOrders = allOrders.Where(o => 
                    o.Status == "4" || 
                    o.Status.Equals("Renting", StringComparison.OrdinalIgnoreCase) || 
                    o.GetStatusEnum() == RentalOrderStatus.Renting).ToList();
                int rentedCars = rentingOrders.Select(o => o.CarId).Distinct().Count();
                int availableCars = totalCars - rentedCars;
                
                System.Diagnostics.Debug.WriteLine($"StaffDashboard: Total cars = {totalCars}, Rented cars = {rentedCars}, Available cars = {availableCars}");
                
                AvailableCarsTextBlock.Text = $"Xe có sẵn: {availableCars}";
                
                // Tính doanh thu từ orders có status == 8 (Completed)
                var completedOrders = GetCompletedOrders(allOrders);
                
                // Debug log để kiểm tra
                System.Diagnostics.Debug.WriteLine($"StaffDashboard: Total orders = {allOrders.Count}, Completed orders = {completedOrders.Count}");
                
                if (completedOrders.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"StaffDashboard: Sample completed orders:");
                    foreach (var order in completedOrders.Take(5))
                    {
                        System.Diagnostics.Debug.WriteLine($"  Order #{order.Id}: Status={order.Status}, SubTotal={order.SubTotal}, Deposit={order.Deposit}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("StaffDashboard: No completed orders found!");
                    // Kiểm tra tất cả status để debug
                    var statusGroups = allOrders.GroupBy(o => o.Status).ToList();
                    foreach (var group in statusGroups)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Status '{group.Key}': {group.Count()} orders");
                    }
                }
                
                // Doanh thu = Deposit + SubTotal (theo yêu cầu)
                double totalRevenue = CalculateRevenue(completedOrders);
                
                var thisMonthOrders = completedOrders
                    .Where(o => o.OrderDate.Year == DateTime.Now.Year && o.OrderDate.Month == DateTime.Now.Month)
                    .ToList();
                
                double thisMonthRevenue = CalculateRevenue(thisMonthOrders);
                
                // Đảm bảo hiển thị giá trị thực tế (không phải 0)
                RevenueTextBlock.Text = $"Tổng doanh thu: {totalRevenue:N0} VNĐ";
                ThisMonthRevenueTextBlock.Text = $"Doanh thu tháng này: {totalRevenue:N0} VNĐ";
                
                System.Diagnostics.Debug.WriteLine($"StaffDashboard: Total Revenue = {totalRevenue:N0} VNĐ, This Month = {thisMonthRevenue:N0} VNĐ");
                
                // Đơn chờ xử lý
                var pendingOrders = allOrders.Where(o => o.Status == "Pending").OrderByDescending(o => o.OrderDate).Take(5).ToList();
                PendingOrdersListBox.ItemsSource = pendingOrders;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StaffDashboard LoadQuickInfo error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Lỗi tải thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // Vẫn hiển thị 0 nếu có lỗi
                RevenueTextBlock.Text = "Tổng doanh thu: 0 VNĐ";
                ThisMonthRevenueTextBlock.Text = "Doanh thu tháng này: 0 VNĐ";
            }
        }

        // Helper method: Lấy danh sách orders có status Completed (8)
        private List<RentalOrderDTO> GetCompletedOrders(List<RentalOrderDTO> allOrders)
        {
            return allOrders
                .Where(o => o.Status == "8" || 
                           o.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) || 
                           o.GetStatusEnum() == RentalOrderStatus.Completed)
                .ToList();
        }

        // Helper method: Tính doanh thu từ danh sách orders (Deposit + SubTotal)
        private double CalculateRevenue(List<RentalOrderDTO> orders)
        {
            if (orders == null || orders.Count == 0)
                return 0;
            
            double revenue = 0;
            foreach (var order in orders)
            {
                double deposit = order.Deposit ?? 0;
                double subTotal = order.SubTotal ?? 0;
                double orderRevenue = deposit + subTotal;
                revenue += orderRevenue;
                
                // Debug log cho từng order
                if (deposit > 0 || subTotal > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Order #{order.Id}: Deposit={deposit:N0}, SubTotal={subTotal:N0}, Revenue={orderRevenue:N0}");
                }
            }
            
            return revenue;
        }

        private async Task LoadCars(List<int> carIds)
        {
            try
            {
                // Luôn load tất cả xe để có tổng số xe chính xác
                var allCars = await _carService.GetAllCarsAsync();
                _carsCache.Clear(); // Clear cache để đảm bảo dữ liệu mới nhất
                foreach (var car in allCars)
                {
                    _carsCache[car.Id] = car;
                }
                
                System.Diagnostics.Debug.WriteLine($"StaffDashboard: Loaded {_carsCache.Count} cars from service");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cars in StaffDashboard: {ex.Message}");
            }
        }

        private void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            var returnWindow = new CarReturnHistoryListWindow(_apiService);
            returnWindow.ShowDialog();
            // Reload sau khi đóng window để cập nhật doanh thu
            LoadQuickInfo();
        }
        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            var deliveryWindow = new CarDeliveryHistoryListWindow(_apiService);
            deliveryWindow.ShowDialog();
            // Reload sau khi đóng window để cập nhật doanh thu
            LoadQuickInfo();
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

