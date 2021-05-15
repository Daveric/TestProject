
namespace MobileApp.ViewModels
{
    using System.Windows.Input;
    using Common.Helpers;
    using Common.Services;
    using GalaSoft.MvvmLight.Command;
    using Common.Models;

    public class RememberPasswordViewModel : BaseViewModel
    {
        private bool _isRunning;
        private bool _isEnabled;
        private readonly ApiService _apiService;

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

        public string Email { get; set; }

        public ICommand RecoverCommand => new RelayCommand(Recover);

        public RememberPasswordViewModel()
        {
            _apiService = new ApiService();
            IsEnabled = true;
        }

        private async void Recover()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter an email.",
                    "Accept");
                return;
            }

            if (!RegexHelper.IsValidEmail(Email))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a valid email.",
                    "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var request = new RecoverPasswordRequest
            {
                Email = Email
            };

            var url = Xamarin.Forms.Application.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.RecoverPasswordAsync(
                url,
                "/api",
                "/Account/RecoverPassword",
                request);

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

            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                "Ok",
                response.Message,
                "Accept");
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
