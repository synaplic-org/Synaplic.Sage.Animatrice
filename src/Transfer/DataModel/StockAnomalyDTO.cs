using System;
using Uni.Scan.Shared.Enums;

namespace Uni.Scan.Transfer.DataModel
{
    public class StockAnomalyDTO
    {
        public int Id { get; set; }
        public AnomalyType AnomalyType { get; set; }
        public string CompanyID { get; set; }
        public string SiteID { get; set; }
        public string OwnerPartyID { get; set; }
        public bool InventoryRestrictedUseIndicator { get; set; }
        public string InventoryStockStatusCode { get; set; }
        public string IdentifiedStockID { get; set; }
        public string LogisticsAreaID { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityUniteCode { get; set; }
        public string IdentifiedStockType { get; set; }
        public string IdentifiedStockTypeCode { get; set; }
        public string LogisticsArea { get; set; }
        public string ProductID { get; set; }
        public string ProductDescription { get; set; }
        public string CorrectedIdentifiedStockID { get; set; }
        public decimal CorrectedQuantity { get; set; }
        public string AnomalyReason { get; set; }
        public AnomalyStatus AnomalyStatus { get; set; }
        public string DeclaredBy { get; set; }
        public string ClosedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime? CloseOn { get; set; }
        //public class StockAnomalyDTOValidator : AbstractValidator<StockAnomalyDTO>
        //{
        //    public StockAnomalyDTOValidator(IStringLocalizer<StockAnomalyDTOValidator> localizer)
        //    {
        //        RuleFor(request => request.ProductID)
        //            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Le Produit est requis !"]);
        //        RuleFor(request => request.LogisticsArea)
        //       .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["La Zone est requise !"]);
        //        RuleFor(request => request.Quantity)
        //          .NotEmpty().NotEqual(0).WithMessage(x => localizer["La Quantité est requise !"]);
        //        RuleFor(request => request.IdentifiedStockID)
        //        .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Le Lot est requis !"]);
        //    }
        //}
    }
}