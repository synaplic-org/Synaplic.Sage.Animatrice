using System;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Client.Shared.Dialogs;

namespace Uni.Scan.Client.Extensions
{
    public static class DialogServiceExtentions
    {
        public static async Task<DialogResult> ShowErrors(this IDialogService _dialogService,
            List<string> responseMessages)
        {
            string buttonText = "FERMER";
            var parameters = new DialogParameters();
            parameters.Add("Errors", responseMessages);
            var options = new DialogOptions { CloseButton = true, FullScreen = false, FullWidth = true };
            var dialog = _dialogService.Show<ErrorDialog>("Errors", parameters);
            var result = await dialog.Result;
            return result;
        }

        public static async Task<DialogResult> ShowErrors(this IDialogService _dialogService,
            Exception ex)
        {
            string buttonText = "FERMER";
            var parameters = new DialogParameters();
            parameters.Add("Errors", new List<string>() { ex.Message });
            var options = new DialogOptions { CloseButton = true, FullScreen = false, FullWidth = true };
            var dialog = _dialogService.Show<ErrorDialog>("Errors", parameters, options);
            var result = await dialog.Result;
            return result;
        }
    }
}