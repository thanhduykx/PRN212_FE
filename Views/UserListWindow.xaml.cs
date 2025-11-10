using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class UserListWindow : Window
    {
        private readonly ApiService _apiService;

        // ObservableCollection bind DataGrid
        public ObservableCollection<UserDTO> Users { get; set; } = new ObservableCollection<UserDTO>();
        private UserDTO _selectedUser;

        public UserListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            UsersDataGrid.ItemsSource = Users;

            // Load dữ liệu
            LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<UserDTO>>>("User/GetAll");
                if (response?.Data?.Values != null)
                {
                    Users.Clear();
                    foreach (var user in response.Data.Values)
                        Users.Add(user);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Load lỗi: {ex.Message}");
            }
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is UserDTO selectedUser)
            {
                _selectedUser = selectedUser;

                EmailTextBox.Text = selectedUser.Email;
                FullNameTextBox.Text = selectedUser.FullName;
                RoleTextBox.Text = selectedUser.Role;
                IsActiveCheckBox.IsChecked = selectedUser.IsActive;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadUsers();
        }
    }
}
