using AssignmentPRN212.Services;
using System.Windows;

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
                    _apiService.SetToken(result.Token);
                    MessageBox.Show($"Login Succesfully! Role: {result.Role}");
                    var carListWindow = new CarListWindow(_apiService); // truyền ApiService đã có token
                    carListWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Login Fail!");
                }
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                MessageBox.Show($"Invalid Token or Account");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

    }

  
}
