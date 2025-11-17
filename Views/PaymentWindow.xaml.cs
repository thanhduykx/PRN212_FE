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

            InitializeOrderInfo();
            CalculateTotal();
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
            double subTotal = _order.SubTotal ?? 0;
            double deposit = _order.Deposit ?? 0;
            double extraFee = _order.ExtraFee ?? 0;
            double damageFee = _order.DamageFee ?? 0;
            double discount = _order.Discount ?? 0;
            // Tổng tiền = Deposit + SubTotal (theo yêu cầu)
            double total = deposit + subTotal;

            SubTotalTextBlock.Text = $"Tạm tính: {subTotal:N0} VNĐ";
            ExtraFeeTextBlock.Text = $"Phí phát sinh: {extraFee:N0} VNĐ";
            DamageFeeTextBlock.Text = $"Phí hư hỏng: {damageFee:N0} VNĐ";
            DiscountTextBlock.Text = $"Giảm giá: {discount:N0} VNĐ";
            TotalAmountTextBlock.Text = $"{total:N0} VNĐ";
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
                // Tổng tiền = Deposit + SubTotal (theo yêu cầu)
                double total = (_order.Deposit ?? 0) + (_order.SubTotal ?? 0);
                string paymentMethod = (PaymentMethodComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Tiền mặt";

                var payment = new CreatePaymentDTO
                {
                    PaymentDate = DateTime.Now,
                    PaymentType = PaymentType.OrderPayment, // Sửa từ FullPayment sang OrderPayment
                    Amount = total,
                    PaymentMethod = paymentMethod,
                    BillingImageUrl = BillingImageUrlTextBox.Text.Trim(),
                    UserId = _userId,
                    RentalOrderId = _order.Id
                };

                var createdPayment = await _paymentService.CreateAsync(payment);

                if (createdPayment != null)
                {
                    // Xác nhận thanh toán
                    var confirmResult = await _paymentService.ConfirmDepositPaymentAsync(_order.Id);
                    
                    if (confirmResult)
                    {
                        // Sau khi thanh toán thành công, chuyển status sang Completed (8)
                        var rentalOrderService = new RentalOrderService(_apiService);
                        var updatedOrder = await rentalOrderService.UpdateOrderStatusAsync(_order.Id, RentalOrderStatus.Completed);
                        
                        if (updatedOrder != null)
                        {
                            MessageBox.Show("Thanh toán thành công! Đơn hàng đã được hoàn thành. Cảm ơn bạn đã sử dụng dịch vụ.", 
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true; // Đánh dấu thanh toán thành công để reload danh sách
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Thanh toán thành công nhưng cập nhật trạng thái thất bại. Vui lòng liên hệ nhân viên.", 
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Thanh toán đã được ghi nhận nhưng xác nhận thất bại. Vui lòng liên hệ nhân viên.", 
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Thanh toán thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

