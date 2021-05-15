
namespace MobileApp.ViewModels
{
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Common.Helpers;
    using Common.Services;

    public class RegisterViewModel : BaseViewModel
    {
        private bool _isRunning;
        private bool _isEnabled;
        private readonly ApiService _apiService;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public string Confirm { get; set; }

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

        public ICommand RegisterCommand => new RelayCommand(Register);

        public RegisterViewModel()
        {
            _apiService = new ApiService();
            IsEnabled = true;
        }

        private async void Register()
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter the first name.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter the last name.",
                    "Accept");
                return;
            }

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

            if (string.IsNullOrEmpty(Address))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter an address.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Phone))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a phone number.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a password.",
                    "Accept");
                return;
            }

            if (Password.Length < 6)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You password must be at minimum 6 characters.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Confirm))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a password confirm.",
                    "Accept");
                return;
            }

            if (!Password.Equals(Confirm))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "The password and the confirm do not match.",
                    "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var request = new NewUserRequest
            {
                Address = Address,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password,
                Phone = Phone
            };

            var url = Xamarin.Forms.Application.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.RegisterUserAsync(
                url,
                "/api",
                "/Account",
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
