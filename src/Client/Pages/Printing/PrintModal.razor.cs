using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Protocol;
using MudBlazor;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests.Label;
using Uni.Scan.Transfer.Requests.LogisticArea;
using Uni.Scan.Transfer.Requests.Task;

namespace Uni.Scan.Client.Pages.Printing
{
    public partial class PrintModal

    {
        [Inject] private ILogisticsClient logisticsClient { get; set; }
        [Inject] private ILabelTemplateClient _labelTemplateClient { get; set; }
        [Inject] private ILogisticAreaClient _areaClient { get; set; }
        [Inject] private ILogisticTaskLabelClient _logisticTaskLabelClient { get; set; }
        [Parameter] public string TaskId { get; set; }
        [Parameter] public string ProductId { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public int intID { get; set; }
        [Parameter] public LabelTemplateDTO labelTemplateDTO { get; set; } = new();
        [Parameter] public string ModalType { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        private List<LabelTemplateDTO> _labelTemplateDTO { get; set; } = new();
        [Parameter] public List<LogisticTaskLabelDTO> _logisticTaskLabelDTO { get; set; } = new();

        private LogisticTaskDTO _task = new();
        private List<LogisticTaskDetailDTO> _taskDetails = new();
        public HashSet<LogisticTaskLabelDTO> _selectedLabels { get; set; } = new();
        private HashSet<LogisticTaskDetailDTO> _selectedItems = new();
        private List<LogisticAreaDTO> Elements = new();
        private LogisticAreaDTO _area = new();
        private string _searchString = "";  
        private bool _processing = false;
        public bool fixed_header = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await GetParametres();

            if (ModalType.Equals("Task"))
            {
            await GetTaskDetails();
            }

            if (ModalType.Equals("Label") || ModalType.Equals("LabelSection"))
            {
                await GetLabels();
            }

            if (ModalType.Equals("Area"))
            {
                await GetAreas();
            }

            if  (ModalType.Equals("SingleLabel"))
            {               
                await GetAllLabels();
                if (intID != 0)
                {
                  var SingleLabel = _logisticTaskLabelDTO.FirstOrDefault(x => x.Id == intID);
                  _logisticTaskLabelDTO = new();
                  _logisticTaskLabelDTO.Add(SingleLabel);
                }
            }
        }

        async Task ProcessSomething()
        {
            if (ModalType.Equals("Task"))
            {
                _processing = true;
                await GetTaskPDF();
                _processing = false;
            }
            else if (ModalType.Equals("Label") || ModalType.Equals("LabelSection") || ModalType.Equals("SingleLabel"))
            {
                _processing = true;
                await GetLabelPDF();
                _processing = false;
            }
            else if (ModalType.Equals("Area"))
            {
                _processing = true;
                await GetAreaPDF();
                _processing = false;
            }
        }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        private async Task GetLabels()
        {
            SetLoading(true);

            var response = await _logisticTaskLabelClient.GetAllTaskLabelAsync(Id);
            if (response.Succeeded)
            {
                _logisticTaskLabelDTO = response.Data.ToList();
                if (ProductId != null)
                {
                    _logisticTaskLabelDTO = _logisticTaskLabelDTO.Where(x => x.ProductId == ProductId).ToList();

                }

            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            SetLoading(false);

        }
        private async Task GetAreas()
        {
            SetLoading(true);

            var response = await _areaClient.GetLogisticAreaAsync(Id);
            if (response.Succeeded)
            {
                //Elements.Add(response.Data);
                _area = response.Data;
                Elements.Add(_area);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            SetLoading(false);

        }
        private async Task GetAllLabels()
        {
            SetLoading(true);

            var response = await _logisticTaskLabelClient.GetAllAsync();
            if (response.Succeeded)
            {
                if (intID != 0)
                {
                    _logisticTaskLabelDTO = response.Data.ToList();
                }
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            SetLoading(false);
        }
        private async Task GetParametres()
        {
            var response = await _labelTemplateClient.GetAllAsync();
            if (response.Succeeded)
            {
                _labelTemplateDTO = response.Data.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

        }
        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
            StateHasChanged();
        }
        private async Task GetTaskDetails()
        {
            SetLoading(true);

            var response = await logisticsClient.GetTaskDetailsAsync(TaskId);
            if (response.Succeeded)
            {
                _task = response.Data;
                _taskDetails = _task.LogisticTaskDetails.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            SetLoading(false);

        }
        private bool AreaSearch(LogisticAreaDTO area)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (area.LogisticAreaID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return area.LogisticAreaID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }
        private bool LabelSearch(LogisticTaskLabelDTO lbl)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (lbl.ProductId?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (lbl.ProductName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return lbl.ProductId?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }
        private bool TaskSearch(LogisticTaskDetailDTO detail)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (detail.ProductID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (detail.ProductDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return detail.ProductID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }
        private async Task GetLabelPDF()
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://scan.sapbydesign.app/reports/");
            var request = new LabelPrintRequest();

            request.Responsible = _task.ResponsibleName;
            request.ModelName = labelTemplateDTO.ModelName.ToString();
            request.Lables = new List<Label>();
            foreach (var i in _selectedLabels)
            {
                request.Lables.Add(new Label()
                {
                    Title = i.Title,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    IdentifiedStock = i.IdentifiedStock,
                    QuatityOnLabel = i.QuatityOnLabel,
                    QuatityUnite = "KGM",
                    ExpirationDate = i.ExpirationDate.ToString(),
                    ProductionDate = i.ProductionDate.ToString(),
                    NbrEtiquettes = i.NbrEtiquettes,
                    Supplement = i.Supplement,
                    Tare = i.Tare,
                    Duplicata = i.Duplicata,
                });
            };
            var res = await _logisticTaskLabelClient.GenerateLabelsPdf64Async(request);
            if (res.Succeeded)
            {
                var parameters = new DialogParameters();
                var str = res.Data;
                Console.WriteLine(str);
                parameters.Add("PDF64String", str);
                var options = new DialogOptions
                {
                    CloseButton = true,
                    MaxWidth = MaxWidth.ExtraLarge,
                    FullWidth = true,
                    DisableBackdropClick = true,
                    FullScreen = true
                };
                var dialog = _dialogService.Show<PdfDialog>("", parameters, options);
                var result = await dialog.Result;
            }
        }
        private async Task GetAreaPDF()
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://scan.sapbydesign.app/reports/");
            var request = new Area();

            request.ModelName = labelTemplateDTO.ModelName;
            request.LogisticAreaID = _area.LogisticAreaID;
            var res = await _logisticTaskLabelClient.GenerateAreaLabelsPdf64Async(request);
            if (res.Succeeded)
            {
                var parameters = new DialogParameters();
                var str = res.Data;
                Console.WriteLine(str);
                parameters.Add("PDF64String", str);
                var options = new DialogOptions
                {
                    CloseButton = true,
                    MaxWidth = MaxWidth.ExtraLarge,
                    FullWidth = true,
                    DisableBackdropClick = true,
                    FullScreen = true
                };
                var dialog = _dialogService.Show<PdfDialog>("", parameters, options);
                var result = await dialog.Result;
            }
        }

        private async Task GetTaskPDF()
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://scan.sapbydesign.app/reports/");
            var request = new TaskPrintRequest();
            request.ModelName = labelTemplateDTO.ModelName.ToString();
            request.Id = _task.Id;
            request.ResponsibleName = _task.ResponsibleName;
            request.ProcessType = _task.ProcessType;
            request.OperationType = _task.OperationType;
            request.ItemsNumberValue = _task.ItemsNumberValue;
            request.SiteName = _task.SiteName;
            request.SiteId = _task.SiteId;
            request.Priority = _task.Priority;
            request.RequestId = _task.RequestId;
            request.TaskLineDetails = new List<TaskLine>();
            foreach (var i in _selectedItems)
            {
                request.TaskLineDetails.Add(new TaskLine()
                {
                    LineItemID = i.LineItemID,
                    ProductID = i.ProductID,
                    ProductDescription = i.ProductDescription,
                    IdentifiedStockID = i.IdentifiedStockID,
                    PlanQuantity = i.PlanQuantity,
                    PlanQuantityUnitCode = i.PlanQuantityUnitCode
                });
            }

            ;

            var res = await _logisticTaskLabelClient.GenerateTaskPdf64Async(request);
            if (res.Succeeded)
            {
                var parameters = new DialogParameters();
                var str = res.Data;
                Console.WriteLine(str);
                parameters.Add("PDF64String", str);
                var options = new DialogOptions
                {
                    CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true, DisableBackdropClick = true,
                    FullScreen = true
                };
                var dialog = _dialogService.Show<PdfDialog>("", parameters, options);
                var result = await dialog.Result;
            }
        }
    }
}