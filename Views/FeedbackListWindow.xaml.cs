using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using AssignmentPRN212.Services;
using AssignmentPRN212.DTOs;

namespace AssignmentPRN212.Views
{
    public partial class FeedbackListWindow : Window
    {
        private readonly ApiService _apiService;
        public ObservableCollection<FeedbackDTO> Feedbacks { get; set; } = new ObservableCollection<FeedbackDTO>();
        private FeedbackDTO _selectedFeedback;

        public FeedbackListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            FeedbackDataGrid.ItemsSource = Feedbacks;
            _ = LoadFeedbacksAsync();
        }

        private async Task LoadFeedbacksAsync()
        {
            try
            {
                Feedbacks.Clear();
                var feedbacks = await _apiService.GetAllFeedbacksAsync();
                foreach (var feedback in feedbacks)
                {
                    Feedbacks.Add(feedback);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading feedbacks: {ex.Message}");
            }
        }

        private void FeedbackDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FeedbackDataGrid.SelectedItem is FeedbackDTO feedback)
            {
                _selectedFeedback = feedback;
                TitleTextBox.Text = feedback.Title;
                ContentTextBox.Text = feedback.Content;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var feedback = new FeedbackDTO
            {
                Title = TitleTextBox.Text,
                Content = ContentTextBox.Text,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var added = await _apiService.AddFeedbackAsync(feedback);
                if (added != null)
                {
                    MessageBox.Show("Feedback added successfully!");
                    await LoadFeedbacksAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding feedback: {ex.Message}");
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFeedback == null)
            {
                MessageBox.Show("Select a feedback first!");
                return;
            }

            _selectedFeedback.Title = TitleTextBox.Text;
            _selectedFeedback.Content = ContentTextBox.Text;

            try
            {
                var updated = await _apiService.UpdateFeedbackAsync(_selectedFeedback);
                if (updated != null)
                {
                    MessageBox.Show("Feedback updated successfully!");
                    await LoadFeedbacksAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating feedback: {ex.Message}");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFeedback == null)
            {
                MessageBox.Show("Select a feedback first!");
                return;
            }

            try
            {
                var success = await _apiService.DeleteFeedbackAsync(_selectedFeedback.Id);
                if (success)
                {
                    MessageBox.Show("Feedback deleted successfully!");
                    await LoadFeedbacksAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting feedback: {ex.Message}");
            }
        }
    }
}