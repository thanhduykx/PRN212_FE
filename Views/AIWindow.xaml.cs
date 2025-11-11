using AssignmentPRN212.Services;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class AIWindow : Window
    {
        private readonly ApiService _apiService;

        public AIWindow(ApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var aiResult = await _apiService.GetAsync<AIResponse>("AI/analyze");
                ResultTextBox.Text = aiResult?.Response ?? "Không có dữ liệu AI";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gọi AI: {ex.Message}");
            }
        }

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hiển thị loading
                LoadingTextBlock.Visibility = Visibility.Visible;
                ResultTextBox.Clear();

                // Disable button nếu muốn
                ((Button)sender).IsEnabled = false;

                // Gọi API
                var aiResult = await _apiService.GetAsync<AIResponse>("AI/analyze");

                // Hiển thị kết quả
                ResultTextBox.Text = aiResult?.Response ?? "Không có kết quả";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
            finally
            {
                // Ẩn loading, enable button
                LoadingTextBlock.Visibility = Visibility.Collapsed;
                ((Button)sender).IsEnabled = true;
            }
        }

    }

    public class AIResponse
    {
        public string Response { get; set; }
    }
}
