using AutoMapper;
using Repository.Entities;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User Mappings
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            CreateMap<CreateStaffUserDTO, User>();

            //Payment Mappings
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.UserFullname, opt => opt.MapFrom(src => src.RentalOrder.User.FullName))
                .ForMember(dest => dest.Car, opt => opt.MapFrom(src => src.RentalOrder.Car.Name))
                .ForMember(dest => dest.OrderDate, otp => otp.MapFrom(src => src.RentalOrder.OrderDate));
            CreateMap<CreatePaymentDTO, Payment>();

            //RentalLocation Mappings
            CreateMap<RentalLocation, RentalLocationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            CreateMap<CreateRentalLocationDTO, RentalLocation>();

            //CarDeliveryHistory Mappings
            CreateMap<CarDeliveryHistory, CarDeliveryHistoryDTO>().ReverseMap();
            CreateMap<CarDeliveryHistoryCreateDTO, CarDeliveryHistory>();
            CreateMap<CarDeliveryHistoryUpdateDTO, CarDeliveryHistory>();

            //CarRentalLocation Mappings
            CreateMap<CarRentalLocation, CarRentalLocationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId));
            //CarReturnHistory Mappings
            CreateMap<CreateCarRentalLocationDTO, CarRentalLocation>();
            CreateMap<CarReturnHistory, CarReturnHistoryDTO>().ReverseMap();
            CreateMap<CarReturnHistory, CarReturnHistoryCreateDTO>().ReverseMap();

            //CitizenId Mappings
            CreateMap<CitizenId, CitizenIdDTO>().ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CitizenIdNumber, opt => opt.MapFrom(src => src.CitizenIdNumber))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ImageUrl2, opt => opt.MapFrom(src => src.ImageUrl2))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.RentalOrderId, opt => opt.MapFrom(src => src.RentalOrderId));
            CreateMap<CreateCitizenIdDTO, CitizenId>();

            //DriverLicense Mappings
            CreateMap<DriverLicense, DriverLicenseDTO>().ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ImageUrl2, opt => opt.MapFrom(src => src.ImageUrl2))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.RentalOrderId, opt => opt.MapFrom(src => src.RentalOrderId));
            CreateMap<CreateDriverLicenseDTO, DriverLicense>();

            //RentalOrder Mappings
            CreateMap<RentalOrder, RentalOrderDTO>().ReverseMap()
                .ForMember(dest => dest.Id, otp => otp.MapFrom(src => src))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.PickupTime, opt => opt.MapFrom(src => src.PickupTime))
                .ForMember(dest => dest.ExpectedReturnTime, opt => opt.MapFrom(src => src.ExpectedReturnTime))
                .ForMember(dest => dest.ActualReturnTime, opt => opt.MapFrom(src => src.ActualReturnTime))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.ExtraFee, opt => opt.MapFrom(src => src.ExtraFee))
                .ForMember(dest => dest.DamageFee, opt => opt.MapFrom(src => src.DamageFee))
                .ForMember(dest => dest.DamageNotes, opt => opt.MapFrom(src => src.DamageNotes))
                .ForMember(dest => dest.WithDriver, opt => opt.MapFrom(src => src.WithDriver))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.RentalLocationId, opt => opt.MapFrom(src => src.RentalLocationId))
                .ForMember(dest => dest.CitizenId, opt => opt.MapFrom(src => src.CitizenId))
                .ForMember(dest => dest.DriverLicenseId, opt => opt.MapFrom(src => src.DriverLicenseId))
                .ForMember(dest => dest.RentalContactId, opt => opt.MapFrom(src => src.RentalContactId))
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId));
            CreateMap<CreateRentalOrderDTO, RentalOrder>();
        }
    }
}
