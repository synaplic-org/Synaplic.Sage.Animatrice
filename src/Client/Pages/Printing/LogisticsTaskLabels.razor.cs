using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Infrastructure.Managers.Identity.Roles;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Shared.Enums;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Client.Pages.Printing
{
    public partial class LogisticsTaskLabels
    {
        [Parameter] public string TaskId { get; set; }
        [Parameter] public string ProcessType { get; set; }
        [Parameter] public int ItemsNumberValue { get; set; }

        [Inject] private ILogisticsClient logisticsClient { get; set; }
        [Inject] private ILogisticTaskLabelClient logisticTaskLabelClient { get; set; }
        [Inject] private IRoleManager RoleManager { get; set; }
        private string _searchString = "";
        private bool _loaded;
        private LogisticTaskDetailDTO currentLine = new();
        private LogisticTaskDTO _task = new();
        public List<LogisticTaskDetailDTO> _taskDetails = new();
        private ClaimsPrincipal _currentUser;
        private bool _canCreateLabels;
        private bool _canEditLabels;
        private bool _canDeleteLabels;
        private bool _canSearchLabels;
        private bool _canPrintLabels;
        public string ModalType { get; set; }

        private LogisticTaskDetailDTO _taskDetail;
        private List<LogisticTaskLabelDTO> _taskLabels = new();
        private LogisticTaskLabelDTO label = new();

        public string PaperStyle(LogisticTaskDetailDTO obj)
        {
            if (obj.Lables.Sum(o => o.NbrEtiquettes * o.QuatityOnLabel) == obj.PlanQuantity)
            {
                return "border-top: solid 4px #008000";
            }

            if (obj.Lables.Sum(o => o.NbrEtiquettes * o.QuatityOnLabel) > obj.PlanQuantity)
            {
                return "border-top: solid 4px #Ff0000";
            }

            return "border-top:solid 4px #Ffa500";
        }

        private bool _processing = false;

        async Task ProcessSomething()
        {
            _processing = true;
            await GetTaskDetails();
            _processing = false;
        }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Create)).Succeeded;
            _canEditLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Edit)).Succeeded;
            _canDeleteLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Delete)).Succeeded;
            _canSearchLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Search)).Succeeded;
            _canPrintLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Print)).Succeeded;
            await GetTaskDetails();
            //_loaded = true;
        }

        private void BackToParent()
        {
            SetLoading(true);
            _navigationManager.NavigateTo($"/Logistics/{ProcessType}");
        }

        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
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

        private async Task GetTaskDetails()
        {
            SetLoading(true);
            var response = await logisticsClient.GetTaskDetailsAsync(TaskId);

            if (response.Succeeded)
            {
                _task = response.Data;

                _taskDetails = _task.LogisticTaskDetails.ToList();

                foreach (var item in _taskDetails)
                {
                    if (_task.Labels.ToList().IsNullOrEmpty())
                    {
                        AddLabel(item);
                    }
                    else
                    {
                        item.Lables = _task.Labels.Where(o =>
                            o.LineItemID == item.LineItemID && o.ProductId == item.ProductID && o.ProductName == item.ProductDescription).ToList();
                    }
                }
            }
            else
            {
                _ = await ShowErrors(response.Messages.ToList());


                BackToParent();
            }

            SetLoading(false);
        }


        private bool Search(LogisticTaskDetailDTO obj)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (obj.ProductDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (obj.ProductID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }


        private async Task SaveLabels()
        {
            SetLoading(true);
            var listelables = _taskDetails.SelectMany(o => o.Lables).ToList();
            var result = await logisticTaskLabelClient.SaveTaskLabelsAsync(listelables);
            if (result.Succeeded)
            {
                _snackBar.Add($"Enregistré", Severity.Success);
                await GetTaskDetails();
            }
            else
            {
                _snackBar.Add($"une erreur est survenue ", Severity.Error);
            }

            SetLoading(false);
        }


        private void AddLabel(LogisticTaskDetailDTO context)
        {
            context.Lables.Add(new LogisticTaskLabelDTO()
            {
                LineItemID = context.LineItemID,
                IdentifiedStock = context.IdentifiedStockID,
                TaskId = context.TaskId,
                LabelUuid = context.OutputUUID,
                PackageID = context.PackageLogisticUnitUUID,
                ProductId = context.ProductID,
                ProductName = context.ProductDescription,
                PlanQuantity = context.PlanQuantity - (context.Lables?.Sum(o => o.NbrEtiquettes * o.QuatityOnLabel) ?? 0),
                ProductSpecification = context.ProductSpecificationID,
                NbrEtiquettes = 1,
                Duplicata = 1,
                QuatityOnLabel = 1,
                Type = LabelType.Linked,
            });
            StateHasChanged();
        }

        private async void ItemHasBeenCommitted()
        {
            await SaveLabels();
            //await saveLine();

        }
        private async Task Delete(int id)
        {
            string deleteContent = Localizer["Supprimer Étiquette"];
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
                var response = await logisticTaskLabelClient.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    _snackBar.Add(Localizer["Étiquette supprimé avec succès ! "], Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add("Erreur ... ", Severity.Error);
                    }
                }
            }
        }

        private async Task Reset()
        {
            await GetTaskDetails();
        }

        private void ShowBtnPress(string pID)
        {
            currentLine = _taskDetails.First(f => f.ProductID == pID);
            currentLine.ShowDetails = !currentLine.ShowDetails;
        }

        private async Task InvokeModal()
        {
            var parameters = new DialogParameters();
            ModalType = "Label";
            parameters.Add(nameof(PrintModal.Id), TaskId);
            parameters.Add(nameof(PrintModal.ModalType), ModalType);
            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<PrintModal>(Localizer["Imprimer"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetTaskDetails();
            }
        }

        private async Task InvokeTaskModal()
        {
            var parameters = new DialogParameters();
            ModalType = "Task";
            parameters.Add(nameof(PrintModal.ModalType), ModalType);
            parameters.Add(nameof(PrintModal.TaskId), TaskId);
            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<PrintModal>(Localizer["Imprimer"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetTaskDetails();
            }
        }

        private async Task InvokeSectionLabelsModal(string ProductID)
        {
            var parameters = new DialogParameters();
            ModalType = "LabelSection";
            parameters.Add(nameof(PrintModal.ModalType), ModalType);
            parameters.Add(nameof(PrintModal.ProductId), ProductID);
            parameters.Add(nameof(PrintModal.Id), TaskId);
            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<PrintModal>(Localizer["Imprimer"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetTaskDetails();
            }
        }
    }
}