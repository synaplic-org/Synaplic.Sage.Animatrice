using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Uni.Scan.Transfer.Requests.Identity;
using System.Collections.Generic;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Client.Infrastructure.Managers.Logistics;
using System.Linq;

namespace Uni.Scan.Client.Pages.Identity
{
    public partial class RegisterUserModal
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly RegisterRequest _registerUserModel = new();
        private List<EmployeeDTO> _employees = new();
        private List<SiteDTO> _sites = new();
        private EmployeeDTO employee = new();
        private SiteDTO site = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Inject] private ILogisticsManager _logisticsManager { get; set; }
        [Inject] private ILogisticAreaManager _logisticAreaManager { get; set; }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        protected override async void OnInitialized()
        {
            await GetEmployees();
            await GetSites();
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
                foreach(var m in response.Messages)
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
                _sites.Add(new() { SiteID="*", SiteName ="***ALL***" });
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
            var response = await _userManager.RegisterUserAsync(_registerUserModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
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

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }

        
    }
}