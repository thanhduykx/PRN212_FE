using AutoMapper;
using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper, IUserRepository userRepository)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Result<CreatePaymentDTO>> AddAsync(CreatePaymentDTO createPaymentDTO)
        {
            var dto = _mapper.Map<Payment>(createPaymentDTO);
            var user = await _userRepository.GetByIdAsync(dto.UserId.Value);
            if (user == null)
            {
                return Result<CreatePaymentDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id của người dùng.");
            }
            var payment = new Payment
            {
                PaymentDate = dto.PaymentDate,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                UserId = user.Id,
                RentalOrderId = dto.RentalOrderId,
                User = user
            };
            await _paymentRepository.AddAsync(payment);
            return Result<CreatePaymentDTO>.Success(createPaymentDTO, "Tạo thanh toán thành công.");
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
            return Result<IEnumerable<PaymentDTO>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> GetAllByUserIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<IEnumerable<PaymentDTO>>.Failure("Người dùng không tồn tại! Kiểm tra lại Id.");
            }
            var payments = await _paymentRepository.GetAllByUserIdAsync(id);
            var dtos = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
            return Result<IEnumerable<PaymentDTO>>.Success(dtos);
        }

        public async Task<Result<PaymentDTO>> GetByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return Result<PaymentDTO>.Failure("Thanh toán không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<PaymentDTO>(payment);
            return Result<PaymentDTO>.Success(dto);
        }

        public Task<Result<UpdatePaymentStatusDTO>> UpdatePaymentStatusAsync(UpdatePaymentStatusDTO updatePaymentDTO)
        {
            throw new NotImplementedException();
        }
    }
}
