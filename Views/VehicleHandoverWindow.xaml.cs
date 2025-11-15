using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;

namespace AssignmentPRN212.Views
{
    /// <summary>
    /// Vehicle Handover Window
    /// Allows staff to process vehicle delivery to customer
    /// </summary>
    public partial class VehicleHandoverWindow : Window
    {
        private readonly StaffService _staffService;
        private readonly Guid _staffId;
        private readonly int _orderId;
        private readonly int _customerId;
        private readonly int _carId;

        public VehicleHandoverWindow(Guid staffId, int orderId, int customerId, int carId,
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
        /// Load order details and document verification status
        /// </summary>
        private async void LoadOrderDetails()
        {
            try
            {
                // Load order information
                var order = await _staffService.GetOrderByIdAsync(_orderId);

                if (order != null)
                {
                    txtOrderInfo.Text = $"Order Code: {order.OrderCode}\n" +
                                       $"Start Date: {order.StartDate:dd/MM/yyyy}\n" +
                                       $"End Date: {order.EndDate:dd/MM/yyyy}\n" +
                                       $"With Driver: {(order.WithDriver ? "Yes" : "No")}";

                    // Load document verification status
                    await LoadDocumentStatus(order);
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
        /// Load document verification status
        /// </summary>
        private async Task LoadDocumentStatus(RentalOrderDTO order)
        {
            try
            {
                // Check Citizen ID
                if (order.CitizenId.HasValue)
                {
                    var citizenId = await _staffService.GetCitizenIdByOrderAsync(_orderId);
                    chkCitizenId.IsChecked = citizenId?.IsValid ?? false;
                }

                // Check Driver License
                if (order.DriverLicenseId.HasValue)
                {
                    var license = await _staffService.GetDriverLicenseByOrderAsync(_orderId);
                    chkLicense.IsChecked = license?.IsValid ?? false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading documents: {ex.Message}", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Complete handover button clicked
        /// </summary>
        private async void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            // Validation Step 1: Check all documents verified
            if (chkCitizenId.IsChecked != true ||
                chkLicense.IsChecked != true ||
                chkSignature.IsChecked != true)
            {
                MessageBox.Show(
                    "Please verify all documents and obtain customer signature\n" +
                    "Vui lòng xác minh tất cả giấy tờ và lấy chữ ký khách hàng",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validation Step 2: Check mileage is valid
            if (!int.TryParse(txtMileage.Text, out int mileage) || mileage < 0)
            {
                MessageBox.Show(
                    "Please enter valid mileage\nVui lòng nhập số km hợp lệ",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Show loading indicator (optional)
                this.IsEnabled = false;
                this.Cursor = System.Windows.Input.Cursors.Wait;

                // Create handover DTO
                var handoverDto = new CreateCarDeliveryHistoryDTO
                {
                    OrderId = _orderId,
                    CustomerId = _customerId,
                    StaffId = (int)_staffId.GetHashCode(), // Note: This is a workaround since Staff ID is Guid
                    CarId = _carId,
                    DeliveryDate = DateTime.Now,
                    VehicleCondition = ((ComboBoxItem)cmbCondition.SelectedItem).Content.ToString(),
                    BatteryLevel = (int)sliderBattery.Value,
                    Mileage = mileage,
                    PhotoUrls = "https://example.com/handover-photo.jpg", // TODO: Implement photo upload
                    Notes = txtNotes.Text
                };

                // Call API to create delivery history
                var result = await _staffService.CreateDeliveryHistoryAsync(handoverDto);

                if (result != null)
                {
                    MessageBox.Show(
                        "Vehicle handover completed successfully!\n" +
                        "Xe đã được giao cho khách hàng thành công!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Set dialog result to true so parent window knows to refresh
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to complete handover. Please try again.\n" +
                        "Không thể hoàn tất giao xe. Vui lòng thử lại.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error completing handover:\n{ex.Message}",
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