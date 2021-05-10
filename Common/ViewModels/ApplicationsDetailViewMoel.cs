
namespace Common.ViewModels
{
    using Helpers;
    using Interfaces;
    using Models;
    using MvvmCross.Commands;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using Newtonsoft.Json;
    using Services;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class ApplicationsDetailViewModel : MvxViewModel<NavigationArgs>
    {
        private readonly IApiService apiService;
        private readonly IDialogService dialogService;
        private readonly IMvxNavigationService navigationService;
        private Application application;
        private bool isLoading;
        private MvxCommand updateCommand;
        private MvxCommand deleteCommand;

        public ApplicationsDetailViewModel(
            IApiService apiService,
            IDialogService dialogService,
            IMvxNavigationService navigationService)
        {
            this.apiService = apiService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.IsLoading = false;
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public Application Application
        {
            get => this.application;
            set => this.SetProperty(ref this.application, value);
        }

        public ICommand UpdateCommand
        {
            get
            {
                updateCommand ??= new MvxCommand(this.Update);
                return this.updateCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                deleteCommand ??= new MvxCommand(this.Delete);
                return this.deleteCommand;
            }
        }

        private void Delete()
        {
            this.dialogService.Confirm(
                "Confirm",
                "This action can't be undone, are you sure to delete the product?",
                "Yes",
                "No",
                () => { var task = ConfirmDelete(); },
                null);
        }

        private async Task ConfirmDelete()
        {
            this.IsLoading = true;

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await this.apiService.DeleteAsync(
                Constants.UrlBase,
                "/api",
                "/Application",
                application.Id,
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

        private async void Update()
        {
            if (string.IsNullOrEmpty(Application.Name))
            {
                this.dialogService.Alert("Error", "You must enter an application name.", "Accept");
                return;
            }
            IsLoading = true;

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await this.apiService.PutAsync(
                Constants.UrlBase,
                "/api",
                "/Application",
                application.Id,
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

        public override void Prepare(NavigationArgs parameter) => application = parameter.Application;
    }
}
