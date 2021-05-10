using MvvmCross.ViewModels;
using System.Windows.Input;
using Common.Helpers;
using Common.Interfaces;
using Common.Models;
using Common.Services;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Newtonsoft.Json;

namespace Common.ViewModels
{
    class AddApplicationViewModel: MvxViewModel
    {
        private string name;
        private MvxCommand addApplicationCommand;
        private readonly IApiService apiService;
        private readonly IDialogService dialogService;
        private readonly IMvxNavigationService navigationService;
        private bool isLoading;

        public AddApplicationViewModel(
            IApiService apiService,
            IDialogService dialogService,
            IMvxNavigationService navigationService)
        {
            this.apiService = apiService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
        }
        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }
        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        public ICommand AddProductCommand
        {
            get
            {
                addApplicationCommand ??= new MvxCommand(AddApplication);
                return addApplicationCommand;
            }
        }

        private async void AddApplication()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                this.dialogService.Alert("Error", "You must enter a product name.", "Accept");
                return;
            }

            this.IsLoading = true;

            var application = new Application
            {
                Name = this.Name,
                User = new User { UserName = Settings.UserEmail }
            };

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await this.apiService.PostAsync(
                Constants.UrlBase,
                "/api",
                "/Applications",
                application,
                "bearer",
                token.Token);

            this.IsLoading = false;

            if (!response.IsSuccess)
            {
                this.dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            await this.navigationService.Close(this);
        }
    }
}
