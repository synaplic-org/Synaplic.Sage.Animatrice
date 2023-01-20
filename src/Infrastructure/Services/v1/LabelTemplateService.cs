using AutoMapper;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Common;
using Uni.Scan.Application.Interfaces.Repositories;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.Models.Identity;
using Uni.Scan.Shared.Constants.Application;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Responses.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Uni.Scan.Infrastructure.Services.v1
{
  
    public interface ILabelTemplateService : IService
    {
        Task<Result<List<LabelTemplateDTO>>> GetAllAsync();
        Task<Result<int>> SaveAsync(LabelTemplateDTO request);
        Task<Result<int>> DeleteAsync(int id);

    }
    public class LabelTemplateService : ILabelTemplateService
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        private readonly IMapper _mapper;

        public LabelTemplateService(IMapper mapper, IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<List<LabelTemplateDTO>>> GetAllAsync()
        {

            var getAll = _unitOfWork.Repository<LabelTemplate>().Entities.ToList().Adapt<List<LabelTemplateDTO>>();
            return await Result<List<LabelTemplateDTO>>.SuccessAsync(getAll);
        }
        

        public async Task<Result<int>> SaveAsync(LabelTemplateDTO request)
        {
            if (request.Id == 0)
            {
                var parametre = request.Adapt<LabelTemplate>();
                await _unitOfWork.Repository<LabelTemplate>().AddAsync(parametre);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllParametresCachekey);
                return await Result<int>.SuccessAsync(parametre.Id);
            }
            else
            {
                var p = await _unitOfWork.Repository<LabelTemplate>().GetByIdAsync(request.Id);
                if (p != null)
                {
                    p.ModelID = request.ModelID;
                    p.ModelName = request.ModelName;
                    p.Type = request.Type;

                    await _unitOfWork.Repository<LabelTemplate>().UpdateAsync(p);
                    await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllParametresCachekey);
                    return await Result<int>.SuccessAsync(p.Id, "Modèle mis a jour");
                }
                else
                {
                    return await Result<int>.FailAsync("Modèle Not Found!");
                }
            }
        }
        public async Task<Result<int>> DeleteAsync(int id)
        {


            var param = await _unitOfWork.Repository<LabelTemplate>().GetByIdAsync(id);
            if (param != null)
            {
                await _unitOfWork.Repository<LabelTemplate>().DeleteAsync(param);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllParametresCachekey);

                return await Result<int>.SuccessAsync(id, "Parametres Deleted");
            }
            else
            {

                return await Result<int>.FailAsync("Parametres Not Found!");
            }

        }

    }
}
