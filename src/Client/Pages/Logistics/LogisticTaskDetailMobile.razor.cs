using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorDB;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Infrastructure.Managers.Logistics;
using Uni.Scan.Client.Infrastructure.Settings;
using Uni.Scan.Client.Pages.Stock;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.DataModel.LogisticTask;
using static MudBlazor.CategoryTypes;


namespace Uni.Scan.Client.Pages.Logistics;

public partial class LogisticTaskDetailMobile
{
    [Parameter] public string TaskObjectID { get; set; }
    [Parameter] public string ProcessType { get; set; }

    [Inject] private ILogisticTaskClient _LogisticTaskClient { get; set; }


    private IndexedDbManager _indexDB;
    private LogisticTaskDTO2 _currentTask;
    private LogisticTaskItemDTO2 _currentLine;
    private bool _showNavigationArrows;
    private int _taskLinesCount;


    protected override async Task OnInitializedAsync()
    {
        _indexDB = await _dbFactory.GetDbManager(_global.IndexDbName);
        await AddScannerEventListener();
        await LoadData();
    }

    #region sanner

    private async Task AddScannerEventListener()
    {
        var dotNetObjectReference = DotNetObjectReference.Create(this);
        await _jsRuntime.InvokeVoidAsync("addScannerEventListener", dotNetObjectReference);
    }


    private MudButton _pageNumberButton;

