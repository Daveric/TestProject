
namespace MobileApp.ViewModels
{
    using System.Windows.Input;
    using Common.Models;
    using Common.Services;
    using GalaSoft.MvvmLight.Command;
    using Common.Helpers;

    public class ChangePasswordViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string PasswordConfirm { get; set; }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetValue(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        public ICommand ChangePasswordCommand => new RelayCommand(ChangePassword);

        public ChangePasswordViewModel()
        {
            _apiService = new ApiService();
            IsEnabled = true;
        }

        private async void ChangePassword()
        {
            if (string.IsNullOrEmpty(CurrentPassword))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter the current password.",
                    "Accept");
                return;
            }

            if (!MainViewModel.GetInstance().UserPassword.Equals(CurrentPassword))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "The current password is incorrect.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter the new password.",
                    "Accept");
                return;
            }

            if (NewPassword.Length < 6)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "The password must have at least 6 characters length.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(PasswordConfirm))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter the password confirm.",
                    "Accept");
                return;
            }

            if (!NewPassword.Equals(PasswordConfirm))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "The password and confirm does not match.",
                    "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var request = new ChangePasswordRequest
            {
                Email = MainViewModel.GetInstance().UserEmail,
                NewPassword = NewPassword,
                OldPassword = CurrentPassword
            };

            var url =Xamarin.Forms.Application.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.ChangePasswordAsync(
                url,
                "/api",
                "/Account/ChangePassword",
                request,
                "bearer",
                MainViewModel.GetInstance().Token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }

            MainViewModel.GetInstance().UserPassword = NewPassword;
            Settings.UserPassword = NewPassword;

            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                "Ok",
                response.Message,
                "Accept");

            await App.Navigator.PopAsync();
        }
    }
}
