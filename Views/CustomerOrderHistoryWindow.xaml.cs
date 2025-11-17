using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class CustomerOrderHistoryWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly RentalOrderService _rentalOrderService;
        private readonly int _userId;
        public ObservableCollection<RentalOrderDTO> Orders { get; set; } = new ObservableCollection<RentalOrderDTO>();

        public CustomerOrderHistoryWindow(ApiService apiService, int userId)
        {
            InitializeComponent();
            _apiService = apiService;
            _rentalOrderService = new RentalOrderService(_apiService);
            _userId = userId;
            OrdersDataGrid.ItemsSource = Orders;
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                var orders = await _rentalOrderService.GetByUserIdAsync(_userId);
                Orders.Clear();
                foreach (var order in orders.OrderByDescending(o => o.OrderDate))
                    Orders.Add(order);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Có thể hiển thị chi tiết đơn hàng
        }

        private void ViewDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RentalOrderDTO order)
            {
                var detailWindow = new OrderDetailWindow(_apiService, order);
                detailWindow.ShowDialog();
            }
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RentalOrderDTO order)
            {
                // Kiểm tra status có thể thanh toán: chỉ PaymentPending (6)
                bool canPay = false;
                
                // Kiểm tra theo string
                if (order.Status.Equals("PaymentPending", StringComparison.OrdinalIgnoreCase))
                {
                    canPay = true;
                }
                // Kiểm tra theo số (enum value)
                else if (order.Status == "6")
                {
                    canPay = true;
                }
                // Kiểm tra theo enum
                else
                {
                    var statusEnum = order.GetStatusEnum();
                    if (statusEnum == RentalOrderStatus.PaymentPending)
                    {
                        canPay = true;
                    }
                }
                
                if (canPay)
                {
                    var paymentWindow = new PaymentWindow(_apiService, order, _userId);
                    var result = paymentWindow.ShowDialog();
                    // Reload danh sách sau khi thanh toán (kể cả khi đóng window)
                    if (result == true || result == null)
                    {
                        LoadOrders();
                    }
                }
                else
                {
                    MessageBox.Show($"Đơn hàng này không thể thanh toán.\nTrạng thái hiện tại: {order.Status}\nChỉ có thể thanh toán khi đơn hàng ở trạng thái 'PaymentPending'.", 
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}

