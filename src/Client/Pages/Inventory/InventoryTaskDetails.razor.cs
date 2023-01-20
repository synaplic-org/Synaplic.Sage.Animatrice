using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDB;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.Managers.Logistics;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Transfer.DataModel;


namespace Uni.Scan.Client.Pages.Inventory;

public partial class InventoryTaskDetails
{
    [Parameter] public string TaskId { get; set; }
    [Parameter] public string OperationId { get; set; }


    [Inject] private ILogisticsManager logisticsManager { get; set; }
    public MudNumericField<decimal> CountedQuantityField { get; set; }


    private bool _showNavigationArrows = true;
    private int _lineCount = 0;
    private int _currentIndex = 0;
    private bool _showAddNewLine;


    private MudTextField<string> barcodeInputText;


    private IndexedDbManager indexDB;
    private InventoryTaskDTO _inventoryTask = new();
    private InventoryTaskOperationDTO _inventoryOperation = new();
    private List<InventoryTaskItemDTO> _InventoryDetails = new();
    private InventoryTaskItemDTO _current = new();


    protected override async Task OnInitializedAsync()
    {
        indexDB = await _dbFactory.GetDbManager(_global.IndexDbName);
        await AddScannerEventListener();
        SetLoading(false);
        await LoadData();
    }

    private DotNetObjectReference<InventoryTaskDetails> dotNetObjectReference;
    private MudButton _pageNulberButton;

    private async Task AddScannerEventListener()
    {
        if (dotNetObjectReference == null)
        {
            dotNetObjectReference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("addScannerEventListener", dotNetObjectReference);
            Console.WriteLine("Scanner Event Listener added! ");
        }
    }

