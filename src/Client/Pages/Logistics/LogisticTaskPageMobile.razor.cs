using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Transfer.DataModel.LogisticTask;

namespace Uni.Scan.Client.Pages.Logistics
{
    public partial class LogisticTaskPageMobile
    {
        [CascadingParameter] public App MyApp { get; set; }
        [Inject] private ILogisticTaskClient _LogisticTaskClient { get; set; }

        [Parameter] public string ProcessType { get; set; }

        private ClaimsPrincipal _currentUser;

        //private bool enabled = true;
        //private bool _loading;
        private string _searchString = "";
        private List<LogisticTaskDTO2> _mytaskList = new();
        private List<LogisticTaskDTO2> _unassignedtaskList = new();
        private string code = "";
        MudTabs tabs;
        MudTextField<string> scaninput;
        private MudIconButton returnButton;
        private IndexedDbManager _indexDB;
        private bool _offline;

        protected override async Task OnInitializedAsync()
        {
            _indexDB = await _dbFactory.GetDbManager(_global.IndexDbName);
            await AddScannerEventListener();
            await LoadData();
        }

        private async Task AddScannerEventListener()
        {
            var dotNetObjectReference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("addScannerEventListener", dotNetObjectReference);
        }

        [JSInvokable]
        public async Task OnScanned(string barcode)
        {
            var tsk = _unassignedtaskList.FirstOrDefault(o =>
                o.Id.Equals(barcode, StringComparison.OrdinalIgnoreCase));
            if (tsk != null)
            {
                await TakeTaskAsync(tsk);
            }
            else
            {
                tsk = _mytaskList.FirstOrDefault(o => o.Id.Equals(barcode, StringComparison.OrdinalIgnoreCase));
                if (tsk != null)
                {
                    OpenTask(tsk);
                }
                else
                {
                    _snackBar.Add(string.Format(Localizer["La tache {0} est introuvable!"], barcode), Severity.Warning);
                }
            }

            StateHasChanged();
        }

        private void OpenTask(LogisticTaskDTO2 dto)
        {
            _navigationManager.NavigateTo($"/Mobile/Logistics/{dto.ProcessType}/Details/{dto.ObjectID}");
        }

        private async Task LoadData()
        {
            SetLoading(true);
            _searchString = String.Empty;
            try
            {
                _mytaskList = new();
                _unassignedtaskList = new();
                var myresponse = await _LogisticTaskClient.GetAllowedTasksAsync(ProcessType);
                if (myresponse.Succeeded)
                {
                    _mytaskList = myresponse.Data.Where(o => !string.IsNullOrWhiteSpace(o.ResponsibleId)).ToList();
                    _unassignedtaskList =
                        myresponse.Data.Where(o => string.IsNullOrWhiteSpace(o.ResponsibleId)).ToList();
                    await UpdateLocalData();
                }
                else
                {
                    await _dialogService.ShowErrors(myresponse.Messages);
                }

                _offline = false;
            }
            catch (HttpRequestException ex)
            {
                await LoadLocalData();
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrors(e);
            }


            if (scaninput != null)
            {
                scaninput.Reset();
            }

            SetLoading(false);
        }

        private async Task LoadLocalData()
        {
            try
            {
                _snackBar.Add(Localizer["Vous êtes hors ligne !"], Severity.Warning);
                _mytaskList = (await _indexDB.ToArray<LogisticTaskDTO2>(nameof(LogisticTaskDTO2))).Where(o => o.ProcessType == ProcessType).ToList();
                _offline = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task UpdateLocalData()
        {
            try
            {
                var lst = (await _indexDB.ToArray<LogisticTaskDTO2>(nameof(LogisticTaskDTO2))).ToList();
                foreach (LogisticTaskDTO2 item in lst)
                {
                    if (_mytaskList.All(o => o.Id != item.Id))
                    {
                        await _indexDB.DeleteRecord(nameof(LogisticTaskDTO2), item.ObjectID);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool Search(LogisticTaskDTO2 taskDTO)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (taskDTO.Id.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await FocusScanInput();
        }

        private async Task FocusScanInput()
        {
            if (scaninput != null && false)
            {
                await returnButton.FocusAsync();
            }
        }

        private async Task TakeTaskAsync(LogisticTaskDTO2 Task)
        {
            SetLoading(true);
            try
            {
                var response = await _LogisticTaskClient.SetTaskResponsibleAsync(Task.ObjectID);
                if (response.Succeeded)
                {
                    if (response.Data)
                    {
                        OpenTask(Task);
                    }
                    else
                    {
                        _snackBar.Add(Localizer["Erreur d'affectation"], Severity.Error);
                    }
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _snackBar.Add(Localizer["Un erreur est survenu !"], Severity.Error);
            }

            SetLoading(false);
        }

        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
            StateHasChanged();
        }

        private async Task BackToParent()
        {
            _navigationManager.NavigateTo($"/Home");
            await Task.Delay(1);
        }

        private async Task ScanBarCode()
        {
            // SetLoading(true);


            var parameters = new DialogParameters();

            var options = new DialogOptions { CloseButton = true, FullScreen = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<BarcodeScannerDialog>(Localizer["Scan"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await OnScanned(result.Data.ToString());
            }


            SetLoading(false);
        }

        private async Task Refresh()
        {
            SetLoading(true);


            await LoadData();


            SetLoading(false);
        }
    }
}