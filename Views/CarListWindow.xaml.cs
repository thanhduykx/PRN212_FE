using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AssignmentPRN212.Views
{
    public partial class CarListWindow : Window
    {
        private readonly CarService _carService;
        private readonly RentalLocationService _rentalLocationService;
        private readonly CarRentalLocationService _carRentalLocationService;
        private readonly ApiService _apiService;
        private CarDTO _selectedCar;
        private bool _isLoading = false; // Flag để tránh multiple calls
        private bool _isUpdatingSelection = false; // Flag để tránh trigger selection changed khi đang update

        public ObservableCollection<CarDTO> Cars { get; set; } = new ObservableCollection<CarDTO>();
        private List<CarDTO> _allCars = new List<CarDTO>(); // Lưu danh sách gốc để filter
        private List<RentalLocationDTO> _allLocations = new List<RentalLocationDTO>(); // Danh sách địa điểm

        public CarListWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _carService = new CarService(apiService);
            _rentalLocationService = new RentalLocationService(apiService);
            _carRentalLocationService = new CarRentalLocationService(apiService);
            CarDataGrid.ItemsSource = Cars;
            Loaded += CarListWindow_Loaded;
        }

        private async void CarListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCars();
            await LoadRentalLocations();
        }

        private async Task LoadRentalLocations()
        {
            try
            {
                _allLocations = await _rentalLocationService.GetAllAsync();
                RentalLocationComboBox.Items.Clear();
                foreach (var location in _allLocations.Where(l => l.IsActive))
                {
                    RentalLocationComboBox.Items.Add(location);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách địa điểm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task LoadCars()
        {
            try
            {
                var response = await _carService.GetAllCarsAsync();
                _allCars = response.ToList();
                
                // Load địa điểm cho từng xe
                await LoadLocationNamesForCars(_allCars);
                
                // Đảm bảo UI được update trên UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ApplyFilter();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadLocationNamesForCars(List<CarDTO> cars)
        {
            try
            {
                // Lấy tất cả CarId
                var carIds = cars.Select(c => c.Id).ToList();
                
                // Load địa điểm cho từng xe
                foreach (var car in cars)
                {
                    var carLocations = await _carRentalLocationService.GetByCarIdAsync(car.Id);
                    if (carLocations.Any())
                    {
                        // Lấy địa điểm đầu tiên
                        var firstLocationId = carLocations.First().LocationId;
                        var location = _allLocations.FirstOrDefault(l => l.Id == firstLocationId);
                        if (location != null)
                        {
                            car.LocationName = location.Name;
                        }
                        else
                        {
                            car.LocationName = "N/A";
                        }
                    }
                    else
                    {
                        car.LocationName = "Chưa có địa điểm";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadLocationNamesForCars error: {ex.Message}");
                // Nếu lỗi, set LocationName mặc định
                foreach (var car in cars)
                {
                    if (string.IsNullOrEmpty(car.LocationName))
                    {
                        car.LocationName = "N/A";
                    }
                }
            }
        }

        private void ApplyFilter()
        {
            string searchText = SearchTextBox?.Text?.ToLower() ?? "";
            
            var filtered = _allCars.Where(car =>
                string.IsNullOrWhiteSpace(searchText) ||
                car.Name.ToLower().Contains(searchText) ||
                car.Model.ToLower().Contains(searchText) ||
                car.SizeType.ToLower().Contains(searchText) ||
                car.BatteryType.ToLower().Contains(searchText) ||
                car.Seats.ToString().Contains(searchText) ||
                car.RentPricePerDay.ToString().Contains(searchText) ||
                car.ImageUrl.ToLower().Contains(searchText)
            ).ToList();

            // Clear và cập nhật danh sách
            Cars.Clear();
            foreach (var car in filtered)
                Cars.Add(car);

            // Refresh DataGrid
            CarDataGrid.Items.Refresh();

            if (TotalCountTextBlock != null)
                TotalCountTextBlock.Text = Cars.Count.ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CarDataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            // Hiển thị số thứ tự (STT) bắt đầu từ 1
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private async void CarDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tránh trigger khi đang update selection programmatically
            if (_isUpdatingSelection) return;
            
            if (CarDataGrid.SelectedItem is CarDTO selectedCar)
            {
                _selectedCar = selectedCar;

                NameTextBox.Text = selectedCar.Name;
                ModelTextBox.Text = selectedCar.Model;
                SeatsTextBox.Text = selectedCar.Seats.ToString();
                
                // Set ComboBox cho SizeType
                SizeTypeComboBox.Text = selectedCar.SizeType;
                
                TrunkTextBox.Text = selectedCar.TrunkCapacity.ToString();
                
                // Set ComboBox cho BatteryType
                BatteryTypeComboBox.Text = selectedCar.BatteryType;
                
                BatteryDurationTextBox.Text = selectedCar.BatteryDuration.ToString();
                RentTextBox.Text = selectedCar.RentPricePerDay.ToString();
                
                // Set ComboBox cho Status
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Tag?.ToString() == selectedCar.Status.ToString())
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }
                
                // Set CheckBox cho IsActive
                IsActiveCheckBox.IsChecked = selectedCar.IsActive;

                RentHourTextBox.Text = selectedCar.RentPricePerHour.ToString();
                RentDayDriverTextBox.Text = selectedCar.RentPricePerDayWithDriver.ToString();
                RentHourDriverTextBox.Text = selectedCar.RentPricePerHourWithDriver.ToString();
                DepositAmountTextBox.Text = selectedCar.DepositAmount > 0 ? selectedCar.DepositAmount.ToString("N0") : "0";
                ImageUrlTextBox.Text = selectedCar.ImageUrl;
                ImageUrl2TextBox.Text = selectedCar.ImageUrl2;
                ImageUrl3TextBox.Text = selectedCar.ImageUrl3;

                // Load và chọn các địa điểm của xe này
                await LoadCarRentalLocations(selectedCar.Id);
            }
        }

        private async Task LoadCarRentalLocations(int carId)
        {
            try
            {
                // Set flag để tránh trigger SelectionChanged event
                _isUpdatingSelection = true;
                
                RentalLocationComboBox.SelectedItem = null;
                var carLocations = await _carRentalLocationService.GetByCarIdAsync(carId);
                
                // Chỉ hiển thị địa điểm đầu tiên (nếu có)
                if (carLocations.Any())
                {
                    var firstLocation = _allLocations.FirstOrDefault(l => l.Id == carLocations.First().LocationId);
                    if (firstLocation != null)
                    {
                        RentalLocationComboBox.SelectedItem = firstLocation;
                        System.Diagnostics.Debug.WriteLine($"Loaded location for car {carId}: {firstLocation.Name} (Id: {firstLocation.Id})");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCarRentalLocations error: {ex.Message}");
            }
            finally
            {
                // Reset flag sau khi update xong
                _isUpdatingSelection = false;
            }
        }


        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Tránh double-click
            if (_isLoading) return;

            _isLoading = true;
            AddButton.IsEnabled = false;
            AddButton.Content = "Đang thêm...";

            try
            {
                // Validate inputs
                if (!ValidateInputs(out CarDTO newCar))
                {
                    _isLoading = false;
                    AddButton.IsEnabled = true;
                    AddButton.Content = "➕ Thêm xe";
                    return;
                }

                // Lấy locationId nếu có địa điểm được chọn
                int? locationId = null;
                
                // Debug: Kiểm tra ComboBox state
                System.Diagnostics.Debug.WriteLine($"=== DEBUG RentalLocationComboBox ===");
                System.Diagnostics.Debug.WriteLine($"SelectedItem: {RentalLocationComboBox.SelectedItem}");
                System.Diagnostics.Debug.WriteLine($"SelectedIndex: {RentalLocationComboBox.SelectedIndex}");
                System.Diagnostics.Debug.WriteLine($"Items.Count: {RentalLocationComboBox.Items.Count}");
                
                // Cách 1: Lấy từ SelectedItem
                if (RentalLocationComboBox.SelectedItem != null)
                {
                    var selectedLocation = RentalLocationComboBox.SelectedItem as RentalLocationDTO;
                    if (selectedLocation != null && selectedLocation.Id > 0)
                    {
                        locationId = selectedLocation.Id;
                        System.Diagnostics.Debug.WriteLine($"✓ Location from SelectedItem: {selectedLocation.Name} (Id: {selectedLocation.Id})");
                    }
                }
                
                // Cách 2: Nếu SelectedItem không có, thử SelectedValue
                if (!locationId.HasValue && RentalLocationComboBox.SelectedValue != null)
                {
                    if (RentalLocationComboBox.SelectedValue is RentalLocationDTO location && location.Id > 0)
                    {
                        locationId = location.Id;
                        System.Diagnostics.Debug.WriteLine($"✓ Location from SelectedValue: {location.Name} (Id: {location.Id})");
                    }
                }
                
                // Cách 3: Nếu vẫn không có, thử SelectedIndex
                if (!locationId.HasValue && RentalLocationComboBox.SelectedIndex >= 0 && RentalLocationComboBox.SelectedIndex < RentalLocationComboBox.Items.Count)
                {
                    var item = RentalLocationComboBox.Items[RentalLocationComboBox.SelectedIndex] as RentalLocationDTO;
                    if (item != null && item.Id > 0)
                    {
                        locationId = item.Id;
                        System.Diagnostics.Debug.WriteLine($"✓ Location from SelectedIndex: {item.Name} (Id: {item.Id})");
                    }
                }

                // Debug: Kiểm tra locationId cuối cùng
                if (locationId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine($"✓✓✓ Final locationId = {locationId.Value}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("✗✗✗ WARNING: Final locationId is null - no location selected");
                    MessageBox.Show("⚠️ Vui lòng chọn địa điểm cho thuê trước khi thêm xe.", 
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _isLoading = false;
                    AddButton.IsEnabled = true;
                    AddButton.Content = "➕ Thêm xe";
                    return;
                }

                // Tạo xe
                System.Diagnostics.Debug.WriteLine($"=== Calling AddCarAsync ===");
                System.Diagnostics.Debug.WriteLine($"Car: {newCar.Name}, LocationId: {locationId}");
                
                var addedCar = await _carService.AddCarAsync(newCar, locationId);
                
                System.Diagnostics.Debug.WriteLine($"=== AddCarAsync completed ===");
                System.Diagnostics.Debug.WriteLine($"AddedCar: {(addedCar != null ? $"Id={addedCar.Id}, Name={addedCar.Name}" : "null")}");

                if (addedCar != null && addedCar.Id > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"=== Car created successfully, now creating CarRentalLocation ===");
                    
                    // Tạo CarRentalLocation sau khi car đã được tạo thành công (giống như update)
                    if (locationId.HasValue && locationId.Value > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"LocationId is valid: {locationId.Value}");
                        
                        try
                        {
                            var selectedLocation = RentalLocationComboBox.SelectedItem as RentalLocationDTO;
                            System.Diagnostics.Debug.WriteLine($"SelectedLocation: {(selectedLocation != null ? $"Id={selectedLocation.Id}, Name={selectedLocation.Name}" : "null")}");
                            
                            if (selectedLocation != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"About to call CreateCarRentalLocation with CarId={addedCar.Id}, LocationId={selectedLocation.Id}");
                                await CreateCarRentalLocation(addedCar.Id, selectedLocation);
                                System.Diagnostics.Debug.WriteLine($"✓ CreateCarRentalLocation completed for CarId={addedCar.Id}, LocationId={selectedLocation.Id}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"✗ SelectedLocation is null, cannot create CarRentalLocation");
                            }
                        }
                        catch (Exception locationEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"✗✗✗ CreateCarRentalLocation exception: {locationEx.Message}");
                            System.Diagnostics.Debug.WriteLine($"StackTrace: {locationEx.StackTrace}");
                            // Không throw - chỉ log, vì car đã được tạo thành công
                            MessageBox.Show($"✅ Xe đã được tạo thành công!\n\n⚠️ Cảnh báo: Không thể tạo liên kết địa điểm.\n\nChi tiết: {locationEx.Message}", 
                                "Thành công với cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"✗ LocationId is not valid: {locationId}");
                    }

                    // Clear form
                    ClearInputs();

                    // Reload danh sách
                    await LoadCars();

                    // Báo thành công
                    MessageBox.Show($"✅ Thêm xe thành công!\n\nID: {addedCar.Id}\nTên xe: {addedCar.Name}\nModel: {addedCar.Model}", 
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Thêm xe thất bại. Vui lòng kiểm tra lại dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                AddButton.IsEnabled = true;
                AddButton.Content = "➕ Thêm xe";
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần cập nhật.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!ValidateInputs(out CarDTO updatedCar)) return;

                updatedCar.Id = _selectedCar.Id;
                updatedCar.CreatedAt = _selectedCar.CreatedAt; // Giữ nguyên CreatedAt
                updatedCar.IsDeleted = _selectedCar.IsDeleted; // Giữ nguyên IsDeleted

                // Hiển thị loading
                UpdateButton.IsEnabled = false;
                UpdateButton.Content = "Đang cập nhật...";

                var result = await _carService.UpdateCarAsync(updatedCar);

                if (result != null && result.Id == updatedCar.Id)
                {
                    // Cập nhật CarRentalLocation cho các địa điểm đã chọn
                    await UpdateCarRentalLocations(result.Id);
                    
                    MessageBox.Show($"Cập nhật xe thành công! ID: {result.Id}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadCars();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Cập nhật xe thất bại. Vui lòng kiểm tra lại dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                // Hiển thị error message chi tiết từ backend
                string errorMsg = httpEx.Message;
                if (errorMsg.Contains("400"))
                {
                    MessageBox.Show($"Lỗi 400 - Dữ liệu không hợp lệ:\n{errorMsg}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Lỗi từ server:\n{errorMsg}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message ?? "N/A"}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                UpdateButton.IsEnabled = true;
                UpdateButton.Content = "✏️ Cập nhật";
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần xóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa xe: {_selectedCar.Name} ({_selectedCar.Model})?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _carService.DeleteCarAsync(_selectedCar.Id);
                    if (success)
                    {
                        MessageBox.Show("Xóa xe thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadCars();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Xóa xe thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInputs(out CarDTO car)
        {
            car = new CarDTO();

            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(ModelTextBox.Text))
            {
                MessageBox.Show("Tên và Model không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(SeatsTextBox.Text, out int seats) || seats <= 0)
            {
                MessageBox.Show("Số ghế phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TrunkTextBox.Text, out int trunk) || trunk < 0)
            {
                MessageBox.Show("Dung tích cốp phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(BatteryDurationTextBox.Text, out int batteryDuration) || batteryDuration < 0)
            {
                MessageBox.Show("Thời lượng pin phải là số nguyên không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!double.TryParse(RentTextBox.Text, out double rentPrice) || rentPrice < 0)
            {
                MessageBox.Show("Giá thuê/ngày phải là số không âm.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            double.TryParse(RentHourTextBox.Text, out double rentHour);
            double.TryParse(RentDayDriverTextBox.Text, out double rentDayDriver);
            double.TryParse(RentHourDriverTextBox.Text, out double rentHourDriver);

            // Parse DepositAmount - mặc định là 0 nếu không nhập hoặc không hợp lệ
            double depositAmount = 0;
            if (!string.IsNullOrWhiteSpace(DepositAmountTextBox?.Text))
            {
                if (double.TryParse(DepositAmountTextBox.Text.Trim(), out double parsedValue) && parsedValue >= 0)
                {
                    depositAmount = parsedValue;
                }
            }

            // Lấy Status từ ComboBox
            int status = 0;
            if (StatusComboBox.SelectedItem != null)
            {
                if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatusItem && selectedStatusItem.Tag != null)
                {
                    int.TryParse(selectedStatusItem.Tag.ToString(), out status);
                }
            }
            else if (StatusComboBox.SelectedIndex >= 0 && StatusComboBox.SelectedIndex < StatusComboBox.Items.Count)
            {
                // Fallback: lấy từ SelectedIndex
                if (StatusComboBox.Items[StatusComboBox.SelectedIndex] is ComboBoxItem item && item.Tag != null)
                {
                    int.TryParse(item.Tag.ToString(), out status);
                }
            }

            // Validation các trường bắt buộc - lấy từ ComboBox (có thể là SelectedItem hoặc Text)
            string sizeType = "";
            if (SizeTypeComboBox.SelectedItem != null)
            {
                // Nếu có item được chọn, lấy từ SelectedItem
                if (SizeTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    sizeType = selectedItem.Content?.ToString()?.Trim() ?? "";
                }
            }
            // Nếu không có SelectedItem hoặc IsEditable, lấy từ Text
            if (string.IsNullOrWhiteSpace(sizeType))
            {
                sizeType = SizeTypeComboBox.Text?.Trim() ?? "";
            }
            if (string.IsNullOrWhiteSpace(sizeType))
            {
                MessageBox.Show("Loại xe không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string batteryType = "";
            if (BatteryTypeComboBox.SelectedItem != null)
            {
                // Nếu có item được chọn, lấy từ SelectedItem
                if (BatteryTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    batteryType = selectedItem.Content?.ToString()?.Trim() ?? "";
                }
            }
            // Nếu không có SelectedItem hoặc IsEditable, lấy từ Text
            if (string.IsNullOrWhiteSpace(batteryType))
            {
                batteryType = BatteryTypeComboBox.Text?.Trim() ?? "";
            }
            if (string.IsNullOrWhiteSpace(batteryType))
            {
                MessageBox.Show("Loại pin không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validation ảnh bắt buộc
            string imageUrl = ImageUrlTextBox?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                MessageBox.Show("URL ảnh xe chính là bắt buộc.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string imageUrl2 = ImageUrl2TextBox?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(imageUrl2))
            {
                MessageBox.Show("URL ảnh xe phụ 1 là bắt buộc.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string imageUrl3 = ImageUrl3TextBox?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(imageUrl3))
            {
                MessageBox.Show("URL ảnh xe phụ 2 là bắt buộc.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Khởi tạo CarDTO đầy đủ cho backend
            car.Name = NameTextBox.Text.Trim();
            car.Model = ModelTextBox.Text.Trim();
            car.Seats = seats;
            car.SizeType = sizeType;
            car.TrunkCapacity = trunk;
            car.BatteryType = batteryType;
            car.BatteryDuration = batteryDuration;
            car.RentPricePerDay = rentPrice;
            car.RentPricePerHour = rentHour;
            car.RentPricePerDayWithDriver = rentDayDriver;
            car.RentPricePerHourWithDriver = rentHourDriver;
            car.DepositAmount = depositAmount;
            car.ImageUrl = imageUrl;
            car.ImageUrl2 = imageUrl2;
            car.ImageUrl3 = imageUrl3;
            car.Status = status;
            car.IsActive = IsActiveCheckBox.IsChecked ?? true;
            car.IsDeleted = false;

            return true;
        }

        private void ClearInputs()
        {
            NameTextBox.Clear();
            ModelTextBox.Clear();
            SeatsTextBox.Clear();
            SizeTypeComboBox.Text = "";
            TrunkTextBox.Clear();
            BatteryTypeComboBox.Text = "";
            BatteryDurationTextBox.Clear();
            RentTextBox.Clear();
            RentHourTextBox.Clear();
            RentDayDriverTextBox.Clear();
            RentHourDriverTextBox.Clear();
            DepositAmountTextBox.Text = "0";
            ImageUrlTextBox.Clear();
            ImageUrl2TextBox.Clear();
            ImageUrl3TextBox.Clear();
            StatusComboBox.SelectedIndex = 0;
            IsActiveCheckBox.IsChecked = true;
            RentalLocationComboBox.SelectedItem = null;
            _selectedCar = null;
            CarDataGrid.SelectedItem = null;
        }

        private void UploadImage1Button_Click(object sender, RoutedEventArgs e)
        {
            UploadImage(ImageUrlTextBox);
        }

        private void UploadImage2Button_Click(object sender, RoutedEventArgs e)
        {
            UploadImage(ImageUrl2TextBox);
        }

        private void UploadImage3Button_Click(object sender, RoutedEventArgs e)
        {
            UploadImage(ImageUrl3TextBox);
        }

        private void UploadImage(TextBox targetTextBox)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif;*.webp)|*.jpg;*.jpeg;*.png;*.gif;*.webp|All files (*.*)|*.*",
                Title = "Chọn ảnh để upload"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                if (fileInfo.Length > 5 * 1024 * 1024) // 5MB
                {
                    MessageBox.Show("Kích thước ảnh không được vượt quá 5MB.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // TODO: Upload image to Cloudinary and get URL
                // Hiện tại chỉ hiển thị file path, cần implement upload API
                targetTextBox.Text = openFileDialog.FileName;
                MessageBox.Show("Chức năng upload ảnh lên Cloudinary đang phát triển.\nVui lòng nhập URL ảnh thủ công hoặc đợi tính năng này được hoàn thiện.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task CreateCarRentalLocation(int carId, RentalLocationDTO location)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== CreateCarRentalLocation START ===");
                System.Diagnostics.Debug.WriteLine($"CarId: {carId}, LocationId: {location.Id}, LocationName: {location.Name}");
                
                // Kiểm tra xem đã tồn tại chưa để tránh duplicate (dùng GetByCarIdAsync thay vì GetByCarAndLocationIdAsync)
                try
                {
                    var currentCarLocations = await _carRentalLocationService.GetByCarIdAsync(carId);
                    var existing = currentCarLocations.FirstOrDefault(cl => cl.LocationId == location.Id);
                    System.Diagnostics.Debug.WriteLine($"Existing CarRentalLocation: {(existing != null ? $"Id={existing.Id}" : "null")}");
                    
                    if (existing != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"CarRentalLocation already exists, skipping creation");
                        System.Diagnostics.Debug.WriteLine($"=== CreateCarRentalLocation END ===");
                        return;
                    }
                }
                catch (Exception checkEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: Could not check existing CarRentalLocation: {checkEx.Message}. Proceeding with creation...");
                }
                
                // Tạo mới
                var createRequest = new CreateCarRentalLocationDTO
                {
                    CarId = carId,
                    LocationId = location.Id,
                    Quantity = 1 // Mặc định số lượng là 1
                };
                
                System.Diagnostics.Debug.WriteLine($"Calling CreateAsync: CarId={createRequest.CarId}, LocationId={createRequest.LocationId}, Quantity={createRequest.Quantity}");
                
                var createdLocation = await _carRentalLocationService.CreateAsync(createRequest);
                
                if (createdLocation != null && createdLocation.Id > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"✓✓✓ CarRentalLocation created successfully: Id={createdLocation.Id}, CarId={createdLocation.CarId}, LocationId={createdLocation.LocationId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✗✗✗ CarRentalLocation CreateAsync returned null or invalid Id");
                }
                
                System.Diagnostics.Debug.WriteLine($"=== CreateCarRentalLocation END ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗✗✗ CreateCarRentalLocation error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                // Throw lại để caller có thể xử lý
                throw;
            }
        }

        private async Task UpdateCarRentalLocations(int carId)
        {
            try
            {
                // Lấy địa điểm hiện tại của xe (chỉ 1 địa điểm)
                var currentCarLocations = await _carRentalLocationService.GetByCarIdAsync(carId);
                var selectedLocation = RentalLocationComboBox.SelectedItem as RentalLocationDTO;

                // Nếu có địa điểm được chọn
                if (selectedLocation != null)
                {
                    // Kiểm tra xem địa điểm đã tồn tại chưa
                    var existing = currentCarLocations.FirstOrDefault(cl => cl.LocationId == selectedLocation.Id);
                    
                    if (existing == null)
                    {
                        // Xóa tất cả địa điểm cũ (vì chỉ cho phép 1 địa điểm)
                        foreach (var carLocation in currentCarLocations)
                        {
                            await _carRentalLocationService.DeleteAsync(carLocation.Id);
                        }
                        
                        // Tạo địa điểm mới
                        var createRequest = new CreateCarRentalLocationDTO
                        {
                            CarId = carId,
                            LocationId = selectedLocation.Id,
                            Quantity = 1
                        };
                        await _carRentalLocationService.CreateAsync(createRequest);
                    }
                }
                else
                {
                    // Nếu không chọn địa điểm, xóa tất cả địa điểm cũ
                    foreach (var carLocation in currentCarLocations)
                    {
                        await _carRentalLocationService.DeleteAsync(carLocation.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateCarRentalLocations error: {ex.Message}");
                // Không throw để không làm gián đoạn quá trình cập nhật xe
            }
        }

        private void RentalLocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Bỏ qua nếu đang trong quá trình update selection (tránh loop)
            if (_isUpdatingSelection) return;

            try
            {
                var selectedLocation = RentalLocationComboBox.SelectedItem as RentalLocationDTO;
                
                if (selectedLocation != null)
                {
                    System.Diagnostics.Debug.WriteLine($"RentalLocationComboBox selection changed: {selectedLocation.Name} (Id: {selectedLocation.Id})");
                    
                    // Nếu user thay đổi location manually (không phải từ LoadCarRentalLocations),
                    // có thể clear _selectedCar để biết đây là thao tác tạo mới
                    // Hoặc giữ nguyên _selectedCar nếu đang edit
                    // Tùy vào logic nghiệp vụ, có thể comment/uncomment dòng dưới:
                    // if (_selectedCar != null && _selectedCar.Id > 0)
                    // {
                    //     // User đang edit và thay đổi location
                    //     System.Diagnostics.Debug.WriteLine($"User changed location for car ID: {_selectedCar.Id}");
                    // }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("RentalLocationComboBox selection cleared");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RentalLocationComboBox_SelectionChanged error: {ex.Message}");
            }
        }
    }
}
