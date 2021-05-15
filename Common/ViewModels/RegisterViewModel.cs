
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

    public class RegisterViewModel : MvxViewModel
    {
        private readonly IApiService _apiService;
        private readonly IMvxNavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private MvxCommand registerCommand;
        private string firstName;
        private string lastName;
        private string email;
        private string address;
        private string phone;
        private string password;
        private string confirmPassword;
        private bool isLoading;

        public RegisterViewModel(
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

        public ICommand RegisterCommand
        {
            get
            {
                registerCommand ??= new MvxCommand(RegisterUser);
                return registerCommand;
            }
        }

        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        public string Phone
        {
            get => phone;
            set => SetProperty(ref phone, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        private async void RegisterUser()
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                _dialogService.Alert("Error", "You must enter a first name.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                _dialogService.Alert("Error", "You must enter a last name.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Email))
            {
                _dialogService.Alert("Error", "You must enter an email.", "Accept");
                return;
            }

            if (!RegexHelper.IsValidEmail(Email))
            {
                _dialogService.Alert("Error", "You must enter a valid email.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Address))
            {
                _dialogService.Alert("Error", "You must enter an address.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Phone))
            {
                _dialogService.Alert("Error", "You must enter a phone.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                _dialogService.Alert("Error", "You must enter a pasword.", "Accept");
                return;
            }

            if (Password.Length < 6)
            {
                _dialogService.Alert("Error", "The password must be a least 6 characters.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                _dialogService.Alert("Error", "You must enter a password confirm.", "Accept");
                return;
            }

            if (!Password.Equals(ConfirmPassword))
            {
                _dialogService.Alert("Error", "The password and confirm does not math.", "Accept");
                return;
            }

            IsLoading = true;

            var request = new NewUserRequest
            {
                Address = Address,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password,
                Phone = Phone
            };

            var response = await _apiService.RegisterUserAsync(
                Constants.UrlBase,
                "/api",
                "/Account",
                request);

            IsLoading = false;

            if (!response.IsSuccess)
            {
                _dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            _dialogService.Alert(
                "Ok",
                "The user was created successfully, you must " +
                "confirm your user by the email sent to you and then you could login with " +
                "the email and password entered.",
                "Accept",
                () => { _navigationService.Close(this); }
            );
        }
    }
}
