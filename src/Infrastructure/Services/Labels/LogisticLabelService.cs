using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Enums;
using Uni.Scan.Shared.Exceptions;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using static MudBlazor.CategoryTypes;

namespace Uni.Scan.Infrastructure.Services.Labels
{
    public interface ILogisticLabelService
    {
        Task<IResult<LogisticTaskLabelDTO2>> SaveLabelAsync(LogisticTaskLabelDTO2 request);
        Task<IResult<bool>> DeleteLabelAsync(LogisticTaskLabelDTO2 request);
    }

    public class LogisticLabelService : ILogisticLabelService
    {
        private readonly UniContext _uniContext;
        private readonly BydIdentifiedStockService _bydIdentifiedStockService;


        public LogisticLabelService(UniContext uniContext, BydIdentifiedStockService bydIdentifiedStockService)
        {
            _uniContext = uniContext;
            _bydIdentifiedStockService = bydIdentifiedStockService;
        }

        public async Task<IResult<LogisticTaskLabelDTO2>> SaveLabelAsync(LogisticTaskLabelDTO2 request)
        {
            Serilog.Log.Debug("start SaveLabelAsync  {0}", request.ProductId);


            var matrial = (_uniContext.Materials.FirstOrDefault(o => o.ProductID == request.ProductId) ??
                           throw new ApiException("ProductId introuvable dans la base")).Adapt<MaterialDTO>();

            Serilog.Log.Debug("IsBatchManaged  {0}", matrial.IsBatchManaged);
            request.ProductName = matrial.Description_KUT;


            if (matrial.IsBatchManaged == false)
            {
                request.IdentifiedStock = "";
                request.SupplierIdentifiedStock = "";
                request.ProductionDate = null;
                request.ExpirationDate = null;
                var result = await SaveToDatabase(request);
                return await Result<LogisticTaskLabelDTO2>.SuccessAsync(result);
            }

            request.IdentifiedStock = request.IdentifiedStock.ToUpper().Trim();
            var bydStockId = await _bydIdentifiedStockService.GetStockIDAsync(request.IdentifiedStock, request.ProductId);

            if (bydStockId == null)
            {
                if (request.Type == LabelType.Free)
                {
                    throw new ApiException("Stock Introuvable dans SAP");
                }

                var created = await _bydIdentifiedStockService.CreateStockID(request.IdentifiedStock, request.ProductId, request.SupplierIdentifiedStock,
                    request.ExpirationDate,
                    request.ProductionDate);

                if (created)
                {
                    var result = await SaveToDatabase(request);
                    return await Result<LogisticTaskLabelDTO2>.SuccessAsync(result);
                }
                else
                {
                    throw new ApiException("Erreur de création dans SAP");
                }
            }
            else
            {
                if (bydStockId.LifeCycleStatusCode != "2" && request.Type == LabelType.Linked)
                {
                    throw new ApiException("Le stock n'est pas actif sur SAP");
                }

                if (request.Type == LabelType.Free)
                {
                    request.QuatityUnite = matrial.BaseMeasureUnitCode;
                    request.SupplierIdentifiedStock = bydStockId.IdentifiedStockPartyID ?? String.Empty;
                    request.ProductionDate = bydStockId.ProductionDateTime?.DateTime;
                    request.ExpirationDate = bydStockId.ExpirationDateTime?.DateTime;
                }

                if (request.Type == LabelType.Linked && (request.SupplierIdentifiedStock != bydStockId.IdentifiedStockPartyID ||
                                                         request.ExpirationDate != bydStockId.ExpirationDateTime?.DateTime ||
                                                         request.ProductionDate != bydStockId.ProductionDateTime?.DateTime))
                {
                    var updated = await _bydIdentifiedStockService.UpdateStockID(bydStockId.ObjectID, request.SupplierIdentifiedStock,
                        request.ExpirationDate,
                        request.ProductionDate);
                    if (!updated)
                    {
                        throw new ApiException("Erreur de mise à jour dans SAP");
                    }
                }

                var result = await SaveToDatabase(request);
                return await Result<LogisticTaskLabelDTO2>.SuccessAsync(result);
            }
        }

        public async Task<IResult<bool>> DeleteLabelAsync(LogisticTaskLabelDTO2 request)
        {
            Serilog.Log.Debug("start DeleteLabelAsync  {0}", request.Id);

            var lable = _uniContext.LogisticTaskLabels.FirstOrDefault(o => o.Id == request.Id);
            if (lable == null) throw new ApiException("Label introuvable !");
            _uniContext.Remove(lable);
            await _uniContext.SaveChangesAsync();

            Serilog.Log.Debug("end  DeleteLabelAsync {0}", lable.Id);

            return await Result<bool>.SuccessAsync();
        }

        private async Task<LogisticTaskLabelDTO2> SaveToDatabase(LogisticTaskLabelDTO2 request)
        {
            var dbObject = request.Adapt<LogisticTaskLabel>();
            var listToUpdate = _uniContext.LogisticTaskLabels.Where(o => o.ProductId == dbObject.ProductId && o.IdentifiedStock == dbObject.IdentifiedStock).ToList();
            foreach (var item in listToUpdate)
            {
                if (item.Id == request.Id)
                {
                    request.Adapt(item);
                }
                else
                {
                    item.SupplierIdentifiedStock = request.SupplierIdentifiedStock;
                    item.ExpirationDate = request.ExpirationDate ?? DateTime.MinValue;
                    item.ProductionDate = request.ProductionDate ?? DateTime.MinValue;
                    //if (item.TaskId == request.TaskId && item.LineItemID == request.LineItemID)
                    //{
                    //   // si il faut mettre les ordres aussi 
                    //}
                }
            }

            if (dbObject.Id == 0)
            {
                _uniContext.Add(request.Adapt<LogisticTaskLabel>());
            }
            //else
            //{
            //    _uniContext.Attach(dbObject);
            //    _uniContext.Entry(dbObject).State = EntityState.Modified;
            //}

            await _uniContext.SaveChangesAsync();
            return dbObject.Adapt<LogisticTaskLabelDTO2>();
        }
    }
}