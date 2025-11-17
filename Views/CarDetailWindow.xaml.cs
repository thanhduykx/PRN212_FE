using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AssignmentPRN212.Views
{
    public partial class CarDetailWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly CarService _carService;
        private readonly FeedbackService _feedbackService;
        private readonly CarDTO _car;
        private readonly int? _userId;
        private readonly string? _userRole;

        public CarDetailWindow(ApiService apiService, CarDTO car, int? userId = null, string? userRole = null)
        {
            InitializeComponent();
            _apiService = apiService;
            _carService = new CarService(_apiService);
            _feedbackService = new FeedbackService(_apiService);
            _car = car;
            _userId = userId;
            _userRole = userRole;

            LoadCarDetails();
            _ = LoadFeedbacks(); // Fire and forget async call
        }

        private void LoadCarDetails()
        {
            CarNameTextBlock.Text = _car.Name;
            CarModelTextBlock.Text = _car.Model;
            CarSeatsTextBlock.Text = _car.Seats.ToString();
            CarBatteryTextBlock.Text = $"{_car.BatteryDuration} km ({_car.BatteryType})";
            CarTrunkTextBlock.Text = $"{_car.TrunkCapacity} L";
            CarPricePerDayTextBlock.Text = $"{_car.RentPricePerDay:N0} VNĐ";
            CarPricePerHourTextBlock.Text = $"{_car.RentPricePerHour:N0} VNĐ";
            CarSizeTypeTextBlock.Text = _car.SizeType;

            // Load main image
            if (!string.IsNullOrWhiteSpace(_car.ImageUrl))
            {
                try
                {
                    CarImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_car.ImageUrl, UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading main image: {ex.Message}");
                }
            }

            // Load second image
            if (!string.IsNullOrWhiteSpace(_car.ImageUrl2))
            {
                try
                {
                    CarImage2.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_car.ImageUrl2, UriKind.Absolute));
                    CarImage2.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image 2: {ex.Message}");
                    CarImage2.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                CarImage2.Visibility = Visibility.Collapsed;
            }

            // Load third image
            if (!string.IsNullOrWhiteSpace(_car.ImageUrl3))
            {
                try
                {
                    CarImage3.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_car.ImageUrl3, UriKind.Absolute));
                    CarImage3.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image 3: {ex.Message}");
                    CarImage3.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                CarImage3.Visibility = Visibility.Collapsed;
            }
        }

        private async Task LoadFeedbacks()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading feedbacks for car: {_car.Name} (Id: {_car.Id})");
                var feedbacks = await _feedbackService.GetByCarNameAsync(_car.Name);
                System.Diagnostics.Debug.WriteLine($"Loaded {feedbacks.Count} feedbacks");
                
                // Clear và set lại ItemsSource để đảm bảo UI được update
                FeedbacksItemsControl.ItemsSource = null;
                FeedbacksItemsControl.ItemsSource = feedbacks;
                
                FeedbackCountTextBlock.Text = $"({feedbacks.Count} đánh giá)";

                // Hiển thị nút Sửa/Xóa chỉ cho Staff và Admin
                if (_userRole == "Admin" || _userRole == "Staff")
                {
                    // Tìm tất cả StackPanel chứa nút Sửa/Xóa và hiển thị
                    UpdateFeedbackActionButtonsVisibility();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading feedbacks: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Lỗi tải đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFeedbackActionButtonsVisibility()
        {
            // Tìm tất cả Border trong FeedbacksItemsControl và hiển thị nút Sửa/Xóa
            if (FeedbacksItemsControl.ItemsSource != null)
            {
                // Cần update sau khi ItemsControl render xong
                FeedbacksItemsControl.UpdateLayout();
                var items = FeedbacksItemsControl.Items;
                foreach (var item in items)
                {
                    var container = FeedbacksItemsControl.ItemContainerGenerator.ContainerFromItem(item);
                    if (container != null)
                    {
                        var border = FindVisualChild<Border>(container);
                        if (border != null)
                        {
                            var actionPanel = FindVisualChild<StackPanel>(border, "ActionButtonsPanel");
                            if (actionPanel != null)
                            {
                                actionPanel.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent, string name = null) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T t && (name == null || (child is FrameworkElement fe && fe.Name == name)))
                    return t;
                var childOfChild = FindVisualChild<T>(child, name);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Image image)
            {
                CarImage.Source = image.Source;
            }
        }

        private async void AddFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_userRole))
            {
                MessageBox.Show("Vui lòng đăng nhập để đánh giá.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Mở window tạo feedback (truyền userRole để Staff/Admin không cần đơn hàng)
            var feedbackWindow = new CreateFeedbackWindow(_apiService, _car.Id, _userId ?? 0, _userRole);
            feedbackWindow.ShowDialog();
            LoadFeedbacks();
        }

        private void EditFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is FeedbackDTO feedback)
            {
                var editWindow = new EditFeedbackWindow(_apiService, feedback);
                editWindow.ShowDialog();
                LoadFeedbacks();
            }
        }

        private async void DeleteFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
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
                            LoadFeedbacks();
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

        private void RentCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_userRole))
            {
                MessageBox.Show("Vui lòng đăng nhập để thuê xe.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var bookingWindow = new CarBookingWindow(_apiService, _car, _userId ?? 0);
            bookingWindow.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

