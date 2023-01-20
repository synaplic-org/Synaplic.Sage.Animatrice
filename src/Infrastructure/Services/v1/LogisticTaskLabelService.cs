using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Repositories;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Constants.Application;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.v1
{
    public interface ILogisticTaskLabelService
    {
        Task<IResult<LogisticTaskLabelDTO>> GetTaskLabel(int id);
        Task<Result<bool>> SaveAsync(List<LogisticTaskLabelDTO> request);
        Task<Result<int>> DeleteAsync(int id);
        Task<Result<List<LogisticTaskLabelDTO>>> GetAllAsync();
        Task<Result<List<LogisticTaskLabelDTO>>> GetAllTaskLabels(string taskId);
        Task<Result<int>> SaveLineAsync(LogisticTaskLabelDTO dto);
        Task<LogisticTaskLabelDTO> GetByIdAsync(int id);
    }

    public class LogisticTaskLabelService : ILogisticTaskLabelService
    {
        private readonly UniContext _uniContext;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<LogisticTaskLabelService> Localizer;
        private readonly ICurrentUserService _currentUserService;
        private readonly BydIdentifiedStockService _bydService;


        public LogisticTaskLabelService(IStringLocalizer<LogisticTaskLabelService> localizer, IUnitOfWork<int> unitOfWork, UniContext uniContext,
            ICurrentUserService currentUserService, BydIdentifiedStockService bydService)
        {
            _uniContext = uniContext;
            _unitOfWork = unitOfWork;
            Localizer = localizer;
            _currentUserService = currentUserService;
            _bydService = bydService;
        }

        public async Task<Result<List<LogisticTaskLabelDTO>>> GetAllAsync()
        {
            var getAll = _unitOfWork.Repository<LogisticTaskLabel>().Entities.Where(x => x.CreatedBy == _currentUserService.UserId).ToList().Adapt<List<LogisticTaskLabelDTO>>();
            return await Result<List<LogisticTaskLabelDTO>>.SuccessAsync(getAll);
        }

        public async Task<Result<List<LogisticTaskLabelDTO>>> GetAllTaskLabels(string taskId)
        {
            var getAll = _unitOfWork.Repository<LogisticTaskLabel>().Entities.Where(i => i.TaskId == taskId).ToList().Adapt<List<LogisticTaskLabelDTO>>();
            return await Result<List<LogisticTaskLabelDTO>>.SuccessAsync(getAll);
        }

        public async Task<IResult<LogisticTaskLabelDTO>> GetTaskLabel(int id)
        {
            var result = _uniContext.LogisticTaskLabels.Where(m => m.Id == id).Single();
            if (result != null)
            {
                var label = result.Adapt<LogisticTaskLabelDTO>();
                return await Result<LogisticTaskLabelDTO>.SuccessAsync(label);
            }
            else
            {
                return await Result<LogisticTaskLabelDTO>.FailAsync();
            }
        }

        public async Task<Result<bool>> SaveAsync(List<LogisticTaskLabelDTO> request)
        {
            var labels = request.Adapt<List<LogisticTaskLabel>>();

            foreach (var item in labels)
            {
                if (item.Id != 0)
                {
                    _uniContext.Attach(item);
                    _uniContext.Entry(item).State = EntityState.Modified;
                    item.CreatedBy = _currentUserService.UserId;
                    item.CreatedOn = DateTime.Now;
                    //item.Duplicata = 1;
                }
                else
                {
                    item.CreatedBy = _currentUserService.UserId;
                    item.CreatedOn = DateTime.Now;
                    //item.Duplicata = 1;
                    _uniContext.Add(item);
                }
            }


            try
            {
                await _uniContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error saving Labels");
                return await Result<bool>.FailAsync(Localizer["Une erreur sql est survenue "]);
            }

            return await Result<bool>.SuccessAsync(true);
        }

        public async Task<Result<int>> DeleteAsync(int id)
        {
            var label = await _unitOfWork.Repository<LogisticTaskLabel>().GetByIdAsync(id);
            if (label != null)
            {
                await _unitOfWork.Repository<LogisticTaskLabel>().DeleteAsync(label);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllLabelsCachekey);
                return await Result<int>.SuccessAsync(id, Localizer["Étiquette supprimé !"]);
            }
            else
            {
                return await Result<int>.FailAsync(Localizer["Étiquette Introuvable !"]);
            }
        }
        public async Task<Result<int>> SaveLineAsync(LogisticTaskLabelDTO dto)
        {
            var label = await _unitOfWork.Repository<LogisticTaskLabel>().GetByIdAsync(dto.Id);
            if (label != null)
            {
  
                await _unitOfWork.Repository<LogisticTaskLabel>().AddAsync(label);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllLabelsCachekey);
                return await Result<int>.SuccessAsync(dto.Id, Localizer["Étiquette mis a jour"]);
            }
            else
            {
                return await Result<int>.FailAsync(Localizer["Étiquette Introuvable!"]);
            }
        }

        public async Task<LogisticTaskLabelDTO> GetByIdAsync(int id)
        {
            var response = await _unitOfWork.Repository<LogisticTaskLabel>().GetByIdAsync(id);
            var p = response.Adapt<LogisticTaskLabelDTO>();
            return p;
        }
      
    }
}