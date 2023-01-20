using Uni.Scan.Client.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Uni.Scan.Client.Shared.Components
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfName { get; set; }

        [Parameter] public string ImageDataUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            this.Email = user.GetEmail();
            this.FirstName = user.GetFirstName();
            this.SecondName = user.GetLastName();
            if (this.FirstName.Length > 0)
            {
                FirstLetterOfName = FirstName[0];
            }

            var UserId = user.GetUserId();
            if (_global.IsMobileView == false)
            {
                var imageResponse = await _accountManager.GetProfilePictureAsync(UserId);
                if (imageResponse.Succeeded && imageResponse.Data != null)
                {
                    ImageDataUrl = imageResponse.Data;
                }
            }
        }
    }
}