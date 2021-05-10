
namespace Common.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Helpers;
    using Interfaces;
    using Models;
    using MvvmCross.Commands;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using Newtonsoft.Json;
    using Services;

    public class ApplicationsViewModel : MvxViewModel
    {
        private List<Application> applications;
        private readonly IApiService apiService;
        private readonly IDialogService dialogService;
        private readonly IMvxNavigationService navigationService;
        private MvxCommand addProductCommand;
        private MvxCommand<Application> itemClickCommand;

        public ApplicationsViewModel(
            IApiService apiService,
            IDialogService dialogService,
            IMvxNavigationService navigationService)
        {
            this.apiService = apiService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
        }

        public ICommand AddProductCommand
        {
            get
            {
                this.addProductCommand = this.addProductCommand ?? new MvxCommand(this.AddProduct);
                return this.addProductCommand;
            }
        }

        public ICommand ItemClickCommand
        {
            get
            {
                this.itemClickCommand = new MvxCommand<Application>(this.OnItemClickCommand);
                return itemClickCommand;
            }
        }

        public List<Application> Application
        {
            get => this.applications;
            set => this.SetProperty(ref this.applications, value);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            this.LoadProducts();
        }

        private async void OnItemClickCommand(Application application)
        {
            await this.navigationService.Navigate<ApplicationsDetailViewModel, NavigationArgs>(
                new NavigationArgs { Application = application });
        }

        private async void AddProduct()
        {
            await this.navigationService.Navigate<AddApplicationViewModel>();
        }

        private async void LoadProducts()
        {
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var response = await this.apiService.GetListAsync<Application>(
                Constants.UrlBase,
                "/api",
                "/Application",
                "bearer",
                token.Token);

            if (!response.IsSuccess)
            {
                this.dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            this.Application = (List<Application>)response.Result;
            this.Application = this.Application.OrderBy(p => p.Name).ToList();
        }
    }
}
