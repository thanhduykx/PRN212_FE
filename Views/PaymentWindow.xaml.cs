using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;

namespace AssignmentPRN212.Views
{
    /// <summary>
    /// Payment Window
    /// Allows staff to process cash payments at the station
    /// </summary>
    public partial class PaymentWindow : Window
    {
        private readonly StaffService _staffService;
        private readonly Guid _staffId;
        private readonly int _orderId;
        private readonly int _customerId;
        private readonly decimal _totalAmount;

        public PaymentWindow(Guid staffId, int orderId, int customerId, decimal totalAmount,
            string apiBaseUrl = "https://localhost:7240/")
        {
            InitializeComponent();

            // Initialize service
            var apiService = new ApiService(apiBaseUrl);
            _staffService = new StaffService(apiService);

            _staffId = staffId;
            _orderId = orderId;
            _customerId = customerId;
            _totalAmount = totalAmount;

            // Display formatted total amount
            txtTotal.Text = totalAmount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " VND";

            // Pre-fill payment amount with order total
            txtAmount.Text = totalAmount.ToString("0");
        }

        /// <summary>
        /// Payment amount textbox changed - format the amount
        /// </summary>
        private void TxtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtAmount.Text, out decimal amount))
            {
                // Show formatted amount with thousand separators
                txtFormatted.Text = amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " VND";

                // Recalculate change
                CalculateChange();
            }
            else
            {
                txtFormatted.Text = string.Empty;
            }
        }

        /// <summary>
        /// Received amount textbox changed - recalculate change
        /// </summary>
        private void TxtReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateChange();
        }

        /// <summary>
        /// Calculate change to return to customer
        /// Formula: Change = Received - Payment Amount
        /// </summary>
        private void CalculateChange()
        {
            // Parse both amounts
            if (decimal.TryParse(txtAmount.Text, out decimal amount) &&
                decimal.TryParse(txtReceived.Text, out decimal received))
            {
                // Calculate change
                var change = received - amount;

                // Display formatted change
                txtChange.Text = change.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " VND";

                // Change text color based on value
                if (change < 0)
                {
                    // Negative = customer didn't give enough
                    txtChange.Foreground = System.Windows.Media.Brushes.Red;
                }
                else if (change == 0)
                {
                    // Exact amount
                    txtChange.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    // Positive = return change to customer
                    txtChange.Foreground = System.Windows.Media.Brushes.Orange;
                }
            }
            else
            {
                txtChange.Text = "0 VND";
                txtChange.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        /// <summary>
        /// Process payment button clicked
        /// </summary>
        private async void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            // Validation Step 1: Check amount is valid and greater than 0
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show(
                    "Please enter valid payment amount\nVui lòng nhập số tiền hợp lệ",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validation Step 2: Check received amount
            if (!string.IsNullOrWhiteSpace(txtReceived.Text))
            {
                if (decimal.TryParse(txtReceived.Text, out decimal received))
                {
                    if (received < amount)
                    {
                        var result = MessageBox.Show(
                            $"Customer gave insufficient amount!\nKhách đưa thiếu tiền!\n\n" +
                            $"Required: {amount:N0} VND\nReceived: {received:N0} VND\n\n" +
                            $"Do you want to continue anyway?",
                            "Warning",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                            return;
                    }
                }
            }

            try
            {
                // Show loading indicator
                this.IsEnabled = false;
                this.Cursor = System.Windows.Input.Cursors.Wait;

                // Create payment DTO
                var paymentDto = new CreatePaymentDTO
                {
                    RentalOrderId = _orderId,
                    UserId = _customerId,
                    Amount = amount,
                    Method = "Cash",
                    Status = "Completed",
                    TransactionDate = DateTime.Now
                };

                // Call API to create payment
                var result = await _staffService.CreatePaymentAsync(paymentDto);

                if (result != null)
                {
                    // Calculate change for display
                    var changeAmount = 0m;
                    if (decimal.TryParse(txtReceived.Text, out decimal received))
                    {
                        changeAmount = received - amount;
                    }

                    // Build success message
                    var message = "Payment processed successfully!\nThanh toán thành công!";
                    if (changeAmount > 0)
                    {
                        message += $"\n\nChange to return: {changeAmount:N0} VND\nTiền thừa trả khách: {changeAmount:N0} VND";
                    }

                    MessageBox.Show(message, "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Set dialog result to true so parent window knows to refresh
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to process payment. Please try again.\n" +
                        "Không thể xử lý thanh toán. Vui lòng thử lại.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error processing payment:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                // Hide loading indicator
                this.IsEnabled = true;
                this.Cursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        /// <summary>
        /// Cancel button clicked
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to cancel?\nBạn có chắc muốn hủy?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}