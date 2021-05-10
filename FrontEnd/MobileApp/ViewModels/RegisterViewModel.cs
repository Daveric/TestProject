using System;
using System.Windows.Input;
using Firebase.Auth;
using GalaSoft.MvvmLight.Command;
using MobileApp.Views.AccessApp;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Attributes
        public string Email;
        public string Password;
        public string Name;
        public string Age;

        public bool IsRunning;
        public bool IsVisible;
        public bool IsEnabled;
        #endregion

        #region Properties
        public string EmailTxt
        {
            get { return this.Email; }
            set { SetValue(ref this.Email, value); }
        }

        public string PasswordTxt
        {
            get { return this.Password; }
            set { SetValue(ref this.Password, value); }
        }

        public string NameTxt
        {
            get { return this.Name; }
            set { SetValue(ref this.Name, value); }
        }

        public string AgeTxt
        {
            get { return this.Age; }
            set { SetValue(ref this.Age, value); }
        }

        public bool IsVisibleTxt
        {
            get { return this.IsVisible; }
            set { SetValue(ref this.IsVisible, value); }
        }

        public bool IsEnabledTxt
        {
            get { return this.IsEnabled; }
            set { SetValue(ref this.IsEnabled, value); }
        }

        public bool IsRunningTxt
        {
            get { return this.IsRunning; }
            set { SetValue(ref this.IsRunning, value); }
        }

        #endregion

        #region Commands
        public ICommand RegisterCommand
        {
            get
            {
                return new RelayCommand(RegisterMethod);
            }
        }
        #endregion

        #region Methods
        private async void RegisterMethod()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter an Email.",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a Password.",
                    "Accept");
                return;
            }

            string WebAPIkey = "AIzaSyDewQerdzU0rAZIcpETYdr-jOAeeHc2RUE";

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(EmailTxt.ToString(), PasswordTxt.ToString());
                string gettoken = auth.FirebaseToken;

                await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            }

            /*
            if (string.IsNullOrEmpty(this.Name))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a Name.",
                    "Accept");
                return;
            }
            if (string.IsNullOrEmpty(this.Age))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter an Age.",
                    "Accept");
                return;
            }
            this.IsVisibleTxt = true;
            this.IsRunningTxt = true;
            this.IsEnabledTxt = false;
            var user = new UserModel
            {
                EmailField = Email,
                PasswordField = Password,
                NamesField = Name,
                AgeField = Age,
                Creation_Date = DateTime.Now,
            };
            await App.Database.SaveUserModelAsync(user);
            await Application.Current.MainPage.DisplayAlert("Successfully", "Welcome " + Name.ToString() + " to your app", "Ok");
            this.IsRunningTxt = false;
            this.IsVisibleTxt = false;
            this.IsEnabledTxt = true;
            await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
            */
        }
        #endregion

        #region Constructor
        public RegisterViewModel()
        {
            IsEnabledTxt = true;
        }
        #endregion

    }
}
