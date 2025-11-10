using AssignmentPRN212.Services;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly string _role;

        public MainWindow(ApiService apiService, string role)
        {
            InitializeComponent();
            _apiService = apiService;
            _role = role; // ✅ Gán role

            // Nếu không phải Admin, ẩn nút xem user
            if (_role != "Admin")
            {
                ViewUsersButton.Visibility = Visibility.Collapsed;
            }
        }


        private void ViewCarsButton_Click(object sender, RoutedEventArgs e)
        {
            var carWindow = new CarListWindow(_apiService);
            carWindow.ShowDialog();
        }

        private void ViewUsersButton_Click(object sender, RoutedEventArgs e)
        {
            var userWindow = new UserListWindow(_apiService);
            userWindow.ShowDialog();
        }
    }
}
