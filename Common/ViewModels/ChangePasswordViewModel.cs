
namespace Common.ViewModels
{
    using System.Windows.Input;
    using Helpers;
    using Interfaces;
    using Models;
    using MvvmCross.Commands;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using Newtonsoft.Json;
    using Services;

    public class ChangePasswordViewModel : MvxViewModel
    {
        private readonly IApiService _apiService;
        private readonly IMvxNavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private MvxCommand changePasswordCommand;
        private string currentPassword;
        private string newPassword;
        private string confirmPassword;
        private bool isLoading;

        public ChangePasswordViewModel(
            IMvxNavigationService navigationService,
            IApiService apiService,
            IDialogService dialogService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            IsLoading = false;
        }

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public ICommand ChangePasswordCommand
        {
            get
            {
                changePasswordCommand ??= new MvxCommand(ChangePassword);
                return changePasswordCommand;
            }
        }

        public string CurrentPassword
        {
            get => currentPassword;
            set => SetProperty(ref currentPassword, value);
        }

        public string NewPassword
        {
            get => newPassword;
            set => SetProperty(ref newPassword, value);
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        private async void ChangePassword()
        {
            if (string.IsNullOrEmpty(CurrentPassword))
            {
                _dialogService.Alert("Error", "You must enter a current pasword.", "Accept");
                return;
            }

            if (!CurrentPassword.Equals(Settings.UserPassword))
            {
                _dialogService.Alert("Error", "The current pasword is not correct.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                _dialogService.Alert("Error", "You must enter a new pasword.", "Accept");
                return;
            }

            if (NewPassword.Length < 6)
            {
                _dialogService.Alert("Error", "The new password must be a least 6 characters.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                _dialogService.Alert("Error", "You must enter a password confirm.", "Accept");
                return;
            }

            if (!NewPassword.Equals(ConfirmPassword))
            {
                _dialogService.Alert("Error", "The password and confirm does not math.", "Accept");
                return;
            }

            IsLoading = true;

            var request = new ChangePasswordRequest
            {
                Email = Settings.UserEmail,
                NewPassword = NewPassword,
                OldPassword = CurrentPassword
            };

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var response = await _apiService.ChangePasswordAsync(
                Constants.UrlBase,
                "/api",
                "/Account/ChangePassword",
                request,
                "bearer",
                token.Token);

            if (!response.IsSuccess)
            {
                _dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            Settings.UserPassword = NewPassword;
            await _navigationService.Close(this);
        }
    }
}
