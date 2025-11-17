using AssignmentPRN212.DTO;
using AssignmentPRN212.Services;
using System;
using System.Windows;

namespace AssignmentPRN212.Views
{
    public partial class OrderDetailWindow : Window
    {
        private readonly RentalOrderDTO _order;
        private readonly ApiService _apiService;

        public OrderDetailWindow(ApiService apiService, RentalOrderDTO order)
        {
            InitializeComponent();
            _apiService = apiService;
            _order = order;
            LoadOrderDetails();
        }

        private async void LoadOrderDetails()
        {
            OrderIdTextBlock.Text = $"#{_order.Id}";
            PhoneNumberTextBlock.Text = _order.PhoneNumber;
            OrderDateTextBlock.Text = _order.OrderDate.ToString("dd/MM/yyyy HH:mm");
            PickupTimeTextBlock.Text = _order.PickupTime.ToString("dd/MM/yyyy HH:mm");
            ReturnTimeTextBlock.Text = _order.ExpectedReturnTime.ToString("dd/MM/yyyy HH:mm");
            WithDriverTextBlock.Text = _order.WithDriver ? "Có" : "Không";
            StatusTextBlock.Text = _order.Status;

            // Tính phí tài xế nếu có
            double driverFee = 0;
            if (_order.WithDriver)
            {
                try
                {
                    var carService = new CarService(_apiService);
                    var car = await carService.GetCarByIdAsync(_order.CarId);
                    if (car != null)
                    {
                        int days = (_order.ExpectedReturnTime - _order.PickupTime).Days + 1;
                        driverFee = (car.RentPricePerDayWithDriver - car.RentPricePerDay) * days;
                    }
                }
                catch
                {
                    // Nếu không load được thông tin xe, phí tài xế = 0
                }
            }

            // Hiển thị giá
            SubTotalTextBlock.Text = $"{(_order.SubTotal ?? 0):N0} VNĐ";
            DepositTextBlock.Text = $"{(_order.Deposit ?? 0):N0} VNĐ";
            DriverFeeTextBlock.Text = $"{driverFee:N0} VNĐ";
            ExtraFeeTextBlock.Text = $"{(_order.ExtraFee ?? 0):N0} VNĐ";
            DamageFeeTextBlock.Text = $"{(_order.DamageFee ?? 0):N0} VNĐ";
            DiscountTextBlock.Text = $"{(_order.Discount ?? 0)}%";
            // Tổng tiền = Deposit + SubTotal
            double totalAmount = (_order.Deposit ?? 0) + (_order.SubTotal ?? 0);
            TotalTextBlock.Text = $"{totalAmount:N0} VNĐ";
        }
    }
}

