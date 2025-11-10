using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using AssignmentPRN212.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class CarListWindow : Window
    {
        private readonly CarService _carService;

        public CarListWindow(ApiService apiService)
        {
            InitializeComponent();
            _carService = new CarService(apiService);

            Loaded += CarListWindow_Loaded;
        }

        private async void CarListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCarsAsync();
        }

        private async Task LoadCarsAsync()
        {
            try
            {
                List<CarDTO> cars = await _carService.GetAllCarsAsync();
                CarDataGrid.ItemsSource = cars;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi load danh sách xe: {ex.Message}");
            }
        }
    }
}
