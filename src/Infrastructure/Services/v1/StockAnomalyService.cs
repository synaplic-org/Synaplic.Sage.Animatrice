using Mapster;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Common;
using Uni.Scan.Application.Interfaces.Repositories;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign.Logistics.Inbound;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Constants.Application;
using Uni.Scan.Shared.Enums;
using Uni.Scan.Shared.Exceptions;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.v1
{
    public interface IStockAnomalyService : IService
    {
        Task<IResult<List<StockAnomalyDTO>>> GetAllAsync();
        Task<IResult<StockAnomalyDTO>> GetByIdAsync(int id);
        Task<Result<int>> SaveAllAsync(StockAnomalyDTO request);
        Task<IResult<bool>> UpdateAsync(StockAnomalyDTO request);
        Task<IResult<bool>> DeleteAsync(int id);
        Task<IResult<int>> ValidateAnomalie(StockAnomalyDTO request);
        Task<IResult<int>> RejectAnomalie(StockAnomalyDTO request);
        Task<IResult<int>> CancelAnomalie(int id);
        Task<StockAnomalyDTO> SaveToDatabase(StockAnomalyDTO request);
        Task<IResult<StockAnomalyDTO>> SaveAnomalyAsync(StockAnomalyDTO request);
    }

    public class StockAnomalyService : IStockAnomalyService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UniContext _uniContext;
        private readonly BydMaterialService _bydMaterialService;


        public StockAnomalyService(UniContext uniContext, IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService, BydMaterialService bydMaterialService)
        {
            _uniContext = uniContext;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _bydMaterialService = bydMaterialService;
        }


        public async Task<IResult<List<StockAnomalyDTO>>> GetAllAsync()
        {
            var getAll = _unitOfWork.Repository<StockAnomaly>().Entities.ToList().Adapt<List<StockAnomalyDTO>>();
            return await Result<List<StockAnomalyDTO>>.SuccessAsync(getAll);
        }

        public async Task<IResult<StockAnomalyDTO>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _unitOfWork.Repository<StockAnomaly>().GetByIdAsync(id);
                var result = response.Adapt<StockAnomalyDTO>();
                return await Result<StockAnomalyDTO>.SuccessAsync(result);
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, "##" + this.GetType().FullName);
                return await Result<StockAnomalyDTO>.FailAsync(e.Message);
            }
        }


        public async Task<Result<int>> SaveAllAsync(StockAnomalyDTO request)
        {
            if (request.Id == 0)
            {
                var anomaly = request.Adapt<StockAnomaly>();
                await _unitOfWork.Repository<StockAnomaly>().AddAsync(anomaly);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);
                return await Result<int>.SuccessAsync(anomaly.Id);
            }
            else
            {
                var anomaly = await _unitOfWork.Repository<StockAnomaly>().GetByIdAsync(request.Id);
                if (anomaly != null)
                {
                    anomaly.AnomalyType = request.AnomalyType;
                    anomaly.CompanyID = request.CompanyID;
                    anomaly.SiteID = request.SiteID;
                    anomaly.OwnerPartyID = request.OwnerPartyID;
                    anomaly.InventoryRestrictedUseIndicator = request.InventoryRestrictedUseIndicator;
                    anomaly.InventoryStockStatusCode = request.InventoryStockStatusCode;
                    anomaly.IdentifiedStockID = request.IdentifiedStockID;
                    anomaly.LogisticsAreaID = request.LogisticsAreaID;
                    anomaly.Quantity = request.Quantity;
                    anomaly.QuantityUniteCode = request.QuantityUniteCode;
                    anomaly.IdentifiedStockType = request.IdentifiedStockType;
                    anomaly.IdentifiedStockTypeCode = request.IdentifiedStockTypeCode;
                    anomaly.LogisticsArea = request.LogisticsArea;
                    anomaly.ProductDescription = request.ProductDescription;
                    anomaly.ProductID = request.ProductID;
                    anomaly.CorrectedIdentifiedStockID = request.CorrectedIdentifiedStockID;
                    anomaly.CorrectedQuantity = request.CorrectedQuantity;
                    anomaly.AnomalyReason = request.AnomalyReason;
                    anomaly.AnomalyStatus = request.AnomalyStatus;
                    anomaly.DeclaredBy = request.DeclaredBy;
                    anomaly.ClosedBy = request.ClosedBy;
                    anomaly.CloseOn = request.CloseOn;
                    anomaly.CreatedOn = DateTime.Now;


                    await _unitOfWork.Repository<StockAnomaly>().UpdateAsync(anomaly);
                    await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);
                    return await Result<int>.SuccessAsync(anomaly.Id, " mis a jour");
                }
                else
                {
                    return await Result<int>.FailAsync("Anomalie Introuvable!");
                }
            }
        }

        public async Task<IResult<int>> ValidateAnomalie(StockAnomalyDTO request)
        {
            var _anomalyDTO = await _unitOfWork.Repository<StockAnomaly>().GetByIdAsync(request.Id);
            _anomalyDTO.AnomalyStatus = AnomalyStatus.Clôturée;
            _anomalyDTO.AnomalyReason = request.AnomalyReason;
            await _unitOfWork.Repository<StockAnomaly>().UpdateAsync(_anomalyDTO);
            await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);
            return await Result<int>.SuccessAsync();
        }

        public async Task<IResult<int>> RejectAnomalie(StockAnomalyDTO request)
        {
            var _anomalyDTO = await _unitOfWork.Repository<StockAnomaly>().GetByIdAsync(request.Id);
            _anomalyDTO.AnomalyStatus = AnomalyStatus.Rejeté;
            _anomalyDTO.AnomalyReason = request.AnomalyReason;
            await _unitOfWork.Repository<StockAnomaly>().UpdateAsync(_anomalyDTO);
            await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);
            return await Result<int>.SuccessAsync();
        }
        public async Task<StockAnomalyDTO> SaveToDatabase(StockAnomalyDTO request)
        {
            var dbObject = request.Adapt<StockAnomaly>();
            if (dbObject.Id == 0)
            {
                _uniContext.Add(dbObject);
            }
            else
            {
                _uniContext.Attach(dbObject);
                _uniContext.Entry(dbObject).State = EntityState.Modified;
            }

            await _uniContext.SaveChangesAsync();
            return dbObject.Adapt<StockAnomalyDTO>();
        }
        public async Task<IResult<StockAnomalyDTO>> SaveAnomalyAsync(StockAnomalyDTO request)
        {
            Serilog.Log.Debug("start SaveAnomalyAsync  {0}", request.ProductID);

            var dbMaterial = (_uniContext.Materials.FirstOrDefault(o => o.ProductID == request.ProductID) ??
                          throw new ApiException("{0} introuvable dans SAP ! ", request.ProductID)).Adapt<StockAnomalyDTO>();
            var oDataMaterial = await _bydMaterialService.GetMaterialAsync(request.ProductID);
            request.DeclaredBy = _currentUserService.UserId;
            request.CreatedOn = DateTime.Now;
            request.ProductDescription = oDataMaterial.Material.Description_KUT;

            if (oDataMaterial != null)
            {
                await SaveToDatabase(request);
                return await Result<StockAnomalyDTO>.SuccessAsync();
            }
            else
            {
                throw new ApiException("Erreur création dans la base.");
            }
        }
        public async Task<IResult<int>> CancelAnomalie(int id)
        {
            var _anomalyDTO = await _unitOfWork.Repository<StockAnomaly>().GetByIdAsync(id);
            _anomalyDTO.AnomalyStatus = AnomalyStatus.Annulée;
            await _unitOfWork.Repository<StockAnomaly>().UpdateAsync(_anomalyDTO);
            await _unitOfWork.CommitAndRemoveCache(new CancellationToken(), ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);
            return await Result<int>.SuccessAsync();
        }

        public async Task<IResult<bool>> UpdateAsync(StockAnomalyDTO request)
        {
            var anomaly = await _unitOfWork.Repository<StockAnomaly>().GetByIdAsync(request.Id);
            if (anomaly != null)
            {
                anomaly.AnomalyType = request.AnomalyType;
                anomaly.CompanyID = request.CompanyID;
                anomaly.SiteID = request.SiteID;
                anomaly.OwnerPartyID = request.OwnerPartyID;
                anomaly.InventoryRestrictedUseIndicator = request.InventoryRestrictedUseIndicator;
                anomaly.InventoryStockStatusCode = request.InventoryStockStatusCode;
                anomaly.IdentifiedStockID = request.IdentifiedStockID;
                anomaly.LogisticsAreaID = request.LogisticsAreaID;
                anomaly.Quantity = request.Quantity;
                anomaly.QuantityUniteCode = request.QuantityUniteCode;
                anomaly.IdentifiedStockType = request.IdentifiedStockType;
                anomaly.IdentifiedStockTypeCode = request.IdentifiedStockTypeCode;
                anomaly.LogisticsArea = request.LogisticsArea;
                anomaly.ProductDescription = request.ProductDescription;
                anomaly.ProductID = request.ProductID;
                anomaly.CorrectedIdentifiedStockID = request.CorrectedIdentifiedStockID;
                anomaly.CorrectedQuantity = request.CorrectedQuantity;
                anomaly.AnomalyReason = request.AnomalyReason;
                anomaly.AnomalyStatus = request.AnomalyStatus;
                anomaly.DeclaredBy = request.DeclaredBy;
                anomaly.ClosedBy = request.ClosedBy;
                anomaly.CloseOn = request.CloseOn;
                anomaly.CreatedOn = DateTime.Now;

                await _unitOfWork.Repository<StockAnomaly>().UpdateAsync(anomaly);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(),
                    ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);
                return await Result<bool>.SuccessAsync(data: true, "Anomalie mis a jour");
            }
            else
            {
                return await Result<bool>.FailAsync("Anomaly Introuvable!");
            }
        }

        public async Task<IResult<bool>> DeleteAsync(int id)
        {
            var param = await _unitOfWork.Repository<StockAnomaly>().GetByIdAsync(id);
            if (param != null)
            {
                await _unitOfWork.Repository<StockAnomaly>().DeleteAsync(param);
                await _unitOfWork.CommitAndRemoveCache(new CancellationToken(),
                    ApplicationConstants.Cache.GetAllStockAnomalyCacheKey);

                return await Result<bool>.SuccessAsync(data: true, "Anomalie supprimé");
            }
            else
            {
                return await Result<bool>.FailAsync("Anomalie Introuvable!");
            }
        }
    }
}