using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Pages.Printing;
using Uni.Scan.Shared.Constants.Application;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Client.Pages.Settings
{
    public partial class TaskParametres
    {
        [Inject] private ILabelTemplateClient _labelTemplateClient { get; set; }
        [Inject] private IScanningCodeClient _scanningCodeClient { get; set; }
        [Inject] private ILogisticParametreClient _parametreClient { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        public List<ScanningCodeDTO> _scanningCodeDTO = new();
        public ScanningCodeDTO _scanningCode = new();
        private List<LabelTemplateDTO> _labelTemplateDTO = new();
        private LabelTemplateDTO _labelTemplate = new();

        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = false;
        private ClaimsPrincipal _currentUser;
        private bool _canCreateParametres;
        private bool _canEditParametres;
        private bool _canDeleteParametres;
        private bool _canSearchParametres;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _global.CurrentTitle = Localizer["PARAMETRES"];
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateParametres =
                (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Parametres.Create)).Succeeded;
            _canEditParametres = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Parametres.Edit))
                .Succeeded;
            _canDeleteParametres =
                (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Parametres.Delete)).Succeeded;
            _canSearchParametres =
                (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Parametres.Search)).Succeeded;
            _loaded = true;
            await GetParametresAsync();

            _scanningCodeDTO.Add(new ScanningCodeDTO { BarCodeType = "Global trade item number" });
            _scanningCodeDTO.Add(new ScanningCodeDTO { BarCodeType = "Identified stock" });
            _scanningCodeDTO.Add(new ScanningCodeDTO { BarCodeType = "Quantity" });
            _scanningCodeDTO.Add(new ScanningCodeDTO { BarCodeType = "Product ID" });
            _scanningCodeDTO.Add(new ScanningCodeDTO { BarCodeType = "Logistics Area" });
            _scanningCodeDTO.Add(new ScanningCodeDTO { BarCodeType = "Unit of measure" });
        }

        private async Task GetParametresAsync()
        {
            SetLoading(true);
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

            SetLoading(false);
        }

        //private async void ItemHasBeenCommitted()
        //{
        //}
        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
            StateHasChanged();
        }

        private async Task Delete(int id)
        {
            string deleteContent = Localizer["Supprimer Contenu"];
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
                var response = await _labelTemplateClient.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages.ToString(), Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                _labelTemplate = _labelTemplateDTO.FirstOrDefault(c => c.Id == id);
                if (_labelTemplate != null)
                {
                    parameters.Add(nameof(AddEditLabelTemplate.labelTemplateDTO), new LabelTemplateDTO
                    {
                        Id = _labelTemplate.Id,
                        ModelName = _labelTemplate.ModelName,
                        ModelID = _labelTemplate.ModelID,
                        Type = _labelTemplate.Type
                    });
                }
            }

            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditLabelTemplate>(id == 0 ? Localizer["Créer"] : Localizer["Modifier"],
                parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _labelTemplate = new LabelTemplateDTO();
            await GetParametresAsync();
        }

        private bool Search(LabelTemplateDTO brand)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.ModelName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (brand.ModelID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return brand.ModelName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }

        private bool CodeSearch(ScanningCodeDTO code)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (code.BarCodeType?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (code.BarCodePrefix?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (code.BarCodeSuffix?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return code.BarCodeType?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }

        #region Save Line

        private async Task Save(ScanningCodeDTO context)
        {
            SetLoading(true);

            var result = await _scanningCodeClient.SaveCodeAsync(context);
            if (result.Succeeded)
            {
                _snackBar.Add($"Enregistré", Severity.Success);
            }
            else
            {
                _snackBar.Add($"Une erreur est survenue ", Severity.Error);
            }

            SetLoading(false);
        }

        #endregion

        #region Save All

        private async Task SaveAllCodes()
        {
            SetLoading(true);

            var result = await _scanningCodeClient.SaveAllCodesAsync(_scanningCodeDTO);
            if (result.Succeeded)
            {
                _snackBar.Add($"Enregistré", Severity.Success);
            }
            else
            {
                _snackBar.Add($"Une erreur est survenue ", Severity.Error);
            }

            SetLoading(false);
        }

        #endregion

        #region Send Line

        private async Task SendCode(ScanningCodeDTO context)
        {
            SetLoading(true);

            var codeList = new List<ScanningCodeDTO>();
            codeList.Add(context);
            var result = await _scanningCodeClient.UpdateCodeAsync(codeList);
            if (result.Succeeded)
            {
                _snackBar.Add($"Enregistré", Severity.Success);
            }
            else
            {
                _snackBar.Add($"Une erreur est survenue ", Severity.Error);
            }

            SetLoading(false);
        }

        #endregion

        #region Send All

        private async Task SendAllCodes()
        {
            SetLoading(true);

            var result = await _scanningCodeClient.UpdateCodeAsync(_scanningCodeDTO);
            if (result.Succeeded)
            {
                _snackBar.Add($"Enregistré", Severity.Success);
            }
            else
            {
                _snackBar.Add($"Une erreur est survenue ", Severity.Error);
            }

            SetLoading(false);
        }

        #endregion
    }
}