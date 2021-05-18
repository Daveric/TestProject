
using System.Windows.Input;
using Common.Helpers;
using Common.Models;
using Common.Services;
using GalaSoft.MvvmLight.Command;
using MobileApp.Helper;
using MobileApp.Views;
using Newtonsoft.Json;

namespace MobileApp.ViewModels
{
    public class LoginViewModel: BaseViewModel
    {
        private readonly ApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;

        public bool IsRemember { get; set; }

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

        public string Password { get; set; }

        public ICommand LoginCommand => new RelayCommand(Login);

        public ICommand RegisterCommand => new RelayCommand(Register);

        public ICommand RememberPasswordCommand => new RelayCommand(RememberPassword);

        public LoginViewModel()
        {
            _apiService = new ApiService();
            IsEnabled = true;
            IsRemember = true;
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EmailError,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordError,
                    Languages.Accept);
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var url = Xamarin.Forms.Application.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.GetTokenAsync(
                url,
                "/Account",
                "/CreateToken",
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

            var token = (TokenResponse)response.Result;

            var response2 = await _apiService.GetUserByEmailAsync(
                url,
                "/api",
                "/Account/GetUserByEmail",
                Email,
                "bearer",
                token.Token);

            var user = (User)response2.Result;
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.User = user;
            mainViewModel.UserEmail = Email;
            mainViewModel.UserPassword = Password;
            mainViewModel.Token = token;
            mainViewModel.Scan = new ScanViewModel();

            Settings.IsRemember = IsRemember;
            Settings.UserEmail = Email;
            Settings.UserPassword = Password;
            Settings.Token = JsonConvert.SerializeObject(token);
            Settings.User = JsonConvert.SerializeObject(user);

            Xamarin.Forms.Application.Current.MainPage = new MasterPage();
        }

        private async void Register()
        {
            MainViewModel.GetInstance().Register = new RegisterViewModel();
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }

        private async void RememberPassword()
        {
            MainViewModel.GetInstance().RememberPassword = new RememberPasswordViewModel();
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new RememberPasswordPage());
        }
    }
}
