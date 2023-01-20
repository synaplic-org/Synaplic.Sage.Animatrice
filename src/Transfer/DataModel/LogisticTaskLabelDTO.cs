using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Uni.Scan.Shared.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace Uni.Scan.Transfer.DataModel
{
    public class LogisticTaskLabelDTO
    {
        public int Id { get; set; }
        [Required] public int NbrEtiquettes { get; set; }

        [Required] public decimal PlanQuantity { get; set; }

        //[Required] 
        public string ProductId { get; set; }
        public string IdentifiedStock { get; set; }
        public string SupplierIdentifiedStock { get; set; }
        public LabelType Type { get; set; }
        public string TaskLineId { get; set; }
        public string LabelUuid { get; set; }
        public int Duplicata { get; set; } = 1; 
        public int Supplement { get; set; }
        public string TaskId { get; set; }
        public int LineItemID { get; set; }
        public string Title { get; set; }
        public string ProductName { get; set; }
        public string SerialStock { get; set; }
        public int Status { get; set; }

        private decimal _QuatityOnLabel;

        [Required]
        public decimal QuatityOnLabel
        {
            get => _QuatityOnLabel;
            set
            {
                if (value > 0 && PlanQuantity > 0)
                {
                    _QuatityOnLabel = value;
                    NbrEtiquettes = (int)(PlanQuantity / _QuatityOnLabel);
                }
                else
                {
                    _QuatityOnLabel = 1;
                    NbrEtiquettes = (int)(PlanQuantity);
                }
            }
        }

        public string QuatityUnite { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ExternalID { get; set; }
        public string ProductSpecification { get; set; }
        public string GTIN { get; set; }
        public string PackageID { get; set; }
        public string TransferOrdre { get; set; }
        public string ProductionOrdre { get; set; }
        public string FabricationOrdre { get; set; }
        public string Tare { get; set; }
        public string Comment { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }

    public class LogisticTaskLabelDTOValidator : AbstractValidator<LogisticTaskLabelDTO>
    {
        public LogisticTaskLabelDTOValidator(IStringLocalizer<LogisticTaskLabelDTOValidator> localizer)
        {
            RuleFor(request => request.ProductId)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Le Produit est requis !"]);
            RuleFor(request => request.NbrEtiquettes)
                .NotEmpty().NotEqual(0).WithMessage(x => localizer["Nombres d'étiquettes requis!"]);
            RuleFor(request => request.PlanQuantity)
                .NotEmpty().NotEqual(0).WithMessage(x => localizer["La Quantité est requise !"]);
            RuleFor(request => request.IdentifiedStock)
                .NotNull().WithMessage(x => localizer["Le Lot est requis !"]);
            RuleFor(request => request.QuatityOnLabel)
                .NotNull().NotEqual(0).WithMessage(x => localizer["Le Colisage est requis !"]);
            //RuleFor(request => request.SupplierIdentifiedStock)
            //    .NotNull().WithMessage(x => localizer["Le Lot du Fournisseur est requis !"]);
        }
    }
}