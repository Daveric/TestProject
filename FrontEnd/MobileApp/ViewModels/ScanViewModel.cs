
using System.Windows.Input;
using Common.Helpers;
using Common.Models;
using Common.Services;
using GalaSoft.MvvmLight.Command;
using MobileApp.Interfaces;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ScanViewModel
    {
        private readonly ApiService _apiService;

        public ScanViewModel()
        {
            _apiService = new ApiService();
        }

        public ICommand ScanCommand => new RelayCommand(ScanQrCode);

        private async void ScanQrCode()
        {
            var scanner = DependencyService.Get<IQrScanningService>();
            var result = await scanner.ScanAsync();
            if (result == null)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Cannot Scan now. Try again",
                    "Accept");
                return;
                
            };
            var request = new SetUserToAppRequest()
            {
                Email = Settings.UserEmail,
                AppId = result
            };

            var url = Xamarin.Forms.Application.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.PostUserToAppAsync(
                url,
                "/api",
                "/Applications/SetUserToApp",
                request,
                "bearer",
                MainViewModel.GetInstance().Token.Token);

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

            await App.Navigator.PopAsync();
        }
    }
}
