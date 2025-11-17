using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Linq;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class CreateFeedbackWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly FeedbackService _feedbackService;
        private readonly int _carId;
        private readonly int _userId;
        private readonly string? _userRole;

        public CreateFeedbackWindow(ApiService apiService, int carId, int userId, string? userRole = null)
        {
            InitializeComponent();
            _apiService = apiService;
            _feedbackService = new FeedbackService(_apiService);
            _carId = carId;
            _userId = userId;
            _userRole = userRole;
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập tiêu đề đánh giá.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ContentTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập nội dung đánh giá.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var rentalOrderService = new RentalOrderService(_apiService);
                int rentalOrderId = 0;

                // Tất cả users (Staff, Admin, Customer) đều không cần đơn hàng để đánh giá
                // Tìm một đơn hàng bất kỳ của xe này để gán vào RentalOrderId (backend yêu cầu)
                var allOrders = await rentalOrderService.GetAllAsync();
                var orderForThisCar = allOrders
                    .Where(o => o.CarId == _carId)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault();

                if (orderForThisCar != null)
                {
                    rentalOrderId = orderForThisCar.Id;
                }
                else
                {
                    // Nếu không có đơn hàng nào của xe này, tìm đơn hàng đầu tiên bất kỳ
                    var anyOrder = allOrders.FirstOrDefault();
                    if (anyOrder != null)
                    {
                        rentalOrderId = anyOrder.Id;
                    }
                    else
                    {
                        // Nếu không có đơn hàng nào trong hệ thống, set = 0
                        // Backend sẽ xử lý hoặc có thể cần tạo một đơn hàng mặc định
                        rentalOrderId = 0;
                    }
                }

                var feedback = new CreateFeedbackDTO
                {
                    Title = TitleTextBox.Text.Trim(),
                    Content = ContentTextBox.Text.Trim(),
                    RentalOrderId = rentalOrderId
                };

                var createdFeedback = await _feedbackService.CreateAsync(feedback);

                if (createdFeedback != null)
                {
                    MessageBox.Show("Đánh giá đã được gửi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gửi đánh giá thất bại. Vui lòng thử lại.\n\nCó thể endpoint API không tồn tại hoặc có lỗi kết nối.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                string errorMsg = $"Lỗi HTTP: {httpEx.Message}";
                if (httpEx.Data.Contains("StatusCode"))
                {
                    errorMsg += $"\nMã lỗi: {httpEx.Data["StatusCode"]}";
                }
                if (httpEx.Data.Contains("ErrorContent"))
                {
                    errorMsg += $"\nChi tiết: {httpEx.Data["ErrorContent"]}";
                }
                MessageBox.Show(errorMsg, "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\n\nChi tiết: {ex.GetType().Name}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

