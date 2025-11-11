using AssignmentPRN212.Services;
using System;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class ResetPasswordWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly UserService _userService;
        private readonly string _email;

        public ResetPasswordWindow(ApiService apiService, string email)
        {
            InitializeComponent();
            _apiService = apiService;
            _userService = new UserService(_apiService);
            _email = email;
            EmailTextBox.Text = email;
        }

        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
            string otp = OTPTextBox.Text.Trim();
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Validation
            if (string.IsNullOrWhiteSpace(otp))
            {
                MessageBox.Show("Vui lòng nhập mã OTP.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var success = await _userService.ResetPasswordAsync(_email, otp, newPassword);

                if (success)
                {
                    MessageBox.Show(
                        "Đặt lại mật khẩu thành công!\nBạn có thể đăng nhập ngay với mật khẩu mới.",
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

