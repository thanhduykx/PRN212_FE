using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                var completedOrders = GetCompletedOrders(allOrders);
                
                // Doanh thu = Deposit + SubTotal (theo yêu cầu)
                double totalRevenue = CalculateRevenue(completedOrders);
                
                var thisMonthOrders = completedOrders
                    .Where(o => o.OrderDate.Year == DateTime.Now.Year && o.OrderDate.Month == DateTime.Now.Month)
                    .ToList();
                
                double thisMonthRevenue = CalculateRevenue(thisMonthOrders);
                
                RevenueTextBlock.Text = $"Tổng doanh thu: {totalRevenue:N0} VNĐ";
                ThisMonthRevenueTextBlock.Text = $"Doanh thu tháng này: {thisMonthRevenue:N0} VNĐ";
                
                // Load doanh thu theo địa điểm từ API
                await LoadRevenueByLocation();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading revenue: {ex.Message}");
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
            return orders.Sum(o => (o.Deposit ?? 0) + (o.SubTotal ?? 0));
        }

        private async System.Threading.Tasks.Task LoadRevenueByLocation()
        {
            try
            {
                var paymentService = new PaymentService(_apiService);
                var revenueByLocation = await paymentService.GetRevenueByLocationAsync();
                
                RevenueByLocationPanel.Children.Clear();
                
                if (revenueByLocation != null && revenueByLocation.Any())
                {
                    foreach (var location in revenueByLocation.OrderByDescending(l => l.TotalRevenue))
                    {
                        var locationBorder = new Border
                        {
                            Background = Brushes.White,
                            CornerRadius = new CornerRadius(3),
                            Padding = new Thickness(8, 5, 8, 5),
                            Margin = new Thickness(0, 0, 0, 5)
                        };
                        
                        var locationStack = new StackPanel();
                        
                        var locationNameText = new TextBlock
                        {
                            Text = $"{location.LocationName}",
                            FontSize = 12,
                            FontWeight = FontWeights.SemiBold,
                            Foreground = Brushes.Black,
                            Margin = new Thickness(0, 0, 0, 2)
                        };
                        
                        var revenueText = new TextBlock
                        {
                            Text = $"Doanh thu: {location.TotalRevenue:N0} VNĐ | Đơn hàng: {location.TotalOrders}",
                            FontSize = 11,
                            Foreground = Brushes.DarkGreen
                        };
                        
                        locationStack.Children.Add(locationNameText);
                        locationStack.Children.Add(revenueText);
                        locationBorder.Child = locationStack;
                        RevenueByLocationPanel.Children.Add(locationBorder);
                    }
                }
                else
                {
                    var emptyText = new TextBlock
                    {
                        Text = "Chưa có dữ liệu doanh thu theo địa điểm",
                        FontSize = 12,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    RevenueByLocationPanel.Children.Add(emptyText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading revenue by location: {ex.Message}");
                var errorText = new TextBlock
                {
                    Text = $"Lỗi tải dữ liệu: {ex.Message}",
                    FontSize = 12,
                    Foreground = Brushes.Red,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5)
                };
                RevenueByLocationPanel.Children.Clear();
                RevenueByLocationPanel.Children.Add(errorText);
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

        private void ViewAIAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            var aiAnalysisWindow = new AIAnalysisWindow(_apiService);
            aiAnalysisWindow.ShowDialog();
        }
    }
}
