using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Infrastructure.Managers.Logistics;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests.Logistics;

namespace Uni.Scan.Client.Pages.Stock
{
    public partial class StockOverview
    {
        [Inject] private ILogisticAreaClient _LogisticAreaClient { get; set; }

        private List<SiteDTO> _sites = new();

        private ClaimsPrincipal _currentUser = new();

        private bool _isOpen = true;

        private StockOverViewComponenet _stockOverViewComponenet;
        private string _UserSiteId;

        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            _UserSiteId = state.User.GetSiteID();
            SiteId = state.User.GetSiteID();
            await LoadData();
            _global.IsLoading = false;
        }

        private async Task LoadData()
        {
            try
            {
                var response = await _LogisticAreaClient.GetSitesAsync();
                if (response.Succeeded)
                {
                    _sites = response.Data.ToList();
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


            //}
        }

        private void Return()
        {
            _navigationManager.NavigateTo("");
        }


        private string CardStyle(bool b)
        {
            if (b) return "margin: 5px; border-left: solid 3px #ff6700;";
            return "margin: 5px; border-left: solid 3px #25638d;";
        }

        public void ToggleOpen()
        {
            if (_isOpen)
                _isOpen = false;
            else
                _isOpen = true;
        }


        private async Task StockSelected(StockOverViewDTO searchProductDto)
        {
            var result = await StockMouvementDialog.MoveProduct(_dialogService, searchProductDto);
            if (result)
            {
                await Search();
            }
        }


        public string? SiteId { get; set; }
        public string? LogisticsAreaId { get; set; }
        public string? ProductId { get; set; }
        public string? IdentifiedStockId { get; set; }

        private async Task Search()
        {
            if (string.IsNullOrEmpty(SiteId))
            {
                _snackBar.Add(Localizer["Sélectionnez un Site!"], MudBlazor.Severity.Error);
                return;
            }

            if (string.IsNullOrEmpty(LogisticsAreaId) && string.IsNullOrEmpty(ProductId))
            {
                _snackBar.Add(Localizer["Veuillez remplir un filtre!"], MudBlazor.Severity.Error);
                return;
            }

            _isOpen = false;

            StateHasChanged();
            if (_stockOverViewComponenet != null)
            {
                await _stockOverViewComponenet.Search(SiteId, LogisticsAreaId, ProductId, IdentifiedStockId);
            }


            StateHasChanged();
        }
    }
}