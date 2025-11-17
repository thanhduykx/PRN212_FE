using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class UserListWindow : Window
    {
        private readonly UserService _userService;

        // ObservableCollection bind DataGrid
        public ObservableCollection<UserDTO> Users { get; set; } = new ObservableCollection<UserDTO>();
        private List<UserDTO> _allUsers = new List<UserDTO>(); // Lưu danh sách gốc để filter
        private UserDTO _selectedUser;

        public UserListWindow(ApiService apiService)
        {
            InitializeComponent();
            _userService = new UserService(apiService);

            UsersDataGrid.ItemsSource = Users;

            // Load dữ liệu
            LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                _allUsers = users.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";
            
            var filtered = _allUsers.Where(user =>
                string.IsNullOrWhiteSpace(searchText) ||
                user.Email.ToLower().Contains(searchText) ||
                user.FullName.ToLower().Contains(searchText) ||
                user.Role.ToLower().Contains(searchText)
            ).ToList();

            Users.Clear();
            foreach (var user in filtered)
                Users.Add(user);

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = Users.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void UsersDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Hiển thị số thứ tự (STT) bắt đầu từ 1
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
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

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn user cần cập nhật.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                {
                    MessageBox.Show("Email và Full Name không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(RoleTextBox.Text))
                {
                    MessageBox.Show("Role không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật giá trị cho _selectedUser
                _selectedUser.Email = EmailTextBox.Text.Trim();
                _selectedUser.FullName = FullNameTextBox.Text.Trim();
                _selectedUser.Role = RoleTextBox.Text.Trim();
                _selectedUser.IsActive = IsActiveCheckBox.IsChecked ?? false;

                // Tạo DTO cho service
                var updateDto = new UpdateUserDTO
                {
                    Id = _selectedUser.Id,
                    Email = _selectedUser.Email,
                    FullName = _selectedUser.FullName,
                    Role = _selectedUser.Role,
                    IsActive = _selectedUser.IsActive
                };

                var updatedUser = await _userService.UpdateUserAsync(updateDto);

                if (updatedUser != null)
                {
                    MessageBox.Show("Cập nhật user thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadUsers();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Cập nhật user thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn user cần xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa user: {_selectedUser.Email}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Sử dụng Id để xóa
                    var success = await _userService.DeleteUserAsync(_selectedUser.Id);

                    if (success)
                    {
                        MessageBox.Show("Xóa user thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadUsers();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Xóa user thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void ClearInputs()
        {
            EmailTextBox.Clear();
            FullNameTextBox.Clear();
            RoleTextBox.Clear();
            IsActiveCheckBox.IsChecked = false;
            _selectedUser = null;
            UsersDataGrid.SelectedItem = null;
        }
    }
}
