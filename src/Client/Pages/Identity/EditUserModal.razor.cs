using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Client.Infrastructure.Managers.Logistics;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests.Identity;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Client.Pages.Identity
{
    public partial class EditUserModal
    {
        private FluentValidationValidator _fluentValidationValidator;
        //private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly UpdateUserRequest _editUserModel = new();
        private List<EmployeeDTO> _employees = new();
        private UserResponse user = new();
        private List<SiteDTO> _sites = new();
        private EmployeeDTO employee = new();
        private SiteDTO site = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Inject] private ILogisticsManager _logisticsManager { get; set; }
        [Inject] private ILogisticAreaManager _logisticAreaManager { get; set; }
        [Parameter] public string userId { get; set; }
        private string UserName { get; set; }
        private string Email { get; set; }

        

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        protected override async Task OnInitializedAsync()
        {
            await GetEmployees();
            await GetSites();
            await GetUser();

        }
    
        private async Task GetUser()
        {
            var response = await _userManager.GetAsync(userId);
            if (response.Succeeded)
            {
                user = response.Data;
                _editUserModel.Id = user.Id;
                UserName = user.UserName;
                Email = user.Email;
                _editUserModel.EmployeeID = user.EmployeeID;
                _editUserModel.SiteID = user.SiteID;
            }
            else
            {
                foreach (var m in response.Messages)
                {
                    _snackBar.Add(m, Severity.Error);
                }

            }
        }
        private async Task GetEmployees()
        {
            var response = await _logisticsManager.GetEmployees();
            if (response.Succeeded)
            {
                _employees = response.Data.ToList();
            }
            else
            {
                foreach (var m in response.Messages)
                {
                    _snackBar.Add(m, Severity.Error);
                }

            }
        }
        private async Task GetSites()
        {
            var response = await _logisticAreaManager.GetSites();
            if (response.Succeeded)
            {
                _sites = response.Data.ToList();
                _sites.Add(new() { SiteID = "*", SiteName = "***ALL***" });
            }
            else
            {
                foreach (var m in response.Messages)
                {
                    _snackBar.Add(m, Severity.Error);
                }

            }
        }
        private async Task SubmitAsync()
        {
            //_registerUserModel.EmployeeID = employee.EmployeeID;
            //_registerUserModel.SiteID = site.SiteID;
            var response = await _userManager.UpdateUserAsync(_editUserModel);
            if (response.Succeeded)
            {
                _snackBar.Add("Utilisateur Mis a jour !", Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}