    [JSInvokable]
    public void OnScanned(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode) || _InventoryDetails.IsNullOrEmpty()) return;

        var itm = _InventoryDetails.FirstOrDefault(o =>
            o.ProductID.Equals(barcode, StringComparison.OrdinalIgnoreCase));
        if (itm != null)
        {
            itm = _InventoryDetails.FirstOrDefault(o =>
                o.IdentifiedStockID.Equals(barcode, StringComparison.OrdinalIgnoreCase));
        }

        if (itm != null)
        {
            MoveTo(itm);
        }
    }

    private async Task LoadData()
    {
        SetLoading(true);

        try
        {
            InventoryTaskDTO item = null;


            item = await indexDB.GetRecordByIdAsync<string, InventoryTaskDTO>(nameof(InventoryTaskDTO), TaskId);


            if (item == null)
            {
                throw new ApplicationException(Localizer["Tache non trouvée"]);
            }
            else
            {
                _inventoryTask = item;
                _inventoryOperation = item.InventoryTaskOperations.FirstOrDefault(o => o.ObjectID == OperationId);
                if (_inventoryOperation != null)
                {
                    _InventoryDetails = _inventoryOperation.InventoryTaskItems?.ToList();
                    if (!_InventoryDetails.IsNullOrEmpty())
                    {
                        MoveTo(_InventoryDetails.FirstOrDefault());
                    }
                }
                else
                {
                    throw new ApplicationException(Localizer["Opération non trouvée"]);
                }
            }
        }
        catch (Exception e)
        {
            await _dialogService.ShowErrors(e);
            SetLoading(false);
            await BackToParent();
        }


        SetLoading(false);
    }

    private void MoveTo(InventoryTaskItemDTO obj)
    {
        if (!_InventoryDetails.IsNullOrEmpty())
        {
            _current = obj;
            _lineCount = _InventoryDetails.Count;
            _showNavigationArrows = (_lineCount > 1);
            _currentIndex = _InventoryDetails.IndexOf(_current);
        }
        else
        {
            _lineCount = _currentIndex = 0;
            _showNavigationArrows = false;
        }

        StateHasChanged();
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

    private async Task MoveStep(int i)
    {
        if (i > 0 && _currentIndex + i < _lineCount)
            _currentIndex++;
        else if (i < 0 && _currentIndex + i >= 0)
            _currentIndex--;
        _current = _InventoryDetails[_currentIndex];
        if (_pageNulberButton != null)
        {
            await _pageNulberButton.FocusAsync();
        }

        StateHasChanged();
    }


    private async Task BackToParent()
    {
        SetLoading(true);
        if (_inventoryTask != null)
        {
            try
            {
                await indexDB.PutRecord(new StoreRecord<InventoryTaskDTO>()
                {
                    StoreName = nameof(InventoryTaskDTO),
                    Record = _inventoryTask
                });
            }
            catch (Exception e)
            {
                _snackBar.Add(Localizer["Une erreur de sauvegarde local est survenue !"], Severity.Error);
                _snackBar.Add(Localizer["Essayer plutôt d'envoyer les données à SAP  !"], Severity.Error);
            }
        }

        _navigationManager.NavigateTo($"/Inventory/Operation/{TaskId}");
    }

    private void SetLoading(bool isloading)
    {
        _global.IsLoading = isloading;
        StateHasChanged();
    }


    private async Task ResetDefaultValues()
    {
        SetLoading(true);


        string confirmText = Localizer["Are you sure you want to reset this task?"];
        string buttonText = Localizer["Rest task"];
        var parameters = new DialogParameters();
        parameters.Add("ContentText", confirmText);
        parameters.Add("ButtonText", buttonText);
        parameters.Add("Color", Color.Warning);

        var dialog = _dialogService.Show<ConfirmationDialog>(Localizer["Confirm"], parameters);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
        }


        SetLoading(false);
    }


    private async Task SplitValues()
    {
        SetLoading(true);


        await Task.Delay(1);


        SetLoading(false);
    }

    private async Task ScanBarCode()
    {
        SetLoading(true);


        var parameters = new DialogParameters();

        var options = new DialogOptions { CloseButton = true, FullScreen = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<BarcodeScannerDialog>(Localizer["Scan"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Console.WriteLine("ScanBarCode " + result.Data.ToString());
            OnScanned(result.Data.ToString());
        }


        SetLoading(false);
    }


    private async Task SetDeviation()
    {
        SetLoading(true);
        await Task.Delay(1);
        SetLoading(false);
    }


    private void FilleQuantite(LogisticTaskDetailDTO line)
    {
        if (line.Disabled) return;

        SetLoading(true);


        line.ConfirmQuantity = line.OpenQuantity;


        SetLoading(false);
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            // await InitializeBarcodeScanner();
        }
    }
    //private async Task InitializeBarcodeScanner()
    //{
    //    var dotNetObjectReference = DotNetObjectReference.Create(this); 
    //    await _jsRuntime.InvokeVoidAsync("InitBarcodeScannerEvent", dotNetObjectReference);

    //}

    //[JSInvokable]
    //public async Task OnScanned(string barcode)
    //{
    //    _snackBar.Add($"code :[{barcode.ToString()}]");
    //     MoveTo(barcode);
    //     await Task.Delay(1);
    //}


    //private void ValueInputChanged(string s)
    //{
    //	MoveTo(s);
    //	barcodeInputText.Clear();
    //}

    private async Task FocusScanInput()
    {
        if (barcodeInputText != null)
        {
            await barcodeInputText.FocusAsync();
        }
    }

    private void QuatiteKeyDown(KeyboardEventArgs obj)
    {
        if (obj.Key == ".")
        {
            obj.Key = "?";
        }
    }

    private async Task SelectQuantityAsync()
    {
        await CountedQuantityField.SelectAsync();
    }

    private void AddNewItem()
    {
        var item = new InventoryTaskItemDTO()
        {
            ProductID = " test",
            CountedQuantityUnitCode = "KG",
            IdentifiedStockID = "WAAAAA3"
        };

        if (!_inventoryOperation.InventoryTaskItems.Any(o =>
                o.ProductID.Equals(item.ProductID, StringComparison.OrdinalIgnoreCase) &&
                o.IdentifiedStockID.Equals(item.IdentifiedStockID, StringComparison.OrdinalIgnoreCase)))
        {
            _inventoryOperation.InventoryTaskItems.Add(item);
            _InventoryDetails = _inventoryOperation.InventoryTaskItems;
            MoveTo(item);
        }
        else
        {
            _snackBar.Add(Localizer["Cet article existe déjà !"]);
        }
    }
}