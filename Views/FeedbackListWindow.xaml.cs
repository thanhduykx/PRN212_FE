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
    public partial class FeedbackListWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly FeedbackService _feedbackService;
        private readonly string? _userRole;

        public ObservableCollection<FeedbackDTO> Feedbacks { get; set; } = new ObservableCollection<FeedbackDTO>();
        private System.Collections.Generic.List<FeedbackDTO> _allFeedbacks = new System.Collections.Generic.List<FeedbackDTO>();
        private FeedbackDTO? _selectedFeedback;

        public FeedbackListWindow(ApiService apiService, string? userRole = null)
        {
            InitializeComponent();
            _apiService = apiService;
            _feedbackService = new FeedbackService(_apiService);
            _userRole = userRole;

            FeedbackDataGrid.ItemsSource = Feedbacks;

            // Chỉ Admin và Staff mới có quyền xóa/sửa
            if (_userRole != "Admin" && _userRole != "Staff")
            {
                // Ẩn các cột thao tác nếu không phải Admin/Staff
                // Hoặc có thể disable window này
            }

            this.Loaded += FeedbackListWindow_Loaded;
        }

        private async void FeedbackListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFeedbacks();
        }

        private async Task LoadFeedbacks()
        {
            try
            {
                _allFeedbacks = await _feedbackService.GetAllAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";

            var filtered = _allFeedbacks.Where(feedback =>
                string.IsNullOrWhiteSpace(searchText) ||
                feedback.Title.ToLower().Contains(searchText) ||
                feedback.Content.ToLower().Contains(searchText) ||
                feedback.UserId.ToString().Contains(searchText) ||
                feedback.RentalOrderId.ToString().Contains(searchText) ||
                feedback.Id.ToString().Contains(searchText)
            ).ToList();

            Feedbacks.Clear();
            foreach (var feedback in filtered)
                Feedbacks.Add(feedback);

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = Feedbacks.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void FeedbackDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FeedbackDataGrid.SelectedItem is FeedbackDTO selectedFeedback)
            {
                _selectedFeedback = selectedFeedback;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadFeedbacks();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Staff")
            {
                MessageBox.Show("Chỉ Admin và Staff mới có quyền sửa đánh giá.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (sender is Button button && button.Tag is FeedbackDTO feedback)
            {
                var editWindow = new EditFeedbackWindow(_apiService, feedback);
                editWindow.ShowDialog();
                await LoadFeedbacks();
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Staff")
            {
                MessageBox.Show("Chỉ Admin và Staff mới có quyền xóa đánh giá.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (sender is Button button && button.Tag is FeedbackDTO feedback)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa đánh giá này?", "Xác nhận",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var success = await _feedbackService.DeleteAsync(feedback.Id);
                        if (success)
                        {
                            MessageBox.Show("Xóa đánh giá thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadFeedbacks();
                        }
                        else
                        {
                            MessageBox.Show("Xóa đánh giá thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}

