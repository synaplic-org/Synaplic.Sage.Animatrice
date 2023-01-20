using Mapster;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Common;
using Uni.Scan.Application.Interfaces.Repositories;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Constants.Application;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.v1
{
    public interface IScanningCodeService : IService
    {
        Task<Result<List<ScanningCodeDTO>>> GetAllAsync();
        Task<ScanningCodeDTO> GetByIdAsync(int id);
        Task<Result<bool>> SaveAllAsync(List<ScanningCodeDTO> request);
        Task<Result<int>> DeleteAsync(int id);
        Task<Result<bool>> SaveAsync(ScanningCodeDTO request);
        Task<Result<bool>> UpdateAsync(List<ScanningCodeDTO> codeList);
    }
    public class ScanningCodeService : IScanningCodeService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly UniContext _uniContext;



        public ScanningCodeService(IUnitOfWork<int> unitOfWork, UniContext uniContext)
        {
            _unitOfWork = unitOfWork;
            _uniContext = uniContext;

        }
        public async Task<Result<List<ScanningCodeDTO>>> GetAllAsync()
        {

            var getAll = _unitOfWork.Repository<ScanningCode>().Entities.ToList().Adapt<List<ScanningCodeDTO>>();
            return await Result<List<ScanningCodeDTO>>.SuccessAsync(getAll);
        }

        public async Task<ScanningCodeDTO> GetByIdAsync(int id)
        {
            var response = await _unitOfWork.Repository<ScanningCode>().GetByIdAsync(id);
            var scancode = response.Adapt<ScanningCodeDTO>();
            return scancode;
        }
        public async Task<Result<bool>> SaveAllAsync(List<ScanningCodeDTO> request)
        {
            foreach (var item in request)
            {
                var scancode = item.Adapt<ScanningCode>();
                await _unitOfWork.Repository<ScanningCode>().AddAsync(scancode);

            }
            await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllParametresCachekey);
            return await Result<bool>.SuccessAsync();
        }
        public async Task<Result<bool>> SaveAsync(ScanningCodeDTO request)
        {
            var scancode = request.Adapt<ScanningCode>();
            await _unitOfWork.Repository<ScanningCode>().AddAsync(scancode);
            await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllParametresCachekey);
            return await Result<bool>.SuccessAsync();
        }
        public async Task<Result<bool>> UpdateAsync(List<ScanningCodeDTO> codeList)
        {
            foreach (var x in codeList)
            {
                var scanningcodes = new List<string>();
                scanningcodes.Add(x.BarCodeType);
                scanningcodes.Add(x.BarCodePrefix);
                scanningcodes.Add(x.BarCodeSuffix);
                var param = new LogisticParametres
                {
                    ParametreID ="ScanningCodes",
                    Owner = "*",
                    ValueString =  JsonConvert.SerializeObject(scanningcodes)
                };
                await _unitOfWork.Repository<LogisticParametres>().AddAsync(param);
            }
            await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllScanningCodeCacheKey);
            return await Result<bool>.SuccessAsync(true, "mis à jour");
        }
        public async Task<Result<int>> DeleteAsync(int id)
        {


            var scancode = await _unitOfWork.Repository<ScanningCode>().GetByIdAsync(id);
            if (scancode != null)
            {
                await _unitOfWork.Repository<ScanningCode>().DeleteAsync(scancode);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllScanningCodeCacheKey);

                return await Result<int>.SuccessAsync(id, "paramètre supprimé");
            }
            else
            {

                return await Result<int>.FailAsync("paramètre Introuvable!");
            }


        }
    }
}
