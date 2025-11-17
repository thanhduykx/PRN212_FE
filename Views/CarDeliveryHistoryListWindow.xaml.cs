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
    public partial class CarDeliveryHistoryListWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly CarService _carService;
        private readonly RentalOrderService _rentalOrderService;

        // ObservableCollection bind DataGrid - hiển thị orders status == 3
        public ObservableCollection<RentalOrderDTO> Orders { get; set; } = new ObservableCollection<RentalOrderDTO>();
        private List<RentalOrderDTO> _allOrders = new List<RentalOrderDTO>(); // Lưu danh sách gốc để filter
        private RentalOrderDTO? _selectedOrder;
        private Dictionary<int, string> _carNamesCache = new Dictionary<int, string>(); // Cache tên xe
        private Dictionary<int, CarDTO> _carsCache = new Dictionary<int, CarDTO>(); // Cache thông tin xe để tính phí tài xế

        public CarDeliveryHistoryListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _carService = new CarService(apiService);
            _rentalOrderService = new RentalOrderService(apiService);

            CarDeliveryHistoryDataGrid.ItemsSource = Orders;

            // Load dữ liệu
            this.Loaded += CarDeliveryHistoryListWindow_Loaded;
        }

        private async void CarDeliveryHistoryListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHistories();
        }

        private async Task LoadHistories()
        {
            try
            {
                // Hiển thị orders có status == "3" (Confirmed) - chờ giao xe
                var allOrders = await _rentalOrderService.GetAllAsync();
                var confirmedOrders = allOrders
                    .Where(o => o.Status == "3" || o.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                // Load thông tin xe cho tất cả CarId
                var carIds = confirmedOrders.Select(o => o.CarId).Distinct().ToList();
                await LoadCarNames(carIds);
                await LoadCars(carIds);
                
                // Gán tên xe và tính phí tài xế vào từng order
                foreach (var order in confirmedOrders)
                {
                    if (_carNamesCache.ContainsKey(order.CarId))
                    {
                        order.CarName = _carNamesCache[order.CarId];
                    }
                    else
                    {
                        order.CarName = $"Car #{order.CarId}";
                    }
                    
                    // Debug: Kiểm tra SubTotal từ backend
                    System.Diagnostics.Debug.WriteLine($"Order #{order.Id}: SubTotal = {order.SubTotal}, Total = {order.Total}");
                    
                    // Tính phí tài xế nếu có tài xế
                    if (order.WithDriver && _carsCache.ContainsKey(order.CarId))
                    {
                        var car = _carsCache[order.CarId];
                        int days = (order.ExpectedReturnTime - order.PickupTime).Days + 1;
                        double driverFee = (car.RentPricePerDayWithDriver - car.RentPricePerDay) * days;
                        order.DriverFeeText = $"{driverFee:N0} VNĐ";
                    }
                    else
                    {
                        order.DriverFeeText = "0 VNĐ";
                    }
                }
                
                _allOrders = confirmedOrders.ToList();
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

        private async Task LoadCars(List<int> carIds)
        {
            try
            {
                var allCars = await _carService.GetAllCarsAsync();
                foreach (var car in allCars)
                {
                    if (!_carsCache.ContainsKey(car.Id))
                    {
                        _carsCache[car.Id] = car;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cars: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";
            
            var filtered = _allOrders.Where(order =>
                string.IsNullOrWhiteSpace(searchText) ||
                order.Id.ToString().Contains(searchText) ||
                order.CarId.ToString().Contains(searchText) ||
                (order.CarName != null && order.CarName.ToLower().Contains(searchText)) ||
                order.PhoneNumber.ToLower().Contains(searchText) ||
                order.Status.ToLower().Contains(searchText)
            ).ToList();

            Orders.Clear();
            foreach (var order in filtered)
                Orders.Add(order);

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = Orders.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CarDeliveryHistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarDeliveryHistoryDataGrid.SelectedItem is RentalOrderDTO selectedOrder)
            {
                _selectedOrder = selectedOrder;
                DeliverCarButton.IsEnabled = true;
            }
            else
            {
                _selectedOrder = null;
                DeliverCarButton.IsEnabled = false;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadHistories();
        }

        private async void DeliverCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần giao xe.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn giao xe cho đơn hàng #{_selectedOrder.Id}?\n\nTên xe: {_selectedOrder.CarName}\n\nĐơn hàng sẽ được chuyển sang trạng thái 'Đang cho thuê'.",
                "Xác nhận giao xe",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DeliverCarButton.IsEnabled = false;
                    DeliverCarButton.Content = "Đang xử lý...";

                    // Chuyển order status sang 4 (Renting)
                    var updatedOrder = await _rentalOrderService.UpdateOrderStatusAsync(_selectedOrder.Id, RentalOrderStatus.Renting);

                    if (updatedOrder != null)
                    {
                        MessageBox.Show("Giao xe thành công! Đơn hàng đã được chuyển sang trạng thái 'Đang cho thuê'.", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadHistories();
                    }
                    else
                    {
                        MessageBox.Show("Giao xe thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DeliverCarButton.IsEnabled = true;
                    DeliverCarButton.Content = "🚗 Giao xe";
                }
            }
        }
    }
}

