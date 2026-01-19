using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;

namespace AssignmentPRN212.Views
{
    /// <summary>
    /// Vehicle Return Window
    /// Allows staff to process vehicle return from customer
    /// </summary>
    public partial class VehicleReturnWindow : Window
    {
        private readonly StaffService _staffService;
        private readonly Guid _staffId;
        private readonly int _orderId;
        private readonly int _customerId;
        private readonly int _carId;

        public VehicleReturnWindow(Guid staffId, int orderId, int customerId, int carId,
            string apiBaseUrl = "https://localhost:7240/")
        {
            InitializeComponent();

            // Initialize service
            var apiService = new ApiService(apiBaseUrl);
            _staffService = new StaffService(apiService);

            _staffId = staffId;
            _orderId = orderId;
            _customerId = customerId;
            _carId = carId;

            // Load order details
            LoadOrderDetails();
        }

        /// <summary>
        /// Load order details
        /// </summary>
        private async void LoadOrderDetails()
        {
            try
            {
                var order = await _staffService.GetOrderByIdAsync(_orderId);

                if (order != null)
                {
                    txtOrderInfo.Text = $"Order Code: {order.OrderCode}\n" +
                                       $"Start Date: {order.StartDate:dd/MM/yyyy}\n" +
                                       $"End Date: {order.EndDate:dd/MM/yyyy}\n" +
                                       $"Total: {order.Total:N0} VND";
                }
                else
                {
                    MessageBox.Show("Order not found", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Damage checkbox checked - enable damage fields
        /// </summary>
        private void ChkDamage_Checked(object sender, RoutedEventArgs e)
        {
            txtDamage.IsEnabled = true;
            txtCharges.IsEnabled = true;
        }

        /// <summary>
        /// Damage checkbox unchecked - disable and clear damage fields
        /// </summary>
        private void ChkDamage_Unchecked(object sender, RoutedEventArgs e)
        {
            txtDamage.IsEnabled = false;
            txtDamage.Text = string.Empty;
            txtCharges.IsEnabled = false;
            txtCharges.Text = "0";
            txtFormattedCharges.Text = string.Empty;
        }

        /// <summary>
        /// Extra charges textbox changed - format the amount
        /// </summary>
        private void TxtCharges_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtCharges.Text, out decimal amount))
            {
                // Format with thousand separators: 1000000 → 1,000,000 VND
                txtFormattedCharges.Text = amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " VND";
            }
            else
            {
                txtFormattedCharges.Text = string.Empty;
            }
        }

        /// <summary>
        /// Complete return button clicked
        /// </summary>
        private async void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            // Validation Step 1: Check mileage is valid
            if (!int.TryParse(txtMileage.Text, out int mileage) || mileage < 0)
            {
                MessageBox.Show(
                    "Please enter valid mileage\nVui lòng nhập số km hợp lệ",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validation Step 2: Check extra charges is valid
            if (!decimal.TryParse(txtCharges.Text, out decimal charges) || charges < 0)
            {
                MessageBox.Show(
                    "Please enter valid charges\nVui lòng nhập phụ phí hợp lệ",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validation Step 3: If damages checked, ensure damage report is filled
            if (chkDamage.IsChecked == true && string.IsNullOrWhiteSpace(txtDamage.Text))
            {
                MessageBox.Show(
                    "Please provide damage report\nVui lòng mô tả hư hỏng",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Show loading indicator
                this.IsEnabled = false;
                this.Cursor = System.Windows.Input.Cursors.Wait;

                // Create return DTO
                var returnDto = new CreateCarReturnHistoryDTO
                {
                    OrderId = _orderId,
                    CustomerId = _customerId,
                    StaffId = (int)_staffId.GetHashCode(), // Note: Workaround for Guid to int conversion
                    CarId = _carId,
                    ReturnDate = DateTime.Now,
                    VehicleCondition = ((ComboBoxItem)cmbCondition.SelectedItem).Content.ToString(),
                    BatteryLevel = (int)sliderBattery.Value,
                    Mileage = mileage,
                    PhotoUrls = "https://example.com/return-photo.jpg", // TODO: Implement photo upload
                    DamageReport = chkDamage.IsChecked == true ? txtDamage.Text : null,
                    ExtraCharges = charges,
                    Notes = txtNotes.Text
                };

                // Call API to create return history
                var result = await _staffService.CreateReturnHistoryAsync(returnDto);

                if (result != null)
                {
                    // Build success message
                    var message = "Vehicle return completed!\nĐã nhận xe trả thành công!";
                    if (charges > 0)
                    {
                        message += $"\n\nExtra charges: {charges:N0} VND\nPhụ phí: {charges:N0} VND";
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
                        "Failed to complete return. Please try again.\n" +
                        "Không thể hoàn tất nhận xe. Vui lòng thử lại.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error completing return:\n{ex.Message}",
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