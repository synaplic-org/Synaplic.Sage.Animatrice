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
    public partial class AddEditLabelTemplate
    {
        [Inject] private ILabelTemplateClient _labelTemplateClient { get; set; }

        [Parameter] public LabelTemplateDTO labelTemplateDTO { get; set; } = new();
        private List<LabelTemplateDTO> _labelTemplateDTO { get; set; } = new();

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        [CascadingParameter] private HubConnection HubConnection { get; set; }

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
            var response = await _labelTemplateClient.SaveAsync(labelTemplateDTO);
            if (response.Succeeded)
            {
                if (labelTemplateDTO.Id == 0)
                {
                    _snackBar.Add("Modèle Enregistré", Severity.Success);
                }
                else
                {
                    _snackBar.Add("Modèle mis a jour", Severity.Success);
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