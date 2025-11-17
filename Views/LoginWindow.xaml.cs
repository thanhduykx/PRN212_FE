using AssignmentPRN212.Services;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly UserService _userService;
        private readonly Window? _parentWindow;

        public LoginWindow(Window? parentWindow = null)
        {
            InitializeComponent();
            string apiBaseUrl = "https://localhost:7200/api/";
            _apiService = new ApiService(apiBaseUrl); // ✅ truyền BaseUrl
            _userService = new UserService(_apiService);
            _parentWindow = parentWindow;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                var result = await _userService.LoginAsync(email, password);

                if (!string.IsNullOrEmpty(result.Token))
                {
                    _apiService.SetToken(result.Token); // Set token cho ApiService

                    // Đóng HomeWindow cũ (nếu có)
                    if (_parentWindow != null)
                    {
                        _parentWindow.Close();
                    }

                    // Mở HomeWindow mới với thông tin đã đăng nhập
                    var homeWindow = new HomeWindow(_apiService, result.Role, result.UserId);
                    homeWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login failed! Kiểm tra email/password.");
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                MessageBox.Show("Invalid Token or Account");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.Show();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
