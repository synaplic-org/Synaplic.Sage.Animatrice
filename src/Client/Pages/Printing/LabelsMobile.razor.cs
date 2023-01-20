using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Shared.Enums;
using Uni.Scan.Transfer.DataModel;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Uni.Scan.Client.Pages.Identity;
using Uni.Scan.Client.Pages.Labels;

namespace Uni.Scan.Client.Pages.Printing
{
    public partial class LabelsMobile
    {
        [Inject] private ILogisticTaskLabelClient _logisticTaskLabelClient { get; set; }
        [Parameter] public LogisticTaskLabelDTO labelDTO { get; set; } = new();
        [Parameter] public int Id { get; set; }
        public int LabelsType { get; set; } = 1;
        public string ModalType { get; set; }

        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";
        private ClaimsPrincipal _currentUser;
        private bool _canCreateLabels;
        private bool _canEditLabels;
        private bool _canDeleteLabels;
        private bool _canPrintLabels;
        private bool _canSearchLabels;
        private List<LogisticTaskLabelDTO> Elements = new();
        private List<LogisticTaskLabelDTO> FilterElements = new();
        private bool _processing = false;
        private LogisticTaskLabelDTO _labelDTO = new();
        private MudIconButton returnButton;

        protected override async Task OnInitializedAsync()
        {
            _global.CurrentTitle = Localizer["ETIQUETTES"];
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Create)).Succeeded;
            _canEditLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Edit)).Succeeded;
            _canDeleteLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Delete)).Succeeded;
            _canSearchLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Search)).Succeeded;
            _canPrintLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Print)).Succeeded;
            await GetLabels();
        }


        async Task ProcessSomething()
        {
            _processing = true;
            await GetLabels();
            _processing = false;
        }

        private async Task GetLabels()
        {
            SetLoading(true);

            var response = await _logisticTaskLabelClient.GetAllAsync();
            if (response.Succeeded)
            {
                Elements = response.Data.ToList();
                FilterElements = Elements;
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

        private async Task SaveLabels()
        {
            var listelables = Elements.ToList();
            var result = await _logisticTaskLabelClient.SaveTaskLabelsAsync(listelables);
            if (result.Succeeded)
            {
                _snackBar.Add($"Enregistré", Severity.Success);
                await GetLabels();
            }
            else
            {
                _snackBar.Add($"une erreur est survenue ", Severity.Error);
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = Localizer["Supprimer Étiquette"];
            var parameters = new DialogParameters
            {
                { nameof(Client.Shared.Dialogs.DeleteConfirmationDialog.ContentText), string.Format(deleteContent, id) }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Client.Shared.Dialogs.DeleteConfirmationDialog>(Localizer["Supprimer"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await _logisticTaskLabelClient.DeleteAsync(id);
                if (response.Succeeded)
                {
                    _snackBar.Add("Étiquette supprimé!", Severity.Success);
                    await GetLabels();
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }

                    await GetLabels();
                }
            }
        }


        private async Task InvokeAddLabelModal(int id = 0)
        {
            await LogisticLabelDialog.ShowDialogAsync(_dialogService, _global.IsMobileView);
            return;

            var parameters = new DialogParameters();
            if (id != 0)
            {
                _labelDTO = Elements.FirstOrDefault(c => c.Id == id);
                if (_labelDTO != null)
                {
                    parameters.Add(nameof(AddLabelModal.taskLabelDTO), new LogisticTaskLabelDTO
                    {
                        Id = _labelDTO.Id,
                        ProductId = _labelDTO.ProductId,
                        ProductName = _labelDTO.ProductName,
                        Type = LabelType.Free,
                        LineItemID = _labelDTO.LineItemID,
                        IdentifiedStock = _labelDTO.IdentifiedStock,
                        LabelUuid = _labelDTO.LabelUuid,
                        ProductionDate = _labelDTO.ProductionDate,
                        ExpirationDate = _labelDTO.ExpirationDate,
                        PlanQuantity = _labelDTO.PlanQuantity,
                        Duplicata = _labelDTO.Duplicata,
                        FabricationOrdre = _labelDTO.FabricationOrdre,
                        Tare = _labelDTO.Tare,
                        ProductionOrdre = _labelDTO.ProductionOrdre,
                        PackageID = _labelDTO.PackageID,
                        QuatityOnLabel = _labelDTO.QuatityOnLabel,
                        NbrEtiquettes = _labelDTO.NbrEtiquettes,
                        ExternalID = _labelDTO.ExternalID,
                        Status = _labelDTO.Status,
                        Title = _labelDTO.Title,
                        Comment = _labelDTO.Comment,
                        SerialStock = _labelDTO.SerialStock,
                        SupplierIdentifiedStock = _labelDTO.SupplierIdentifiedStock,
                        TransferOrdre = _labelDTO.TransferOrdre,
                        ProductSpecification = _labelDTO.ProductSpecification,
                        GTIN = _labelDTO.GTIN,
                        TaskLineId = _labelDTO.TaskLineId,
                    });
                }
            }


            var options = new DialogOptions { FullScreen = true, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddLabelModal>(id == 0 ? Localizer["Créer"] : Localizer["Modifier"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _labelDTO = new LogisticTaskLabelDTO();
            await GetLabels();
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