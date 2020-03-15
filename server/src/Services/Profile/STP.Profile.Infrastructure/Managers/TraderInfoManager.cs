using STP.Common.Exceptions;
using STP.Infrastructure;
using STP.Interfaces.Enums;
using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Infrastructure.Validation.TraderInfo;
using STP.Profile.Interfaces.DataAccess;
using STP.Profile.Interfaces.Managers;
using System.Threading.Tasks;

namespace STP.Profile.Infrastructure.Managers
{
    public class TraderInfoManager : ITraderInfoManager
    {
        private readonly ITraderInfoRepository _repository;
        private readonly IdentityHttpService _identityHttpService;
        public TraderInfoManager(ITraderInfoRepository repository, IdentityHttpService identityHttpService)
        {
            _repository = repository;
            _identityHttpService = identityHttpService;
        }

        public async Task<TraderInfoEntity[]> GetAllAsync(BaseFilterModel filter)
        {
            var entities = await _repository.GetAllAsync(filter);
            if (entities == null)
            {
                throw new NotFoundException(ErrorCode.TraderInfoNotFound);
            }
            return entities;
        }

        public async Task<TraderInfoEntity[]> GetByFilterAsync(TraderInfoFilterModel filter)
        {
            var entities = await _repository.GetByFilterAsync(filter);
            if (entities == null)
            {
                throw new NotFoundException(ErrorCode.TraderInfoNotFound);
            }
            return entities;
        }

        public async Task<TraderInfoEntity[]> GetByCopyCountAsync(BaseFilterModel filter)
        {
            var entities = await _repository.GetByCopyCountAsync(filter);
            if (entities == null)
            {
                throw new NotFoundException(ErrorCode.TraderInfoNotFound);
            }
            return entities;
        }

        public async Task<TraderInfoEntity> UpdateAsync(TraderInfoEntity entity)
        {
            var result = _repository.Update(entity);
            if (result == null)
            {
                throw new InvalidDataException(ErrorCode.CannotUpdateTraderInfo);
            }
            await _repository.UnitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<TraderInfoEntity> CreateAsync(TraderInfoEntity entity)
        {
            var validationResult = await new TraderInfoEntityValidation().ValidateAsync(entity);
            if (! validationResult.IsValid)
            {
                throw new InvalidDataException(ErrorCode.CannotCreateTraderInfo, string.Join(" ", validationResult.Errors));
            }

            if( await _repository.IsExistAsync(entity.Id))
            {
                throw new InvalidDataException(ErrorCode.TraderInfoIdExist);
            }

            if(! await _identityHttpService.IsExistUserAsync(entity.Id))
            {
                throw new InvalidDataException(ErrorCode.UserNotFound);
            }

            var result = await _repository.InsertAsync(entity);
            if (result == null)
            {
                throw new InvalidDataException(ErrorCode.CannotCreateTraderInfo);
            }

            await _repository.UnitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<TraderInfoEntity> GetByIdAsync(string id)
        {
            var entity = await _repository.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(ErrorCode.TraderInfoNotFound);
            }
            return entity;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var removed = _repository.Remove(id);
            if (! removed)
            {
                throw new NotFoundException(ErrorCode.TraderInfoNotFound);
            }
            await _repository.UnitOfWork.SaveChangesAsync();
            return true;
        }

    }
}
