
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
        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly IMvxNavigationService _navigationService;
        private Application application;
        private bool isLoading;
        private MvxCommand updateCommand;
        private MvxCommand deleteCommand;

        public ApplicationsDetailViewModel(
            IApiService apiService,
            IDialogService dialogService,
            IMvxNavigationService navigationService)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            IsLoading = false;
        }

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public Application Application
        {
            get => application;
            set => SetProperty(ref application, value);
        }

        public ICommand UpdateCommand
        {
            get
            {
                updateCommand ??= new MvxCommand(Update);
                return updateCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                deleteCommand ??= new MvxCommand(Delete);
                return deleteCommand;
            }
        }

        private void Delete()
        {
            _dialogService.Confirm(
                "Confirm",
                "This action can't be undone, are you sure to delete the product?",
                "Yes",
                "No",
                () => { var task = ConfirmDelete(); },
                null);
        }

        private async Task ConfirmDelete()
        {
            IsLoading = true;

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.DeleteAsync(
                Constants.UrlBase,
                "/api",
                "/Application",
                application.Id,
                "bearer",
                token.Token);

            IsLoading = false;

            if (!response.IsSuccess)
            {
                _dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            await _navigationService.Close(this);
        }

        private async void Update()
        {
            if (string.IsNullOrEmpty(Application.Name))
            {
                _dialogService.Alert("Error", "You must enter an application name.", "Accept");
                return;
            }
            IsLoading = true;

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.PutAsync(
                Constants.UrlBase,
                "/api",
                "/Application",
                application.Id,
                application,
                "bearer",
                token.Token);

            IsLoading = false;

            if (!response.IsSuccess)
            {
                _dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            await _navigationService.Close(this);
        }

        public override void Prepare(NavigationArgs parameter) => application = parameter.Application;
    }
}
