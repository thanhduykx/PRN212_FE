using AssignmentPRN212.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AssignmentPRN212.Models;
namespace AssignmentPRN212.Views
{
    public partial class AIWindow : Window
    {
        private readonly ApiService _apiService;

        public AIWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

            // Bắt sự kiện nhấn Enter
            ChatInputTextBox.KeyDown += ChatInputTextBox_KeyDown;
        }


        // Nhấn Enter để gửi chat
        private async void ChatInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Ngăn xuống dòng
                await SendChatAsync(ChatInputTextBox.Text);
                ChatInputTextBox.Clear();
            }
        }
        // Gửi câu hỏi chat
        private async void SendChatButton_Click(object sender, RoutedEventArgs e)
        {
            var question = ChatInputTextBox.Text.Trim(); if (string.IsNullOrEmpty(question)) return; ChatInputTextBox.Clear(); AppendResult($"Bạn: {question}"); try
            {
                ShowLoading(true); // Gọi API chat
                var chatResult = await _apiService.PostAsync<ChatRequest, ChatResponse>("AI/chat", new ChatRequest { Message = question }); AppendResult($"AI: {chatResult.Reply}");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi chat: {ex.Message}"); }
            finally { ShowLoading(false); }
        }
        // Gửi chat
        private async Task SendChatAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            AppendResult($"Bạn: {message}");

            try
            {
                ShowLoading(true);

                var chatResult = await _apiService.PostAsync<ChatRequest, ChatResponse>("AI/chat",
                    new ChatRequest { Message = message });

                AppendResult($"AI: {chatResult.Reply}");
            }
            catch (Exception ex)
            {
                AppendResult($"Lỗi chat: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        // Phân tích dữ liệu
        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            await AnalyzeDataAsync();
        }

        private async Task AnalyzeDataAsync()
        {
            try
            {
                ShowLoading(true);
                ResultTextBox.Clear();

                var result = await _apiService.GetAsync<AIResponse>("AI/analyze");

                AppendResult("AI Phân tích dữ liệu:\n" + result.Response);
            }
            catch (Exception ex)
            {
                AppendResult($"Lỗi phân tích: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        // Hiển thị/ẩn loading
        private void ShowLoading(bool show)
        {
            LoadingProgressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            LoadingTextBlock.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        // Append text vào ResultTextBox
        private void AppendResult(string text)
        {
            if (!string.IsNullOrEmpty(ResultTextBox.Text))
                ResultTextBox.AppendText("\n\n");

            ResultTextBox.AppendText(text);
            ResultTextBox.ScrollToEnd();
        }
    }


}
