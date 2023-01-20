using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Shared.Enums;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.DataModel.LogisticTask;

namespace Uni.Scan.Client.Pages.Labels
{
    public partial class LogisticLabelPage
    {
        [Parameter] public string TaskObjectID { get; set; }
        [Parameter] public string ProcessType { get; set; }

        [Inject] private ILogisticTaskClient _LogisticTaskClient { get; set; }
        [Inject] private ILogisticLabelClient _LogisticLabelClient { get; set; }

        private LogisticTaskDTO2 _currentTask = new();
        private List<LogisticTaskItemDTO2> _logisticTaskistLines = new();

        private string _searchString;
        private ClaimsPrincipal _currentUser;
        private bool _canCreateLabels, _canEditLabels, _canDeleteLabels, _canSearchLabels, _canPrintLabels;
        private List<int> _expandedLineItems = new List<int>();
        private LogisticTaskLabelDTO2 selectedLabelItem;
        private LogisticTaskLabelDTO2 backedLable;
        private LogisticTaskItemDTO2 _selectedTaskItem;


        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
            StateHasChanged();
        }


        protected override async Task OnParametersSetAsync()
        {
            _global.CurrentTitle = Localizer["ETIQUETTES : "] + Localizer[ProcessType].ToString().ToUpper();
            await LoadData();
        }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Create)).Succeeded;
            _canEditLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Edit)).Succeeded;
            _canDeleteLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Delete)).Succeeded;
            _canSearchLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Search)).Succeeded;
            _canPrintLabels = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Labels.Print)).Succeeded;
            ExpandAllLabels();
        }


        private async Task LoadData()
        {
            SetLoading(true);

            try
            {
                var myresponse = await _LogisticTaskClient.GetTaskDetailAsync(TaskObjectID);
                if (myresponse.Succeeded)
                {
                    _currentTask = myresponse.Data;
                    _logisticTaskistLines = _currentTask.Items;
                    _global.CurrentTitle = Localizer["ETIQUETTES : "] + Localizer[_currentTask.OperationType].ToString().ToUpper() + $" [ {_currentTask.Id} ]";
                    if (_expandedLineItems.IsNullOrEmpty())
                    {
                        _expandedLineItems = _logisticTaskistLines.Select(o => o.LineItemID).ToList();
                    }
                }
                else
                {
                    await _dialogService.ShowErrors(myresponse.Messages);
                }
            }
            catch (HttpRequestException ex)
            {
                _snackBar.Add(Localizer["Vous êtes hors ligne !"]);
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrors(e);
                await BackToParentAsync();
            }


            SetLoading(false);
        }

        private async Task BackToParentAsync()
        {
            await Task.Delay(1);
        }


        private bool Search(LogisticTaskItemDTO2 arg)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            var str = $"{arg.LineItemID}#{arg.ProductID}#{arg.ProductDescription}#{arg.PlanQuantity}#{arg.PlanQuantityUnitCode}#";

            if (str.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }

        private void AddNewLabel(LogisticTaskItemDTO2 context)
        {
            ExpandLabels(context, true);
            if (context.Lables.Any(o => o.Id == 0))
            {
                _snackBar.Add(Localizer["Element incomplet dans la liste "], Severity.Warning);
                return;
            }

            var lbl = new LogisticTaskLabelDTO2
            {
                ProductId = context.ProductID,
                TaskId = _currentTask.Id,
                LineItemID = context.LineItemID,
                QuatityOnLabel = 1,
                Type = LabelType.Linked,
                QuatityUnite = context.PlanQuantityUnitCode,
                IsBatchManaged = context.IsBatchManaged,
                PlanQuantity = context.PlanQuantity - context.Lables.Sum(o => o.PlanQuantity)
            };

            if (lbl.PlanQuantity < 0M) lbl.PlanQuantity = 0;
            var oidstock = context.Details?.FirstOrDefault()?.IdentifiedStockID;
            if (!string.IsNullOrWhiteSpace(oidstock) && context.Lables.IsNullOrEmpty())
            {
                lbl.IdentifiedStock = oidstock;
            }

            context.Lables.Add(lbl);
        }


        private void ExpandLabels(LogisticTaskItemDTO2 context, bool forceexpand = false)
        {
            if (!_expandedLineItems.Contains(context.LineItemID))
            {
                _expandedLineItems.Add(context.LineItemID);
            }
            else if (!forceexpand)
            {
                _expandedLineItems.Remove(context.LineItemID);
            }

            StateHasChanged();
        }

        private void ExpandAllLabels()
        {
            if (_expandedLineItems.Any())
            {
                _expandedLineItems = new List<int>();
            }
            else
            {
                _expandedLineItems = _currentTask.Items?.Select(o => o.LineItemID).ToList() ?? new List<int>();
            }
        }

        private string GetLabelCardStyle(LogisticTaskItemDTO2 context)
        {
            var style = "text-align: center;font-size: larger;";
            var qte = context.Lables.Sum(o => o.PlanQuantity);
            if (qte == context.PlanQuantity) return style + "color: #2f7e2a";
            if (qte < context.PlanQuantity) return style + "color: #ff8318";
            if (qte > context.PlanQuantity) return style + "color: #ff0000";
            return style;
        }


        private async Task RemoveLabelAsync(LogisticTaskLabelDTO2 labelContext, LogisticTaskItemDTO2 parentItem)
        {
            if (labelContext.Id == 0)
            {
                parentItem.Lables.Remove(labelContext);
            }
            else
            {
                var confirm = await DeleteConfirmationDialog.ShowDialogAsync(_dialogService, Localizer[" Etes-vous sûrs de vouloir supprimer ce label ?"]);
                if (!confirm) return;


                SetLoading(true);
                try
                {
                    var resp = await _LogisticLabelClient.DeleteLabelAsync(labelContext);
                    if (resp.Succeeded)
                    {
                        await LoadData();
                    }
                    else
                    {
                        await _dialogService.ShowErrors(resp.Messages);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _snackBar.Add(Localizer["Vous êtes hors ligne !"]);
                }
                catch (Exception e)
                {
                    await _dialogService.ShowErrors(e);
                }

                SetLoading(false);
            }
        }


        private async Task PrintLabel(LogisticTaskLabelDTO2 labelContext)
        {
            await Task.Delay(1);
        }

        private void DuplicateLabel(LogisticTaskLabelDTO2 labelContext, LogisticTaskItemDTO2 parentDto)
        {
            ExpandLabels(parentDto, true);
            if (parentDto.Lables.Any(o => o.Id == 0))
            {
                _snackBar.Add(Localizer["Elément incomplet dans la liste "], Severity.Warning);
                return;
            }

            var duplicata = new LogisticTaskLabelDTO2();
            labelContext.Adapt(duplicata);
            duplicata.Id = 0;
            duplicata.IsBatchManaged = parentDto.IsBatchManaged;
            duplicata.PlanQuantity = parentDto.PlanQuantity - parentDto.Lables.Sum(o => o.PlanQuantity);

            if (duplicata.PlanQuantity < 0M) duplicata.PlanQuantity = 0;

            parentDto.Lables.Add(duplicata);
        }

        private async void LabelRowEditCommit(object obj)
        {
            SetLoading(true);
            try
            {
                var resp = await _LogisticLabelClient.SaveLabelAsync((LogisticTaskLabelDTO2)obj);
                if (resp.Succeeded)
                {
                    resp.Data.Adapt((LogisticTaskLabelDTO2)obj);
                    await LoadData();
                }
                else
                {
                    await _dialogService.ShowErrors(resp.Messages);
                }
            }
            catch (HttpRequestException ex)
            {
                _snackBar.Add(Localizer["Vous êtes hors ligne !"]);
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrors(e);
            }

            SetLoading(false);
        }


        private void BackupLabel(object obj)
        {
            var dto = (LogisticTaskLabelDTO2)obj;
            backedLable = new LogisticTaskLabelDTO2()
            {
                Id = dto.Id,
                IdentifiedStock = dto.IdentifiedStock,
                SupplierIdentifiedStock = dto.SupplierIdentifiedStock,
                PlanQuantity = dto.PlanQuantity,
                QuatityOnLabel = dto.QuatityOnLabel
            };
        }

        private void ResetLabelToOriginalValues(object obj)
        {
            var dto = (LogisticTaskLabelDTO2)obj;
            if (this.backedLable != null && dto.Id == this.backedLable.Id)
            {
                dto.IdentifiedStock = backedLable.IdentifiedStock;
                dto.SupplierIdentifiedStock = backedLable.SupplierIdentifiedStock;
                dto.PlanQuantity = backedLable.PlanQuantity;
                dto.QuatityOnLabel = backedLable.QuatityOnLabel;
            }
        }

        private async Task EditeLabelAsync(LogisticTaskLabelDTO2 labelContext)
        {
            var result = await LogisticLabelDialog.ShowDialogAsync(_dialogService, labelContext);
            if (result)
            {
                await LoadData();
            }
        }

        private async Task PrintTask(LogisticTaskDTO2 currentTask)
        {
            await LogisticTaskPrintDialog.ShowDialogAsync(_dialogService, currentTask);
        }

        private async Task PrintLables(LogisticTaskItemDTO2 context)
        {
            await LogisticLabelPrintDialog.ShowDialogAsync(_dialogService, context);
        }

        private async Task PrintOneLabelAsync(LogisticTaskLabelDTO2 labelContext, LogisticTaskItemDTO2 logisticTaskItemDto2)
        {
            await LogisticLabelPrintDialog.ShowDialogAsync(_dialogService, logisticTaskItemDto2, labelContext);
        }
    }
}