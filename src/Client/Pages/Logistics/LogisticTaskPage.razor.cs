using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorDB;
using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Infrastructure.Managers.Logistics;
using Uni.Scan.Client.Shared.Dialogs;
using Uni.Scan.Transfer.DataModel.LogisticTask;
using System.Net.Http;
using Microsoft.VisualBasic;
using Uni.Scan.Shared.Extensions;

namespace Uni.Scan.Client.Pages.Logistics
{
    public partial class LogisticTaskPage
    {
        [CascadingParameter] public App MyApp { get; set; }
        [Inject] private ILogisticTaskClient _LogisticTaskClient { get; set; }

        [Parameter] public string ProcessType { get; set; }

        private ClaimsPrincipal _currentUser;

        //private bool enabled = true;
        //private bool _loading;
        private string _searchString = "";

        //private List<LogisticTaskDTO2> _mytaskList = new();
        //private List<LogisticTaskDTO2> _unassignedtaskList = new();
        private string code = "";
        MudTabs tabs;
        MudTextField<string> scaninput;
        private MudIconButton returnButton;
        private List<LogisticTaskDTO2> _taskList;

        protected override async Task OnInitializedAsync()
        {
            _global.CurrentTitle = Localizer[ProcessType].ToString().ToUpper();
        }

        protected override async Task OnParametersSetAsync()
        {
            _global.CurrentTitle = Localizer[ProcessType].ToString().ToUpper();
            await LoadData();
        }


        private async Task LoadData()
        {
            SetLoading(true);
            _searchString = String.Empty;
            try
            {
                _taskList = new();

                var myresponse = await _LogisticTaskClient.GetAllowedTasksAsync(ProcessType);
                if (myresponse.Succeeded)
                {
                    _taskList = myresponse.Data;
                }
                else
                {
                    await _dialogService.ShowErrors(myresponse.Messages);
                }
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrors(e);
            }


            if (scaninput != null)
            {
                scaninput.Reset();
            }

            SetLoading(false);
        }


        private bool Search(LogisticTaskDTO2 taskDTO)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            var str = $"{taskDTO.Id}#{taskDTO.SiteId}#{taskDTO.ResponsibleName}#{taskDTO.ProcessType}#{taskDTO.RequestId}#";

            if (str.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await FocusScanInput();
        }

        private async Task FocusScanInput()
        {
            if (scaninput != null && false)
            {
                await returnButton.FocusAsync();
            }
        }


        private void SetLoading(bool val)
        {
            _global.IsLoading = val;
            StateHasChanged();
        }

        private async Task BackToParent()
        {
            SetLoading(true);

            _navigationManager.NavigateTo($"/Home");
            await Task.Delay(1);
            SetLoading(false);
        }


        private async Task Refresh()
        {
            SetLoading(true);


            await LoadData();


            SetLoading(false);
        }

        private void PrinLabels(LogisticTaskDTO2 dto)
        {
            _navigationManager.NavigateTo($"/Logistics/Labels/{ProcessType}/{dto.Id}");
        }

        private void PrinLabels2(LogisticTaskDTO2 dto)
        {
            _navigationManager.NavigateTo($"/Logistics/{dto.ProcessType}/Labels/{dto.ObjectID}");
        }
    }
}