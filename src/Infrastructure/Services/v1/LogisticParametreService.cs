using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Uni.Scan.Application.Interfaces.Common;
using Uni.Scan.Application.Interfaces.Repositories;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Shared.Constants.Application;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Mapster;
using System.Linq;

namespace Uni.Scan.Infrastructure.Services.v1
{
    public interface ILogisticParametreService : IService
    {
        Task<Result<List<LogisticParametresDTO>>> GetAllAsync();
        Task<LogisticParametresDTO> GetByIdAsync(int id);
        Task<Result<int>> SaveAsync(LogisticParametresDTO request);
        Task<Result<int>> DeleteAsync(int id);
    }
    public class LogisticParametreService : ILogisticParametreService
    {
        private readonly IUnitOfWork<int> _unitOfWork;


        public LogisticParametreService(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<List<LogisticParametresDTO>>> GetAllAsync()
        {

            var getAll = _unitOfWork.Repository<LogisticParametres>().Entities.ToList().Adapt<List<LogisticParametresDTO>>();
            return await Result<List<LogisticParametresDTO>>.SuccessAsync(getAll);
        }

        public async Task<LogisticParametresDTO> GetByIdAsync(int id)
        {
            var response = await _unitOfWork.Repository<LogisticParametres>().GetByIdAsync(id);
            var prm = response.Adapt<LogisticParametresDTO>();
            return prm;
        }

        public async Task<Result<int>> SaveAsync(LogisticParametresDTO request)
        {
            var prm = request.Adapt<LogisticParametres>();
            await _unitOfWork.Repository<LogisticParametres>().AddAsync(prm);
            await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);
            return await Result<int>.SuccessAsync(prm.Id);
        }
        public async Task<Result<int>> DeleteAsync(int id)
        {


            var param = await _unitOfWork.Repository<LogisticParametres>().GetByIdAsync(id);
            if (param != null)
            {
                await _unitOfWork.Repository<LogisticParametres>().DeleteAsync(param);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);

                return await Result<int>.SuccessAsync(id, "paramètre supprimé");
            }
            else
            {

                return await Result<int>.FailAsync("paramètre Introuvable!");
            }


        }
    }
}
