using Uni.Scan.Client.Extensions;
using Uni.Scan.Client.Infrastructure.Settings;
using Uni.Scan.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Uni.Scan.Client.Infrastructure.Managers.Identity.Roles;
using Microsoft.AspNetCore.Components;
using Uni.Scan.Client.Shared.Dialogs;
using System.Net.Http;

namespace Uni.Scan.Client.Shared
{
    public partial class NoAppBarLayout : IDisposable
    {
        [Inject] private IRoleManager RoleManager { get; set; }

        private string CurrentUserId { get; set; }
        private string ImageDataUrl { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfName { get; set; }


        private async Task LoadDataAsync()
        {
            try
            {
                var state = await _stateProvider.GetAuthenticationStateAsync();
                var user = state.User;
                if (user == null) return;
                if (user.Identity?.IsAuthenticated == true)
                {
                    CurrentUserId = user.GetUserId();
                    FirstName = user.GetFirstName();
                    if (FirstName.Length > 0)
                    {
                        FirstLetterOfName = FirstName[0];
                    }

                    SecondName = user.GetLastName();
                    Email = user.GetEmail();
                    var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
                    if (imageResponse.Succeeded)
                    {
                        ImageDataUrl = imageResponse.Data;
                    }

                    var currentUserResult = await _userManager.GetAsync(CurrentUserId);
                    if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                    {
                        _snackBar.Add(
                            Localizer["You are logged out because the user with your Token has been deleted."],
                            Severity.Error);
                        await _authenticationManager.Logout();
                    }

                    await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _snackBar.Add(e.Message, Severity.Error);
            }
        }

        private MudTheme _currentTheme;
        private bool _drawerOpen = true;
        private bool _rightToLeft = false;

        private async Task RightToLeftToggle()
        {
            var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
            _rightToLeft = isRtl;
            _drawerOpen = false;
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _currentTheme = UniTheme.DefaultTheme;
                _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
                _rightToLeft = await _clientPreferenceManager.IsRTL();
                _interceptor.RegisterEvent();
                hubConnection = hubConnection.TryInitialize(_navigationManager);
                await hubConnection.StartAsync();

                await LoadDataAsync();

                //hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
                //{
                //    if (CurrentUserId == receiverUserId)
                //    {
                //        _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                //        _snackBar.Add(message, Severity.Info, config =>
                //        {
                //            config.VisibleStateDuration = 10000;
                //            config.HideTransitionDuration = 500;
                //            config.ShowTransitionDuration = 500;
                //            config.Action = Localizer["Chat?"];
                //            config.ActionColor = Color.Primary;
                //            config.Onclick = snackbar =>
                //            {
                //                _navigationManager.NavigateTo($"chat/{senderUserId}");
                //                return Task.CompletedTask;
                //            };
                //        });
                //    }
                //});
                hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
                {
                    try
                    {
                        var token = await _authenticationManager.TryForceRefreshToken();
                        if (!string.IsNullOrEmpty(token))
                        {
                            _snackBar.Add(Localizer["Refreshed Token."], Severity.Success);
                            _httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", token);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _snackBar.Add(Localizer["You are Logged Out."], Severity.Error);
                        await _authenticationManager.Logout();
                        _navigationManager.NavigateTo("/");
                    }
                });
                hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole,
                    async (userId, roleId) =>
                    {
                        if (CurrentUserId != userId)
                        {
                            var rolesResponse = await RoleManager.GetRolesAsync();
                            if (rolesResponse.Succeeded)
                            {
                                var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
                                if (role != null)
                                {
                                    var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
                                    if (currentUserRolesResponse.Succeeded &&
                                        currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                                    {
                                        _snackBar.Add(
                                            Localizer[
                                                "You are logged out because the Permissions of one of your Roles have been updated."],
                                            Severity.Error);
                                        await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect,
                                            CurrentUserId);
                                        await _authenticationManager.Logout();
                                        _navigationManager.NavigateTo("/login");
                                    }
                                }
                            }
                        }
                    });
            }
            catch (HttpRequestException ex)
            {
                _snackBar.Add(Localizer["Vous êtes hors ligne !"], Severity.Warning);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _snackBar.Add(Localizer[e.Message], Severity.Warning);
            }
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                { nameof(LogoutDialog.ContentText), $"{Localizer["Logout Confirmation"]}" },
                { nameof(LogoutDialog.ButtonText), $"{Localizer["Logout"]}" },
                { nameof(LogoutDialog.Color), Color.Error },
                { nameof(LogoutDialog.CurrentUserId), CurrentUserId },
                { nameof(LogoutDialog.HubConnection), hubConnection }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            _dialogService.Show<Dialogs.LogoutDialog>(Localizer["Logout"], parameters, options);
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private async Task DarkMode()
        {
            bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
            _currentTheme = isDarkMode
                ? UniTheme.DefaultTheme
                : UniTheme.DarkTheme;
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
            //_ = hubConnection.DisposeAsync();
        }

        private HubConnection hubConnection;
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
    }
}