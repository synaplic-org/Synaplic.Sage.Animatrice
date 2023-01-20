using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uni.Scan.Shared.Enums;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.DataModel.LogisticTask;

namespace Uni.Scan.Client.Infrastructure.ApiClients
{
    public partial class BooleanIResult : Result<bool>
    {
    }

    public partial class BooleanResult : Result<bool>
    {
    }

    public partial class Int32Result : Result<Int32>
    {
    }

    public partial class Int32IResult : Result<Int32>
    {
    }

    public partial class StringResult : Result<string>
    {
    }

    public partial class StringIResult : Result<string>
    {
    }

    public partial class StringListIResult : Result<List<string>>
    {
    }

    public partial class LogisticTaskDTO2IResult : Result<LogisticTaskDTO2>
    {
    }

    public partial class EmployeeDTOListIResult : Result<List<EmployeeDTO>>
    {
    }

    public partial class MaterialDTOIResult : Result<MaterialDTO>
    {
    }

    public partial class LogisticTaskLabelDTO2IResult : Result<LogisticTaskLabelDTO2>
    {
    }

    public partial class MaterialDTOListIResult : Result<List<MaterialDTO>>
    {
    }

    public partial class LogisticTaskDTO2ListIResult : Result<List<LogisticTaskDTO2>>
    {
    }

    public partial class LogisticTaskLabelDTOListResult : Result<List<LogisticTaskLabelDTO>>
    {
    }

    public partial class LogisticParametresDTOListResult : List<Result<List<LogisticParametresDTO>>>
    {
    }

    public partial class StockAnomalyDTOListIResult : Result<List<StockAnomalyDTO>>
    {
    }

    public partial class StockAnomalyDTOIResult : Result<StockAnomalyDTO>
    {
    }

    public partial class StockAnomalyDTOListResult : Result<List<StockAnomalyDTO>>
    {
    }

    public partial class LogisticTaskLabelDTOIResult : Result<LogisticTaskLabelDTO>
    {
    }

    public partial class ScanningCodeDTOListResult : Result<List<ScanningCodeDTO>>
    {
    }

    public partial class LogisticTaskDTOListIResult : Result<List<LogisticTaskDTO>>
    {
    }

    public partial class LogisticTaskDTOIResult : Result<LogisticTaskDTO>
    {
    }

    public partial class LabelTemplateDTOListResult : Result<List<LabelTemplateDTO>>
    {
    }

    public partial class SearchProductDTOListIResult : Result<List<SearchProductDTO>>
    {
    }

    public partial class InventoryTaskDTOListIResult : Result<List<InventoryTaskDTO>>
    {
    }

    public partial class InventoryTaskDTOIResult : Result<InventoryTaskDTO>
    {
    }

    public partial class StockOverViewDTOListIResult : Result<List<StockOverViewDTO>>
    {
    }

    public partial class StockAnomalyDTOListIResult : Result<List<StockAnomalyDTO>>
    {
    }

    public partial class LogisticAreaDTOIResult : Result<LogisticAreaDTO>
    {
    }

    public partial class LogisticAreaDTOListIResult : Result<List<LogisticAreaDTO>>
    {
    }

    public partial class SiteDTOListIResult : Result<List<SiteDTO>>
    {
    }
}