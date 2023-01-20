using System.ComponentModel;

namespace Uni.Scan.Shared.Enums
{
    public enum AnomalyType
    {
        [Description("Produit Introuvable")] Produit_Introuvable,

        [Description("Lot et Quantité Erronée")]
        Lot_Quantite_Erronee,
        [Description("Lot  Erronée")] Lot_Erronee,
        [Description("Quantité Erronée")] Quantite_Erronee,
        [Description("Produit Trouvé")] Produit_Trouve
    }
}