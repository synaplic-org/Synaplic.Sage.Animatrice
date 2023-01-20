using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Transfer.DataModel;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Uni.Scan.Client.Pages.Stock
{
    public partial class StockAnomalyMobile
    {


        [Inject] private IStockAnomalyClient _stockAnomalyClient { get; set; }
        [Parameter] public StockAnomalyDTO anomalyDTO { get; set; } = new();
        [Parameter] public int Id { get; set; }

        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";
        private List<StockAnomalyDTO> Elements = new();
        private List<StockAnomalyDTO> FilterElements = new();

        private StockAnomalyDTO _anomalyDTO = new();
        private MudIconButton returnButton;

        private ClaimsPrincipal _currentUser;
        private bool _canViewAnomalies;
        private bool _canCreateAnomalies;
        private bool _canEditAnomalies;
        private bool _canDeleteAnomalies;
        private bool _canSearchAnomalies;
        private bool _canValidateAnomalies;
        private bool _canCancelAnomalies;
        private bool _canRejectAnomalies;

        public int AnomaliesType { get; set; } = 1;
        public string AnomalyStat { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            //_global.CurrentTitle = Localizer["STOCK ANOMALIES"];
        }

        protected override async Task OnInitializedAsync()
        {
            _global.CurrentTitle = Localizer["STOCK ANOMALIES"];
            _currentUser = await _authenticationManager.CurrentUser();
            _canViewAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.View)).Succeeded;
            _canCreateAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.Create)).Succeeded;
            _canEditAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.Edit)).Succeeded;
            _canDeleteAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.Delete)).Succeeded;
            _canSearchAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.Search)).Succeeded;
            _canValidateAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.Validate)).Succeeded;
            _canRejectAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.Reject)).Succeeded;
            _canCancelAnomalies = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.StockAnomalies.Cancel)).Succeeded;
            await GetAnomalies();
        }

        private async Task GetAnomalies()
        {
            SetLoading(true);
            try
            {
                var response = await _stockAnomalyClient.GetAllAsync();
                if (response.Succeeded)
                {
                    Elements = response.Data.ToList();
                    FilterElements = Elements;
                }
                else
                {
                    await _dialogService.ShowErrors(response.Messages);
                }
            }
            catch (HttpRequestException ex)
            {
                _snackBar.Add(Localizer["Vous êtes hors ligne !"], Severity.Warning);
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrors(e);
            }

            SetLoading(false);
        }

     

        private async Task Delete(int id)
        {
            string deleteContent = Localizer["Supprimer "];
            var parameters = new DialogParameters
            {
                { nameof(Client.Shared.Dialogs.DeleteConfirmationDialog.ContentText), string.Format(deleteContent, id) }
            };
            var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog =
                _dialogService.Show<Client.Shared.Dialogs.DeleteConfirmationDialog>(Localizer["Supprimer"], parameters,
                    options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await _stockAnomalyClient.DeleteAsync(id);
                if (response.Succeeded)
                {
                    _snackBar.Add("Supprimé!", Severity.Success);
                    await GetAnomalies();
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }

                    await GetAnomalies();
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                _anomalyDTO = Elements.FirstOrDefault(c => c.Id == id);
                if (_anomalyDTO != null)
                {
                    parameters.Add(nameof(StockAnomalyDialog.stockanomalyDTO), new StockAnomalyDTO
                    {
                        Id = _anomalyDTO.Id,
                        ProductID = _anomalyDTO.ProductID,
                        OwnerPartyID = _anomalyDTO.OwnerPartyID,
                        ProductDescription = _anomalyDTO.ProductDescription,
                        SiteID = _anomalyDTO.SiteID,
                        IdentifiedStockID = _anomalyDTO.IdentifiedStockID,
                        Quantity = _anomalyDTO.Quantity,
                        QuantityUniteCode = _anomalyDTO.QuantityUniteCode,
                        CompanyID = _anomalyDTO.CompanyID,
                        CorrectedQuantity = _anomalyDTO.CorrectedQuantity,
                        CorrectedIdentifiedStockID = _anomalyDTO.CorrectedIdentifiedStockID,
                        DeclaredBy = _anomalyDTO.DeclaredBy,
                        AnomalyReason = _anomalyDTO.AnomalyReason,
                        AnomalyStatus = _anomalyDTO.AnomalyStatus,
                        AnomalyType = _anomalyDTO.AnomalyType,
                        ClosedBy = _anomalyDTO.ClosedBy,
                        CloseOn = _anomalyDTO.CloseOn,
                        InventoryRestrictedUseIndicator = _anomalyDTO.InventoryRestrictedUseIndicator,
                        InventoryStockStatusCode = _anomalyDTO.InventoryStockStatusCode,
                        IdentifiedStockType = _anomalyDTO.IdentifiedStockType,
                        IdentifiedStockTypeCode = _anomalyDTO.IdentifiedStockTypeCode,
                        LogisticsArea = _anomalyDTO.LogisticsArea,
                        LogisticsAreaID = _anomalyDTO.LogisticsAreaID,
                    });
                }
            }
        
          
                var options = new DialogOptions
                { FullScreen = true, FullWidth = true, DisableBackdropClick = true };

                var dialog = _dialogService.Show<StockAnomalyDialog>(id == 0 ? Localizer["Créer"] : Localizer["Modifier"], parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    await Reset();
                }
            
        }

        private async Task Reset()
        {
            _anomalyDTO = new StockAnomalyDTO();
            await GetAnomalies();
        }

       

        private async Task BackToParent()
        {
            SetLoading(true);
            _navigationManager.NavigateTo($"/Home");
            await Task.Delay(1);
            SetLoading(false);
        }

        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
            StateHasChanged();
        }

    }
}
