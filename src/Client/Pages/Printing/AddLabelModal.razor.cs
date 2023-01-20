using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Shared.Constants.Application;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Client.Pages.Printing
{
    public partial class AddLabelModal
    {
        [Inject] private ILogisticTaskLabelClient logisticTaskLabelClient { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public LogisticTaskLabelDTO taskLabelDTO { get; set; } = new();
        private List<LogisticTaskLabelDTO> _taskLabelDTO { get; set; } = new();

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated()
        {
            if (string.IsNullOrEmpty(taskLabelDTO.ProductId)) return false;
            if (string.IsNullOrEmpty(taskLabelDTO.SupplierIdentifiedStock)) return false;
            if (string.IsNullOrEmpty(taskLabelDTO.IdentifiedStock)) return false;
            return true;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();

            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            List<LogisticTaskLabelDTO> _list = new();
            _list.Add(taskLabelDTO);
            taskLabelDTO.Type = Scan.Shared.Enums.LabelType.Free;
            var response = await logisticTaskLabelClient.SaveTaskLabelsAsync(_list);
            if (response.Succeeded)
            {
                if (taskLabelDTO.Id == 0)
                {
                    _snackBar.Add("Étiquette Enregistré", Severity.Success);
                }
                else
                {
                    _snackBar.Add("Étiquette mis à jour", Severity.Success);
                }

                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
        }

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}