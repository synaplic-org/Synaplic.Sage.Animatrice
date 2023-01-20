using System;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Uni.Scan.Shared.Constants.Identity;
using Uni.Scan.Transfer.Requests.Identity;
using System.Globalization;
using Microsoft.JSInterop;

namespace Uni.Scan.Client.Pages.Authentication
{
    public partial class Login
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();

        private string CurrentLanguageCode { get; set; }


        protected override async Task OnInitializedAsync()
        {
            CurrentLanguageCode = await _clientPreferenceManager.GetCurrentLanguageCodeAsync();
            var state = await _stateProvider.GetAuthenticationStateAsync();
            if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            {
                _navigationManager.NavigateTo("/");
            }
            //await AddScannerEventListener();
        }

        private async Task SubmitAsync()
        {
            var result = await _authenticationManager.Login(_tokenModel);
            if (result.Succeeded)
            {
                _snackBar.Add(string.Format(Localizer["Welcome {0}"], _tokenModel.Email), Severity.Success);
                _navigationManager.NavigateTo("/", true);
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        void TogglePasswordVisibility()
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

        private void FillAdministratorCredentials()
        {
            _tokenModel.Email = UserConstants.SuperAdminEmail;
            _tokenModel.Password = UserConstants.DefaultPassword;
        }

        private void FillBasicUserCredentials()
        {
            _tokenModel.Email = UserConstants.BasicUserEmail;
            _tokenModel.Password = UserConstants.BasicPassword;
        }

        private async void ChangeLanguage(string value)
        {
            if (value == CurrentLanguageCode) return;

            var result = await _clientPreferenceManager.ChangeLanguageAsync(value);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                //System.Console.WriteLine("value : " +value + "    #########");
                //System.Console.WriteLine("CurrentLanguageCode : " +value + "    #########");
                //CultureInfo culture = new CultureInfo(value); ;
                //CultureInfo.DefaultThreadCurrentCulture = culture;
                //CultureInfo.DefaultThreadCurrentUICulture = culture;
                //await InvokeAsync(() => StateHasChanged());
                // _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }


        [JSInvokable]
        public void OnScanned(string barcode)
        {
            if (barcode.Contains("@"))
            {
                _tokenModel.Email = barcode.Replace("EMAIL:", "");
                Console.WriteLine(barcode);
                StateHasChanged();
            }
            else if (barcode.StartsWith("PASS:"))
            {
                _tokenModel.Password = barcode.Replace("PASS:", "");
                Console.WriteLine(barcode);
                StateHasChanged();
            }
        }

        private async Task AddScannerEventListener()
        {
            var dotNetObjectReference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("addScannerEventListener", dotNetObjectReference);
        }
    }
}