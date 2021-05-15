
namespace MobileApp.ViewModels
{
    using System.Windows.Input;
    using Common.Models;
    using Common.Services;
    using GalaSoft.MvvmLight.Command;
    using Newtonsoft.Json;
    using Common.Helpers;
    using Views;

    public class ProfileViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private User _user;
        
        public User User
        {
            get => _user;
            set => SetValue(ref _user, value);
        }

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

        public ICommand SaveCommand => new RelayCommand(Save);

        public ICommand ModifyPasswordCommand => new RelayCommand(ModifyPassword);

        public ProfileViewModel()
        {
            _apiService = new ApiService();
            User = MainViewModel.GetInstance().User;
            IsEnabled = true;
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(User.FirstName))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter the first name.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(User.LastName))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter the last name.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(User.Address))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter an address.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(User.PhoneNumber))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a phone number.",
                    "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = Xamarin.Forms.Application.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.PutAsync(
                url,
                "/api",
                "/Account",
                User,
                "bearer",
                MainViewModel.GetInstance().Token.Token);

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

            MainViewModel.GetInstance().User = User;
            Settings.User = JsonConvert.SerializeObject(User);

            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                "Ok",
                "User updated!",
                "Accept");
            await App.Navigator.PopAsync();
        }

        private async void ModifyPassword()
        {
            MainViewModel.GetInstance().ChangePassword = new ChangePasswordViewModel();
            await App.Navigator.PushAsync(new ChangePasswordPage());
        }
    }
}
