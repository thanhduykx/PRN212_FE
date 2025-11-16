using AutoMapper;
using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RentalContactService : IRentalContactService
    {
        private readonly IRentalContactRepository _repo;
        private readonly IMapper _mapper;

        public RentalContactService(IRentalContactRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<RentalContactDTO>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<RentalContactDTO>>(list);
            return Result<IEnumerable<RentalContactDTO>>.Success(mapped);
        }

        public async Task<Result<RentalContactDTO?>> GetByRentalOrderIdAsync(int rentalOrderId)
        {
            var entity = await _repo.GetByRentalOrderIdAsync(rentalOrderId);
            if (entity == null || entity.IsDeleted)
                return Result<RentalContactDTO?>.Failure("Không tìm thấy hợp đồng.");

            return Result<RentalContactDTO?>.Success(_mapper.Map<RentalContactDTO>(entity));
        }

        public async Task<Result<string>> AddAsync(RentalContactCreateDTO dto)
        {
            var entity = _mapper.Map<RentalContact>(dto);
            entity.IsDeleted = false;

            await _repo.AddAsync(entity);
            return Result<string>.Success("Thêm hợp đồng thành công.");
        }

        public async Task<Result<string>> UpdateAsync(RentalContactUpdateDTO dto)
        {
            var entity = await _repo.GetByRentalOrderIdAsync(dto.RentalOrderId ?? 0);
            if (entity == null)
                return Result<string>.Failure("Không tìm thấy hợp đồng để cập nhật.");

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
            return Result<string>.Success("Cập nhật hợp đồng thành công.");
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            var entity = await _repo.GetByRentalOrderIdAsync(id);
            if (entity == null)
                return Result<string>.Failure("Không tìm thấy hợp đồng để xóa.");

            await _repo.DeleteAsync(id);
            return Result<string>.Success("Xóa hợp đồng thành công.");
        }
    }
}
