using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Client.Pages.Printing
{
    public partial class LogisticAreaPage
    {
        [Inject] private ILogisticAreaClient _areaClient { get; set; }
        private List<LogisticAreaDTO> Elements = new();

        private string searchString = "";
        private MudIconButton returnButton;
        private ClaimsPrincipal _currentUser;
        private bool _canViewLogisticArea;
        private bool _canSearchLogisticArea;
        private bool _canPrintLogisticArea;
        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        public string ModalType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _global.CurrentTitle = Localizer["ZONES LOGISTIQUES"];
            _currentUser = await _authenticationManager.CurrentUser();
            _canViewLogisticArea = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.LogisticArea.View)).Succeeded;
            _canSearchLogisticArea = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.LogisticArea.Search)).Succeeded;
            _canPrintLogisticArea = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.LogisticArea.Print)).Succeeded;
            await GetAreas();
        }

        private async Task GetAreas()
        {
            SetLoading(true);

            var response = await _areaClient.GetAllAreasAsync();
            if (response.Succeeded)
            {
                Elements = response.Data.ToList();
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

        private async Task Reset()
        {
            await GetAreas();
        }

        private bool Search(LogisticAreaDTO area)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return true;
            if (area.LogisticAreaID?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return area.LogisticAreaID?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true;
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

        private async Task PrintAreaLabel(string id)
        {
            var parameters = new DialogParameters();
            ModalType = "Area";
            parameters.Add(nameof(PrintModal.ModalType), ModalType);
            parameters.Add(nameof(PrintModal.Id), id);
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<PrintModal>(Localizer["Imprimer"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }
    }
}