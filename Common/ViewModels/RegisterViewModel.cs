
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
        private readonly IApiService apiService;
        private readonly IMvxNavigationService navigationService;
        private readonly IDialogService dialogService;
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
            this.apiService = apiService;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.IsLoading = false;
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public ICommand RegisterCommand
        {
            get
            {
                registerCommand ??= new MvxCommand(this.RegisterUser);
                return this.registerCommand;
            }
        }

        public string FirstName
        {
            get => this.firstName;
            set => this.SetProperty(ref this.firstName, value);
        }

        public string LastName
        {
            get => this.lastName;
            set => this.SetProperty(ref this.lastName, value);
        }

        public string Email
        {
            get => this.email;
            set => this.SetProperty(ref this.email, value);
        }

        public string Address
        {
            get => this.address;
            set => this.SetProperty(ref this.address, value);
        }

        public string Phone
        {
            get => this.phone;
            set => this.SetProperty(ref this.phone, value);
        }

        public string Password
        {
            get => this.password;
            set => this.SetProperty(ref this.password, value);
        }

        public string ConfirmPassword
        {
            get => this.confirmPassword;
            set => this.SetProperty(ref this.confirmPassword, value);
        }

        private async void RegisterUser()
        {
            if (string.IsNullOrEmpty(this.FirstName))
            {
                this.dialogService.Alert("Error", "You must enter a first name.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.LastName))
            {
                this.dialogService.Alert("Error", "You must enter a last name.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Email))
            {
                this.dialogService.Alert("Error", "You must enter an email.", "Accept");
                return;
            }

            if (!RegexHelper.IsValidEmail(this.Email))
            {
                this.dialogService.Alert("Error", "You must enter a valid email.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Address))
            {
                this.dialogService.Alert("Error", "You must enter an address.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Phone))
            {
                this.dialogService.Alert("Error", "You must enter a phone.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                this.dialogService.Alert("Error", "You must enter a pasword.", "Accept");
                return;
            }

            if (this.Password.Length < 6)
            {
                this.dialogService.Alert("Error", "The password must be a least 6 characters.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.ConfirmPassword))
            {
                this.dialogService.Alert("Error", "You must enter a pasword confirm.", "Accept");
                return;
            }

            if (!this.Password.Equals(this.ConfirmPassword))
            {
                this.dialogService.Alert("Error", "The pasword and confirm does not math.", "Accept");
                return;
            }

            this.IsLoading = true;

            var request = new NewUserRequest
            {
                Address = this.Address,
                Email = this.Email,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Password = this.Password,
                Phone = this.Phone
            };

            var response = await this.apiService.RegisterUserAsync(
                Constants.UrlBase,
                "/api",
                "/Account",
                request);

            this.IsLoading = false;

            if (!response.IsSuccess)
            {
                this.dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            this.dialogService.Alert(
                "Ok",
                "The user was created successfully, you must " +
                "confirm your user by the email sent to you and then you could login with " +
                "the email and password entered.",
                "Accept",
                () => { this.navigationService.Close(this); }
            );
        }
    }
}
