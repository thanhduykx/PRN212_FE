using AssignmentPRN212.Services;
using System;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly UserService _userService;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
            string apiBaseUrl = "https://localhost:7200/api/";
            _apiService = new ApiService(apiBaseUrl);
            _userService = new UserService(_apiService);
        }

        private async void SendOTP_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập email.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var success = await _userService.ForgotPasswordAsync(email);

                if (success)
                {
                    MessageBox.Show(
                        $"Mã OTP đã được gửi đến email: {email}\nVui lòng kiểm tra hộp thư của bạn.",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Mở cửa sổ reset password
                    var resetWindow = new ResetPasswordWindow(_apiService, email);
                    resetWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể gửi mã OTP. Vui lòng kiểm tra lại email.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}

