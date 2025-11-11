using AssignmentPRN212.Services;
using System;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class ConfirmEmailWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly UserService _userService;

        public ConfirmEmailWindow(ApiService apiService = null)
        {
            InitializeComponent();
            if (apiService == null)
            {
                string apiBaseUrl = "https://localhost:7200/api/";
                _apiService = new ApiService(apiBaseUrl);
            }
            else
            {
                _apiService = apiService;
            }
            _userService = new UserService(_apiService);
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string token = OTPTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                MessageBox.Show("Vui lòng nhập mã OTP.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var success = await _userService.ConfirmEmailAsync(token);

                if (success)
                {
                    MessageBox.Show(
                        "Email đã được xác nhận thành công!\nBạn có thể đăng nhập ngay.",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Mã OTP không hợp lệ hoặc đã hết hạn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

