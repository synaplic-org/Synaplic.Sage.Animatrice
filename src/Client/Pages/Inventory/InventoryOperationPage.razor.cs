using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;
using BlazorDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Infrastructure.Managers;
using Uni.Scan.Client.Infrastructure.Managers.Logistics;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Transfer.DataModel;


namespace Uni.Scan.Client.Pages.Inventory
{
    public partial class InventoryOperationPage
    {
        [CascadingParameter] public App MyApp { get; set; }
        [Inject] private IInventoryClient _InventoryClient { get; set; }

        [Parameter] public string ObjectId { get; set; }


        private ClaimsPrincipal _currentUser;
        private InventoryTaskDTO _inventoryTask;


        private string _searchString = "";

        private bool scannerIsClosed = true;
        private string code = "";

        MudTextField<string> scaninput;
        private MudIconButton returnButton;

        private List<InventoryTaskOperationDTO> _operationListe;
        private IndexedDbManager indexDB;
        private bool _CanCloseTask;
        private bool _CanResetTask;

        protected override async Task OnInitializedAsync()
        {
            await AddScannerEventListener();
            SetLoading(false);
            indexDB = await _dbFactory.GetDbManager("UniScan");
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
            var tsk = _operationListe.FirstOrDefault(o =>
                o.LogisticsAreaID.Equals(barcode, StringComparison.OrdinalIgnoreCase));
            if (tsk != null)
            {
                OpenOperationTask(tsk);
            }
            else
            {
                _snackBar.Add(string.Format(Localizer["La zone {0} est introuvable!"], barcode), Severity.Warning);
            }

            StateHasChanged();
        }

        private void OpenOperationTask(InventoryTaskOperationDTO dto)
        {
            _navigationManager.NavigateTo($"/Inventory/{_inventoryTask.ObjectID}/Details/{dto.ObjectID}");
        }

        private async Task LoadData()
        {
            SetLoading(true);
            _searchString = String.Empty;

            try
            {
                var item = await indexDB.GetRecordByIdAsync<string, InventoryTaskDTO>(nameof(InventoryTaskDTO),
                    ObjectId);


                if (item == null)
                {
                    var myresponse = await _InventoryClient.GetTaskDetailsAsync(ObjectId);
                    if (myresponse.Succeeded)
                    {
                        _inventoryTask = myresponse.Data;
                        if (_inventoryTask != null)
                        {
                            await indexDB.PutRecord(new StoreRecord<InventoryTaskDTO>()
                            {
                                StoreName = nameof(InventoryTaskDTO),
                                Record = _inventoryTask
                            });
                        }
                    }
                    else
                    {
                        foreach (var message in myresponse.Messages)
                        {
                            _snackBar.Add(message, Severity.Error);
                        }
                    }
                }
                else
                {
                    _inventoryTask = item;
                }

                _operationListe = _inventoryTask.InventoryTaskOperations?.ToList() ??
                                  new List<InventoryTaskOperationDTO>();
                _CanCloseTask = !_operationListe.IsNullOrEmpty() && _operationListe.All(o => o.Progress >= 100);

                _CanResetTask = !_operationListe.IsNullOrEmpty() && _operationListe.Any(o => o.Progress > 0);
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

        private bool Search(InventoryTaskOperationDTO taskDTO)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (taskDTO.LogisticsAreaID.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
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


        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
            StateHasChanged();
        }

        private async Task BackToParent()
        {
            SetLoading(true);

            _navigationManager.NavigateTo($"/Inventory");
            await Task.Delay(1);
            SetLoading(false);
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

        private void PrinLabels(InventoryTaskOperationDTO dto)
        {
            // _navigationManager.NavigateTo($"/Logistics/{ProcessType.In}/Labels/{dto.Id}");
        }

        private async Task<bool> Save()
        {
            SetLoading(true);


            try
            {
                var requestList = _inventoryTask.InventoryTaskOperations.SelectMany(o => o.InventoryTaskItems).ToList();

                var response = await _InventoryClient.UpdateInventoryTaskDetailsAsync(requestList);
                if (response.Succeeded)
                {
                    SetLoading(false);
                    return true;
                }
                else
                {
                    await ShowErrors(response.Messages);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _snackBar.Add("An error occured!", Severity.Error);
            }

            SetLoading(false);
            return false;
        }

        private async Task<DialogResult> ShowErrors(List<string> responseMessages)
        {
            string buttonText = Localizer["Rest task"];
            var parameters = new DialogParameters();
            parameters.Add("Errors", responseMessages);


            var dialog = _dialogService.Show<ErrorDialog>(Localizer["Errors"], parameters);
            var result = await dialog.Result;
            return result;
        }

        private async Task FinishAndClose()
        {
            string confirmText = Localizer[" Etes-vous sûrs de vouloir clôturer cette tache?"];
            string buttonText = Localizer["Cloturer"];
            var parameters = new DialogParameters();
            parameters.Add("ContentText", confirmText);
            parameters.Add("ButtonText", buttonText);
            parameters.Add("Color", Color.Dark);

            var dialog = _dialogService.Show<ConfirmationDialog>(Localizer["Clôture"], parameters);
            var result = await dialog.Result;
            if (result.Cancelled) return;


            var ssaved = await Save();
            if (ssaved)
            {
                try
                {
                    var response = await _InventoryClient.FinishInventoryTaskAsync(_inventoryTask.ObjectID);
                    if (response.Succeeded)
                    {
                        await indexDB.DeleteRecord(nameof(InventoryTaskDTO), _inventoryTask.ObjectID);
                        BackToParent();
                    }
                    else
                    {
                        await ShowErrors(response.Messages);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _snackBar.Add("An error occured!", Severity.Error);
                }
            }
        }

        private async Task ResetTask()
        {
            string confirmText = Localizer[" Etes-vous sûrs de vouloir abandonner toutes vos modifications?"];
            string buttonText = Localizer["Reset"];
            var parameters = new DialogParameters();
            parameters.Add("ContentText", confirmText);
            parameters.Add("ButtonText", buttonText);
            parameters.Add("Color", Color.Error);

            var dialog = _dialogService.Show<ConfirmationDialog>(Localizer["Reset"], parameters);
            var result = await dialog.Result;
            if (result.Cancelled) return;

            try
            {
                await indexDB.DeleteRecord(nameof(InventoryTaskDTO), _inventoryTask.ObjectID);
                await LoadData();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _snackBar.Add("An error occured!", Severity.Error);
            }
        }

        private async Task UnisignTask()
        {
            string confirmText = Localizer[" Etes-vous sûrs de vouloir vous désaffecter cette tache?"];
            string buttonText = Localizer["Désaffecter"];
            var parameters = new DialogParameters();
            parameters.Add("ContentText", confirmText);
            parameters.Add("ButtonText", buttonText);
            parameters.Add("Color", Color.Error);

            var dialog = _dialogService.Show<ConfirmationDialog>(Localizer["désaffecter"], parameters);
            var result = await dialog.Result;
            if (result.Cancelled) return;

            try
            {
                var response = await _InventoryClient.RemoveTaskResponsibleAsync(_inventoryTask);
                if (response.Succeeded)
                {
                    await indexDB.DeleteRecord(nameof(InventoryTaskDTO), _inventoryTask.ObjectID);
                    BackToParent();
                }
                else
                {
                    await ShowErrors(response.Messages);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _snackBar.Add("An error occured!", Severity.Error);
            }
        }
    }
}