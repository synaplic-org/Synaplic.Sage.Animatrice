using System.ComponentModel;

namespace Uni.Scan.Shared.Enums
{
    public enum AnomalyStatus
    {
        [Description("Nouveau")] Nouveau = 0,
        [Description("Clôturée")] Clôturée = 1,
        [Description("Rejeté")] Rejeté = 2,
        [Description("Annulée")] Annulée = 99
    }
}
