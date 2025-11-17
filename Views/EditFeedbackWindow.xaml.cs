using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class EditFeedbackWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly FeedbackService _feedbackService;
        private readonly FeedbackDTO _feedback;

        public EditFeedbackWindow(ApiService apiService, FeedbackDTO feedback)
        {
            InitializeComponent();
            _apiService = apiService;
            _feedbackService = new FeedbackService(_apiService);
            _feedback = feedback;

            TitleTextBox.Text = feedback.Title;
            ContentTextBox.Text = feedback.Content;
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
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

                var updateDto = new UpdateFeedbackDTO
                {
                    Id = _feedback.Id,
                    Title = TitleTextBox.Text.Trim(),
                    Content = ContentTextBox.Text.Trim()
                };

                var updatedFeedback = await _feedbackService.UpdateAsync(updateDto);

                if (updatedFeedback != null)
                {
                    MessageBox.Show("Cập nhật đánh giá thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật đánh giá thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

