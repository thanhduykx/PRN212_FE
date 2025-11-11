using AssignmentPRN212.Services;
using System.Windows;
using System.Windows.Input;

namespace AssignmentPRN212.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;

        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            string apiBaseUrl = "https://localhost:7200/api/";
            _apiService = new ApiService(apiBaseUrl); // ✅ truyền BaseUrl
            _userService = new UserService(_apiService);
            // Bắt sự kiện nhấn Enter trên email và password
            EmailTextBox.KeyDown += LoginInput_KeyDown;
            PasswordBox.KeyDown += LoginInput_KeyDown;
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

                    if (result.Role == "Admin")
                    {
                        var mainWindow  = new MainWindow(_apiService,"Admin"); // AdminWindow có 2 button: Xem User, Xem Car
                        mainWindow.Show();
                    }
                    else
                    {
                        var carListWindow = new CarListWindow(_apiService); // Staff hoặc Customer
                        carListWindow.Show();
                    }

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
        // Nhấn Enter để gửi chat
        // Nhấn Enter sẽ login
        private async void LoginInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Login_Click(this, new RoutedEventArgs());
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
