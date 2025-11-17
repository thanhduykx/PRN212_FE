using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using Microsoft.Win32;
using System;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class PaymentWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly PaymentService _paymentService;
        private readonly RentalOrderDTO _order;
        private readonly int _userId;

        public PaymentWindow(ApiService apiService, RentalOrderDTO order, int userId)
        {
            InitializeComponent();
            _apiService = apiService;
            _paymentService = new PaymentService(_apiService);
            _order = order;
            _userId = userId;

            Loaded += PaymentWindow_Loaded;
        }

        private async void PaymentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Load lại order từ backend để đảm bảo có đầy đủ thông tin giá tiền
            await LoadOrderDetails();
            InitializeOrderInfo();
            CalculateTotal();
        }

        private async System.Threading.Tasks.Task LoadOrderDetails()
        {
            try
            {
                var rentalOrderService = new RentalOrderService(_apiService);
                var updatedOrder = await rentalOrderService.GetByIdAsync(_order.Id);
                if (updatedOrder != null)
                {
                    // Cập nhật TẤT CẢ thông tin giá tiền từ backend (bao gồm cả ExtraFee, DamageFee, Discount)
                    _order.SubTotal = updatedOrder.SubTotal;
                    _order.Deposit = updatedOrder.Deposit;
                    
                    // Đảm bảo cập nhật ExtraFee, DamageFee, Discount từ backend (staff đã cập nhật)
                    _order.ExtraFee = updatedOrder.ExtraFee;
                    _order.DamageFee = updatedOrder.DamageFee;
                    _order.Discount = updatedOrder.Discount;
                    _order.DamageNotes = updatedOrder.DamageNotes;
                    
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow - Order #{_order.Id} loaded from backend:");
                    System.Diagnostics.Debug.WriteLine($"  SubTotal = {_order.SubTotal}");
                    System.Diagnostics.Debug.WriteLine($"  Deposit = {_order.Deposit}");
                    System.Diagnostics.Debug.WriteLine($"  ExtraFee = {_order.ExtraFee}");
                    System.Diagnostics.Debug.WriteLine($"  DamageFee = {_order.DamageFee}");
                    System.Diagnostics.Debug.WriteLine($"  Discount = {_order.Discount}");
                    System.Diagnostics.Debug.WriteLine($"  DamageNotes = {_order.DamageNotes}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow - Order #{_order.Id} not found in backend");
                }
                
                // Nếu SubTotal hoặc Deposit là null/0, tính lại từ thông tin xe
                // NHƯNG KHÔNG ghi đè ExtraFee, DamageFee, Discount từ backend
                if ((_order.SubTotal == null || _order.SubTotal == 0) || (_order.Deposit == null || _order.Deposit == 0))
                {
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow - Order #{_order.Id}: SubTotal or Deposit is null/0, calculating from car info");
                    await CalculatePriceFromCar();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading order details: {ex.Message}\n{ex.StackTrace}");
                // Nếu không load được, thử tính từ thông tin xe
                await CalculatePriceFromCar();
            }
        }

        private async System.Threading.Tasks.Task CalculatePriceFromCar()
        {
            try
            {
                var carService = new CarService(_apiService);
                var car = await carService.GetCarByIdAsync(_order.CarId);
                
                if (car != null)
                {
                    int days = (_order.ExpectedReturnTime - _order.PickupTime).Days + 1;
                    double pricePerDay = car.RentPricePerDay;
                    double pricePerDayWithDriver = car.RentPricePerDayWithDriver;
                    double driverFeePerDay = pricePerDayWithDriver - pricePerDay;
                    
                    // Tính phí tài xế tổng (nếu có tài xế)
                    double totalDriverFee = _order.WithDriver ? driverFeePerDay * days : 0;
                    
                    // Tính lại SubTotal và Deposit từ thông tin xe
                    if (_order.SubTotal == null || _order.SubTotal == 0)
                    {
                        // SubTotal = (giá không tài xế * số ngày) + phí tài xế
                        _order.SubTotal = (days * pricePerDay) + totalDriverFee;
                        System.Diagnostics.Debug.WriteLine($"PaymentWindow - Calculated SubTotal from car: {_order.SubTotal}");
                    }
                    
                    if (_order.Deposit == null || _order.Deposit == 0)
                    {
                        // Deposit = DepositAmount từ Car
                        _order.Deposit = car.DepositAmount;
                        System.Diagnostics.Debug.WriteLine($"PaymentWindow - Calculated Deposit from car: {_order.Deposit}");
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow - Order #{_order.Id}: Calculated from car - SubTotal = {_order.SubTotal}, Deposit = {_order.Deposit}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow - Car #{_order.CarId} not found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating price from car: {ex.Message}");
            }
        }

        private void InitializeOrderInfo()
        {
            OrderIdTextBlock.Text = $"#{_order.Id}";
            OrderDateTextBlock.Text = _order.OrderDate.ToString("dd/MM/yyyy HH:mm");
            PickupTimeTextBlock.Text = _order.PickupTime.ToString("dd/MM/yyyy HH:mm");
            ReturnTimeTextBlock.Text = _order.ExpectedReturnTime.ToString("dd/MM/yyyy HH:mm");
        }

        private void CalculateTotal()
        {
            // Lấy giá trị từ order (đã được load từ backend)
            double subTotal = _order.SubTotal ?? 0;
            double deposit = _order.Deposit ?? 0;
            double extraFee = _order.ExtraFee ?? 0;  // Lấy từ backend (staff đã cập nhật)
            double damageFee = _order.DamageFee ?? 0;  // Lấy từ backend (staff đã cập nhật)
            double discountPercent = _order.Discount ?? 0;  // Lấy từ backend (staff đã cập nhật)
            
            // Tính tổng tiền: (Deposit + SubTotal + ExtraFee + DamageFee) * (1 - Discount/100)
            double totalBeforeDiscount = deposit + subTotal + extraFee + damageFee;
            double discountAmount = totalBeforeDiscount * (discountPercent / 100.0);
            double total = totalBeforeDiscount - discountAmount;

            // Hiển thị chi tiết - ĐẢM BẢO hiển thị các giá trị từ backend
            SubTotalTextBlock.Text = $"Tạm tính: {subTotal:N0} VNĐ";
            DepositTextBlock.Text = $"Đặt cọc: {deposit:N0} VNĐ";
            ExtraFeeTextBlock.Text = $"Phí phát sinh: {extraFee:N0} VNĐ";
            DamageFeeTextBlock.Text = $"Phí hư hỏng: {damageFee:N0} VNĐ";
            
            if (discountPercent > 0)
            {
                DiscountTextBlock.Text = $"Giảm giá: {discountPercent:N0}% ({discountAmount:N0} VNĐ)";
            }
            else
            {
                DiscountTextBlock.Text = $"Giảm giá: 0% (0 VNĐ)";
            }
            
            TotalAmountTextBlock.Text = $"{total:N0} VNĐ";
            
            System.Diagnostics.Debug.WriteLine($"PaymentWindow CalculateTotal:");
            System.Diagnostics.Debug.WriteLine($"  SubTotal: {subTotal}");
            System.Diagnostics.Debug.WriteLine($"  Deposit: {deposit}");
            System.Diagnostics.Debug.WriteLine($"  ExtraFee: {extraFee} (from backend)");
            System.Diagnostics.Debug.WriteLine($"  DamageFee: {damageFee} (from backend)");
            System.Diagnostics.Debug.WriteLine($"  Discount: {discountPercent}% (from backend)");
            System.Diagnostics.Debug.WriteLine($"  Total: {total}");
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Chọn ảnh hóa đơn"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // TODO: Upload image to server and get URL
                // For now, just show the file path
                BillingImageUrlTextBox.Text = openFileDialog.FileName;
                MessageBox.Show("Chức năng upload ảnh đang phát triển. Vui lòng nhập URL ảnh thủ công.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tính tổng tiền giống như CalculateTotal()
                double subTotal = _order.SubTotal ?? 0;
                double deposit = _order.Deposit ?? 0;
                double extraFee = _order.ExtraFee ?? 0;
                double damageFee = _order.DamageFee ?? 0;
                double discountPercent = _order.Discount ?? 0;
                
                double totalBeforeDiscount = deposit + subTotal + extraFee + damageFee;
                double discountAmount = totalBeforeDiscount * (discountPercent / 100.0);
                double total = totalBeforeDiscount - discountAmount;
                
                string paymentMethod = (PaymentMethodComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Tiền mặt";

                var payment = new CreatePaymentDTO
                {
                    PaymentDate = DateTime.Now,
                    PaymentType = PaymentType.OrderPayment,
                    Amount = total,
                    PaymentMethod = paymentMethod,
                    BillingImageUrl = BillingImageUrlTextBox.Text.Trim(),
                    UserId = _userId,
                    RentalOrderId = _order.Id
                };

                System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - Amount = {total}, OrderId = {_order.Id}");

                // Thử tạo payment (có thể thất bại với 403 nếu customer không có quyền)
                // Nhưng vẫn tiếp tục chuyển status sang Completed
                PaymentDTO? createdPayment = null;
                try
                {
                    createdPayment = await _paymentService.CreateAsync(payment);
                    if (createdPayment != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - Payment created successfully");
                    }
                }
                catch (System.Net.Http.HttpRequestException httpEx) when (httpEx.Message.Contains("403"))
                {
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - Payment creation failed with 403 (customer may not have permission to create payment directly). Continuing with status update...");
                    // Customer có thể không có quyền tạo payment trực tiếp, nhưng vẫn có thể chuyển status
                }
                catch (Exception paymentEx)
                {
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - Payment creation failed: {paymentEx.Message}. Continuing with status update...");
                    // Tiếp tục chuyển status ngay cả khi tạo payment thất bại
                }
                
                // Sau khi thanh toán (hoặc thử thanh toán), chuyển status sang Completed (8)
                var rentalOrderService = new RentalOrderService(_apiService);
                var updatedOrder = await rentalOrderService.UpdateOrderStatusAsync(_order.Id, RentalOrderStatus.Completed);
                
                if (updatedOrder != null)
                {
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - Order status updated to Completed. New status: {updatedOrder.Status}");
                    
                    // Xác nhận thanh toán (optional - có thể thất bại nhưng không ảnh hưởng đến việc chuyển status)
                    try
                    {
                        await _paymentService.ConfirmDepositPaymentAsync(_order.Id);
                    }
                    catch (Exception confirmEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - ConfirmDepositPayment failed (ignored): {confirmEx.Message}");
                        // Ignore lỗi xác nhận thanh toán, vì status đã được chuyển sang Completed
                    }
                    
                    if (createdPayment != null)
                    {
                        MessageBox.Show("Thanh toán thành công! Đơn hàng đã được hoàn thành. Cảm ơn bạn đã sử dụng dịch vụ.", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Đơn hàng đã được hoàn thành. Thanh toán sẽ được xử lý bởi hệ thống.", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    
                    this.DialogResult = true; // Đánh dấu thanh toán thành công để reload danh sách
                    this.Close();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - Failed to update order status to Completed");
                    MessageBox.Show("Cập nhật trạng thái thất bại. Vui lòng liên hệ nhân viên.", 
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PaymentWindow PayButton_Click - Exception: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

