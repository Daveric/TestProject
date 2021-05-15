
namespace Common.ViewModels
{
    using System.Windows.Input;
    using Interfaces;
    using Models;
    using MvvmCross.Commands;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using Services;
    using Helpers;
    using Newtonsoft.Json;

    public class LoginViewModel : MvxViewModel
    {
        private string email;
        private string password;
        private MvxCommand loginCommand;
        private MvxCommand registerCommand;
        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly IMvxNavigationService _navigationService;
        private readonly INetworkProvider _networkProvider;
        private bool isLoading;

        public LoginViewModel(
            IApiService apiService,
            IDialogService dialogService,
            IMvxNavigationService navigationService,
            INetworkProvider networkProvider)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _networkProvider = networkProvider;

            Email = "erickdavid17@outlook.com";
            Password = "Pwd12345.";
            IsLoading = false;
        }

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public ICommand LoginCommand
        {
            get
            {
                loginCommand ??= new MvxCommand(DoLoginCommand);
                return loginCommand;
            }
        }

        public ICommand RegisterCommand
        {
            get
            {
                registerCommand ??= new MvxCommand(DoRegisterCommand);
                return registerCommand;
            }
        }

        private async void DoRegisterCommand()
        {
            await _navigationService.Navigate<RegisterViewModel>();
        }

        private async void DoLoginCommand()
        {
            if (string.IsNullOrEmpty(Email))
            {
                _dialogService.Alert("Error", "You must enter an email.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                _dialogService.Alert("Error", "You must enter a password.", "Accept");
                return;
            }

            if (!_networkProvider.IsConnectedToWifi())
            {
                _dialogService.Alert("Error", "The App requiered a internet connection, please check and try again.", "Accept");
                return;
            }

            IsLoading = true;

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var response = await _apiService.GetTokenAsync(
                Constants.UrlBase,
                "/Account",
                "/CreateToken",
                request);

            if (!response.IsSuccess)
            {
                IsLoading = false;
                _dialogService.Alert("Error", "User or password incorrect.", "Accept");
                return;
            }

            var token = (TokenResponse)response.Result;

            var response2 = await _apiService.GetUserByEmailAsync(
                Constants.UrlBase,
                "/api",
                "/Account/GetUserByEmail",
                Email,
                "bearer",
                token.Token);

            var user = (User)response2.Result;
            Settings.UserPassword = Password;
            Settings.User = JsonConvert.SerializeObject(user);
            Settings.UserEmail = Email;
            Settings.Token = JsonConvert.SerializeObject(token);

            IsLoading = false;
            await _navigationService.Navigate<ApplicationsViewModel>();
        }
    }
}
