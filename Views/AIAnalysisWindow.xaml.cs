using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class AIAnalysisWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly AIService _aiService;

        public ObservableCollection<CarUsageAnalysisResponse> CarUsageResults { get; set; } = new ObservableCollection<CarUsageAnalysisResponse>();

        public AIAnalysisWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _aiService = new AIService(apiService);

            CarUsageDataGrid.ItemsSource = CarUsageResults;
        }

        private async void AnalyzeDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnalyzeDataButton.IsEnabled = false;
                AnalyzeDataButton.Content = "Đang phân tích...";
                AnalysisResultTextBlock.Text = "Đang phân tích dữ liệu, vui lòng đợi...";
                AnalysisDetailsBorder.Visibility = Visibility.Collapsed;

                var result = await _aiService.AnalyzeAsync();

                if (result != null)
                {
                    // Hiển thị kết quả phân tích
                    string analysisText = result.Summary ?? result.Analysis ?? "Phân tích hoàn tất.";
                    AnalysisResultTextBlock.Text = analysisText;
                    
                    // Hiển thị chi tiết nếu có
                    bool hasDetails = result.TotalOrders.HasValue || 
                                     result.TotalRevenue.HasValue ||
                                     result.TotalCars.HasValue ||
                                     result.TotalUsers.HasValue ||
                                     !string.IsNullOrEmpty(result.Insights);
                    
                    if (hasDetails)
                    {
                        AnalysisDetailsBorder.Visibility = Visibility.Visible;
                        
                        if (result.TotalOrders.HasValue)
                            TotalOrdersTextBlock.Text = $"Tổng số đơn hàng: {result.TotalOrders.Value:N0}";
                        else
                            TotalOrdersTextBlock.Text = "";
                        
                        if (result.TotalRevenue.HasValue)
                            TotalRevenueTextBlock.Text = $"Tổng doanh thu: {result.TotalRevenue.Value:N0} VNĐ";
                        else
                            TotalRevenueTextBlock.Text = "";
                        
                        if (result.TotalCars.HasValue)
                            TotalCarsTextBlock.Text = $"Tổng số xe: {result.TotalCars.Value:N0}";
                        else
                            TotalCarsTextBlock.Text = "";
                        
                        if (result.TotalUsers.HasValue)
                            TotalUsersTextBlock.Text = $"Tổng số người dùng: {result.TotalUsers.Value:N0}";
                        else
                            TotalUsersTextBlock.Text = "";
                        
                        if (!string.IsNullOrEmpty(result.Insights))
                            AnalysisInsightsTextBlock.Text = $"💡 Gợi ý: {result.Insights}";
                        else
                            AnalysisInsightsTextBlock.Text = "";
                    }
                }
                else
                {
                    AnalysisResultTextBlock.Text = "Không thể phân tích dữ liệu. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                AnalysisResultTextBlock.Text = $"Lỗi: {ex.Message}";
                MessageBox.Show($"Lỗi phân tích dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                AnalyzeDataButton.IsEnabled = true;
                AnalyzeDataButton.Content = "📊 Phân tích dữ liệu";
            }
        }

        private async void AnalyzeCarUsageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnalyzeCarUsageButton.IsEnabled = false;
                AnalyzeCarUsageButton.Content = "Đang phân tích...";
                CarUsageEmptyTextBlock.Visibility = Visibility.Visible;

                var results = await _aiService.GetCarUsageAnalysisAsync();

                CarUsageResults.Clear();

                if (results != null && results.Any())
                {
                    CarUsageEmptyTextBlock.Visibility = Visibility.Collapsed;
                    foreach (var item in results)
                    {
                        // Map TotalRentals to RentalCount if needed
                        if (item.RentalCount == 0 && item.TotalRentals > 0)
                        {
                            item.RentalCount = item.TotalRentals;
                        }
                        CarUsageResults.Add(item);
                    }
                }
                else
                {
                    CarUsageEmptyTextBlock.Text = "Không có dữ liệu phân tích sử dụng xe.";
                    CarUsageEmptyTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                CarUsageEmptyTextBlock.Text = $"Lỗi: {ex.Message}";
                CarUsageEmptyTextBlock.Visibility = Visibility.Visible;
                MessageBox.Show($"Lỗi phân tích sử dụng xe: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                AnalyzeCarUsageButton.IsEnabled = true;
                AnalyzeCarUsageButton.Content = "🚗 Phân tích sử dụng xe";
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all results
            AnalysisResultTextBlock.Text = "Nhấn 'Phân tích dữ liệu' để xem kết quả...";
            AnalysisDetailsBorder.Visibility = Visibility.Collapsed;
            CarUsageResults.Clear();
            CarUsageEmptyTextBlock.Text = "Nhấn 'Phân tích sử dụng xe' để xem kết quả...";
            CarUsageEmptyTextBlock.Visibility = Visibility.Visible;
        }
    }
}

