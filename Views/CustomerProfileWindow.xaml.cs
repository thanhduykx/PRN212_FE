using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class CustomerProfileWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly UserService _userService;
        private readonly int _userId;

        public CustomerProfileWindow(ApiService apiService, int userId)
        {
            InitializeComponent();
            _apiService = apiService;
            _userService = new UserService(_apiService);
            _userId = userId;
            LoadUserInfo();
        }

        private async void LoadUserInfo()
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(_userId);
                if (user != null)
                {
                    EmailTextBox.Text = user.Email;
                    FullNameTextBox.Text = user.FullName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateNameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                {
                    MessageBox.Show("Họ và tên không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var updateDto = new UpdateCustomerNameDTO
                {
                    UserId = _userId,
                    FullName = FullNameTextBox.Text.Trim()
                };

                var updatedUser = await _userService.UpdateCustomerNameAsync(updateDto);

                if (updatedUser != null)
                {
                    MessageBox.Show("Cập nhật tên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật tên thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdatePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OldPasswordBox.Password))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu cũ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewPasswordBox.Password))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu mới.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("Mật khẩu mới và xác nhận không khớp.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NewPasswordBox.Password.Length < 6)
                {
                    MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var updateDto = new UpdateCustomerPasswordDTO
                {
                    UserId = _userId,
                    OldPassword = OldPasswordBox.Password,
                    NewPassword = NewPasswordBox.Password
                };

                var updatedUser = await _userService.UpdateCustomerPasswordAsync(updateDto);

                if (updatedUser != null)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    OldPasswordBox.Clear();
                    NewPasswordBox.Clear();
                    ConfirmPasswordBox.Clear();
                }
                else
                {
                    MessageBox.Show("Đổi mật khẩu thất bại. Kiểm tra lại mật khẩu cũ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