    [JSInvokable]
    public void OnScanned(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode)) return;

        var item = _currentTask?.Items?.FirstOrDefault(o =>
            o.ProductID.Equals(barcode, StringComparison.OrdinalIgnoreCase));
        if (item != null)
        {
            MoveToItem(item);
        }
        else
        {
            _snackBar.Add(string.Format(Localizer["L'article {0} est introuvable!"], barcode), Severity.Warning);
        }
    }

    #endregion


    private async Task LoadData(bool tryLocalFirst = true)
    {
        SetLoading(true);


        try
        {
            if (tryLocalFirst)
                _currentTask =
                    await _indexDB.GetRecordByIdAsync<string, LogisticTaskDTO2>(nameof(LogisticTaskDTO2), TaskObjectID);


            if (_currentTask == null)
            {
                var myresponse = await _LogisticTaskClient.GetTaskDetailAsync(TaskObjectID);
                if (myresponse.Succeeded)
                {
                    await SetCurrentTask(myresponse.Data);
                }
                else
                {
                    await _dialogService.ShowErrors(myresponse.Messages);
                }
            }
            else
            {
                if (_currentTask.Items.IsNullOrEmpty())
                {
                    throw new ApplicationException(Localizer["Tache vide"]);
                }


                if (_currentLine == null || _currentLine.LineItemID == 0)
                {
                    MoveToItem(_currentTask.Items.FirstOrDefault());
                }
            }
        }
        catch (Exception e)
        {
            await _dialogService.ShowErrors(e);
            SetLoading(false);
            await BackToParentAsync(false);
        }


        SetLoading(false);
    }


    private async Task SetCurrentTask(LogisticTaskDTO2 logisticTaskDto2)
    {
        _currentTask = logisticTaskDto2;
        if (_currentTask != null)
        {
            if (_currentTask.ProcessingStatusCode == "3")
            {
                await DeletedFromLocalDB();
                await BackToParentAsync(false);
            }
            else
            {
                await SaveInLocalDB();
                if (_currentLine != null && _currentLine.LineItemID > 0)
                {
                    MoveToItem(_currentTask.Items.FirstOrDefault(o => o.LineItemID == _currentLine.LineItemID));
                }
                else
                {
                    MoveToItem(_currentTask.Items.FirstOrDefault());
                }
            }
        }
        else
        {
            throw new ApplicationException(Localizer["Tache non chargée , réessayez!"]);
        }
    }

    private async Task<Guid?> SaveInLocalDB()
    {
        if (_currentTask != null)
        {
            return await _indexDB.PutRecord(new StoreRecord<LogisticTaskDTO2>()
            {
                StoreName = nameof(LogisticTaskDTO2),
                Record = _currentTask
            });
        }

        return null;
    }

    private void MoveToItem(LogisticTaskItemDTO2 objDto2)
    {
        _showNavigationArrows = (_currentTask.Items.Count > 1);
        _taskLinesCount = _currentTask.Items.Count;
        _currentLine = objDto2;
        ValidateCurrentLine();

        StateHasChanged();
    }

    private async Task MoveStep(int step)
    {
        if (_currentTask.Items.Any(o => o.LineItemID == (_currentLine.LineItemID + step)))
        {
            _currentLine = _currentTask.Items.FirstOrDefault(o => o.LineItemID == (_currentLine.LineItemID + step));
            ValidateCurrentLine();
        }

        if (_pageNumberButton != null)
        {
            await _pageNumberButton.FocusAsync();
        }
    }

    private void ValidateCurrentLine()
    {
        if (_currentLine != null && _currentLine.Details?.IsNullOrEmpty() == false)
        {
            _currentLine.Details.ForEach(ValidateItem);
        }
    }


    private async Task BackToParentAsync(bool savelocal = true)
    {
        SetLoading(true);
        try
        {
            if (savelocal)
            {
                await SaveInLocalDB();
            }
        }
        catch (Exception e)
        {
            _snackBar.Add(Localizer["Une erreur de sauvegarde local est survenue !"], Severity.Error);
            _snackBar.Add(Localizer["Essayer plutôt d'envoyer les données  SAP  !"], Severity.Error);
        }

        _navigationManager.NavigateTo($"/Mobile/Logistics/{ProcessType}");
    }


    private void SetLoading(bool val)
    {
        _global.IsLoading = val;
        StateHasChanged();
    }


    //private async Task ResetDefaultValues()
    //{
    //    SetLoading(true);


    //    string confirmText = Localizer["Are you sure you want to reset this task?"];
    //    string buttonText = Localizer["Rest task"];
    //    var parameters = new DialogParameters();
    //    parameters.Add("ContentText", confirmText);
    //    parameters.Add("ButtonText", buttonText);
    //    parameters.Add("Color", Color.Warning);

    //    var dialog = _dialogService.Show<ConfirmationDialog>(Localizer["Confirm"], parameters);
    //    var result = await dialog.Result;
    //    if (!result.Cancelled)
    //    {
    //    }


    //    SetLoading(false);
    //}


    //private async Task SplitValues()
    //{
    //    SetLoading(true);


    //    await Task.Delay(1);


    //    SetLoading(false);
    //}

    private async Task ScanBarCode()
    {
        SetLoading(true);


        var parameters = new DialogParameters();

        var options = new DialogOptions { CloseButton = true, FullScreen = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<BarcodeScannerDialog>(Localizer["Scan"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            OnScanned(result.Data.ToString());
        }


        SetLoading(false);
    }

    private async Task ResetTask()
    {
        var result = await ConfirmationDialog.ShowDialogAsync(_dialogService,
            Localizer["Réinitialisation"],
            Localizer["Etes-vous sûrs de vouloir annuler toutes vos modifications?"],
            Localizer["Réinitialiser"],
            Color.Error
        );

        if (result.Cancelled) return;

        try
        {
            await DeletedFromLocalDB();
            await LoadData(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _snackBar.Add("An error occured!", Severity.Error);
        }
    }

    private async Task DeletedFromLocalDB()
    {
        await _indexDB.DeleteRecord(nameof(LogisticTaskDTO2), TaskObjectID);
        _currentTask = null;
    }

    private async Task UnassignedTask()
    {
        SetLoading(true);
        string confirmText = Localizer[" Etes-vous sûrs de vouloir vous désaffecter cette tache?"];
        string buttonText = Localizer["Désaffecter"];
        var parameters = new DialogParameters();
        parameters.Add("ContentText", confirmText);
        parameters.Add("ButtonText", buttonText);
        parameters.Add("Color", Color.Warning);

        var dialog = _dialogService.Show<ConfirmationDialog>(Localizer["désaffecter"], parameters);
        var result = await dialog.Result;
        if (result.Cancelled) return;
        try
        {
            var response = await _LogisticTaskClient.RemoveTaskResponsibleAsync(TaskObjectID);
            if (response.Succeeded)
            {
                await DeletedFromLocalDB();
                SetLoading(false);
                await BackToParentAsync(false);
            }
            else
            {
                await _dialogService.ShowErrors(response.Messages);
            }
        }
        catch (Exception e)
        {
            await _dialogService.ShowErrors(e);
        }

        SetLoading(false);
    }

    private async Task SetDeviation()
    {
        SetLoading(true);


        await Task.Delay(1);


        SetLoading(false);
    }


    private void FilleQuantite(LogisticTaskItemDetailDTO2 obj)
    {
        if (obj.Disabled) return;

        SetLoading(true);

        obj.ConfirmQuantity = 0;
        obj.ConfirmQuantity = _currentLine.SumOpenQuantity - _currentLine.SumConfirmQuantity;


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


    private void ValidateItem(LogisticTaskItemDetailDTO2 line)
    {
        line.LogisticSourceAreaError = false;
        line.LogisticTargetAreaError = false;
        line.StockIDError = false;
        line.ConfirmQuantityError = false;
        if (line.ConfirmQuantity > 0)
        {
            line.LogisticSourceAreaError = !line.Disabled && CanEditSourceArea() &&
                                           string.IsNullOrWhiteSpace(line.SourceLogisticsAreaID);
            line.LogisticTargetAreaError = !line.Disabled && CanEditTargetArea() &&
                                           string.IsNullOrWhiteSpace(line.TargetLogisticsAreaID);
            line.StockIDError = !line.Disabled && _currentLine.IsBatchManaged && CaneEditStockID() &&
                                string.IsNullOrWhiteSpace(line.IdentifiedStockIDNew);
        }
        else if (line.IsNew)
        {
            line.ConfirmQuantityError = true;
        }


        StateHasChanged();
    }


    private void AddNewLine()
    {
        if (_currentLine != null && !string.IsNullOrWhiteSpace(_currentLine.ProductID))
        {
            var det = _currentLine.Details.FirstOrDefault(o => o.IsNew == false && o.OpenQuantity > 0);
            if (det != null) _currentLine.Details.Add(det.CreateNew());
        }
    }


    private async Task ShowLinesListe()
    {
        if (_currentLine.Errors.IsNullOrEmpty() == false)
        {
            await _dialogService.ShowErrors(_currentLine.Errors);
        }
    }

    private bool CanHaveDevialtion()
    {
        if (_currentLine != null && !string.IsNullOrWhiteSpace(_currentLine.ProductID))
        {
            if (_currentLine.TotalQuantity >= _currentLine.PlanQuantity)
            {
                _currentLine.DeviationFound = false;
            }

            return _currentLine.TotalQuantity < _currentLine.PlanQuantity;
        }


        return false;
    }

    private bool CanAddNewLine()
    {
        if (_currentLine != null)
        {
            if (_currentLine.Details.Any(o => o.ConfirmQuantity == 0 && (o.IsNew || o.OpenQuantity > 0)))
            {
                return false;
            }

            return CanHaveDevialtion() && CanSplitQuantite() && !_currentLine.DeviationFound;
        }

        return false;
    }

    private bool CanSplitQuantite()
    {
        if (_currentTask == null) return false;
        string[] codes = new[] { "11", "12", "13", "14", "21", "23", "30" };
        return codes.Contains(_currentTask.OperationTypeCode);
    }

    //"1"  => "Make",
    //"10" => "Supply",

    //"11" => "Put Away",
    //"12" => "Unload",
    //"13" => "Returns Put Away",
    //"14" => "Returns Unload",
    //"21" => "Pick",
    //"22" => "Load",
    //"23" => "Returns Pick",
    //"24" => "Returns Load",
    //"30" => "Replenish",
    //"31" => "Remove",

    //"8"  => "Check",

    private bool CanEditTargetArea()
    {
        if (_currentTask == null) return false;

        string[] codes = new[] { "11", "13", "14", "31" };
        return codes.Contains(_currentTask.OperationTypeCode);
    }

    private bool CanViewTargetArea()
    {
        if (_currentTask == null) return false;
        string[] codes = new[] { "12", "14", "21", "23", "30" };
        return codes.Contains(_currentTask.OperationTypeCode);
    }

    private bool CanEditSourceArea()
    {
        if (_currentTask == null) return false;
        string[] codes = new[] { "21", "23", "30" };
        return codes.Contains(_currentTask.OperationTypeCode);
    }


    private bool CanViewSourceArea()
    {
        if (_currentTask == null) return false;
        string[] codes = new[] { "11", "13", "22", "24", "31" };
        return codes.Contains(_currentTask.OperationTypeCode);
    }

    private bool CaneEditStockID()
    {
        if (_currentTask == null) return false;
        string[] codes = new[] { "12", "21", "23", "30" };
        return codes.Contains(_currentTask.OperationTypeCode);
    }

    private async Task ShowActions()
    {
        var result = await LogisticTaskDetailActionDialog.ShowDialogAsync(_dialogService);
        if (result.Cancelled) return;

        switch (result.Data.ToString())
        {
            case LogisticTaskDetailActionDialog.UnisignTask:
                await UnassignedTask();
                break;
            case LogisticTaskDetailActionDialog.ResetTask:
                await ResetTask();
                break;
            case LogisticTaskDetailActionDialog.SaveTask:
                await SaveTask();
                break;
        }
    }

    private async Task SelectStockID(LogisticTaskItemDetailDTO2 item)
    {
        var stk = await StockSelectDialog.GetStock(_dialogService, _currentTask.SiteId, item.ProductID,
            item.IdentifiedStockID);
        if (stk != null)
        {
            if (stk.InventoryRestrictedUseIndicator)
            {
                _snackBar.Add(Localizer["Le stock est restreint"], Severity.Warning);
            }
            else
            {
                item.IdentifiedStockID = stk.IdentifiedStockID;
                item.SourceLogisticsAreaID = stk.LogisticsAreaID;
            }


            StateHasChanged();
        }
    }

    private async Task SelectTargetArea(LogisticTaskItemDetailDTO2 item)
    {
        var stk = await StockSelectDialog.GetStock(_dialogService, _currentTask.SiteId, item.ProductID,
            item.IdentifiedStockID);
        if (stk != null)
        {
            // item.TargetLogisticsAreaID = stk.LogisticsAreaID;

            StateHasChanged();
        }
    }

    private async Task SaveTask()
    {
        if (_currentLine == null || _currentTask.Items.IsNullOrEmpty())
        {
            _snackBar.Add(Localizer["La tache en cours est vide"], Severity.Error);
            return;
        }

        var oitesms = _currentTask.Items.SelectMany(o => o.Details).ToList();
        if (!oitesms.Any(o => o.Disabled == false && o.ConfirmQuantity > 0))
        {
            _snackBar.Add(Localizer["Veuillez remplir au moins une quantité"], Severity.Error);
            return;
        }

        var erros = new List<string>();
        foreach (var item in oitesms)
        {
            ValidateItem(item);
            if (item.LogisticTargetAreaError)
                erros.Add(string.Format(Localizer["{0}-Zone cible vide"], item.LineItemID));
            if (item.LogisticSourceAreaError)
                erros.Add(string.Format(Localizer["{0}-Zone source vide"], item.LineItemID));
            if (item.StockIDError)
                erros.Add(string.Format(Localizer["{0}-ID Stock vide"], item.LineItemID));
            if (item.ConfirmQuantityError)
                erros.Add(string.Format(Localizer["{0}-Quantité nulle "], item.LineItemID));
        }

        if (erros.IsNullOrEmpty() == false)
        {
            await _dialogService.ShowErrors(erros);
            return;
        }

        var result = await ConfirmationDialog.ShowDialogAsync(_dialogService,
            Localizer["Enregistrement"],
            Localizer[" Etes-vous sûrs de vouloir envoyer toutes vos modifications vers SAP ?"],
            Localizer["Envoyer"],
            Color.Secondary
        );

        if (result.Cancelled) return;

        SetLoading(true);
        try
        {
            var myresponse = await _LogisticTaskClient.SaveTaskAsync(_currentTask);

            if (!myresponse.Succeeded)
            {
                await _dialogService.ShowErrors(myresponse.Messages);
                if (myresponse.Data is { ProcessingStatusCode: "?" })
                {
                    await DeletedFromLocalDB();
                    await BackToParentAsync(false);
                }
            }
            else
            {
                if (myresponse.Data == null || myresponse.Data.ProcessingStatusCode == "3")
                {
                    await DeletedFromLocalDB();
                    await BackToParentAsync(false);
                }
                else
                {
                    await SetCurrentTask(myresponse.Data);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            await _dialogService.ShowErrors(new ApplicationException(Localizer["Vous êtes hors ligne"]));
        }
        catch (Exception e)
        {
            await _dialogService.ShowErrors(e);
        }

        SetLoading(false);
    }

    private void DeleteItem(LogisticTaskItemDetailDTO2 item)
    {
        if (_currentLine != null && !string.IsNullOrWhiteSpace(_currentLine.ProductID))
        {
            _currentLine.Details.Remove(item);
            StateHasChanged();
        }
    }
}