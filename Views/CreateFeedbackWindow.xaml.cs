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

        public CreateFeedbackWindow(ApiService apiService, int carId, int userId)
        {
            InitializeComponent();
            _apiService = apiService;
            _feedbackService = new FeedbackService(_apiService);
            _carId = carId;
            _userId = userId;
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

                // Lấy đơn hàng gần nhất của user với carId này
                var rentalOrderService = new RentalOrderService(_apiService);
                var userOrders = await rentalOrderService.GetByUserIdAsync(_userId);
                var orderForThisCar = userOrders
                    .Where(o => o.CarId == _carId)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault();

                if (orderForThisCar == null)
                {
                    MessageBox.Show("Bạn chưa có đơn hàng nào cho xe này. Vui lòng thuê xe trước khi đánh giá.", 
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var feedback = new CreateFeedbackDTO
                {
                    Title = TitleTextBox.Text.Trim(),
                    Content = ContentTextBox.Text.Trim(),
                    RentalOrderId = orderForThisCar.Id
                };

                var createdFeedback = await _feedbackService.CreateAsync(feedback);

                if (createdFeedback != null)
                {
                    MessageBox.Show("Đánh giá đã được gửi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gửi đánh giá thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

