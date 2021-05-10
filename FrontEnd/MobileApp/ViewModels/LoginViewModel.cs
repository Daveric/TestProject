using System;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Windows.Input;
using Firebase.Auth;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace MobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Attribute
        public string Email;
        public string Password;
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

        public bool IsRunningTxt
        {
            get { return this.IsRunning; }
            set { SetValue(ref this.IsRunning, value); }
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

        #endregion

        #region Commands
        public ICommand LoginCommand => new RelayCommand(LoginMethod);

        #endregion

        #region Methods


        public async void LoginMethod()
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


            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(EmailTxt.ToString(), PasswordTxt.ToString());
                var uid = auth.User.LocalId;

                Preferences.Set("MyFirebaseRefreshToken", uid);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Invalid useremail or Password", "OK");
            }

            this.IsVisibleTxt = true;
            this.IsRunningTxt = true;
            this.IsEnabledTxt = false;

            await Task.Delay(20);

            /*
            List<UserModel> e = App.Database.GetUsersValidate(Email, Password).Result;
            if (e.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                  "Error",
                  "Email or Password Incorrect.",
                  "Accept");
                this.IsRunningTxt = false;
                this.IsVisibleTxt = false;
                this.IsEnabledTxt = true;
            }
            else if (e.Count > 0)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ContainerTabbedPage());
                this.IsRunningTxt = false;
                this.IsVisibleTxt = false;
                this.IsEnabledTxt = true;
            }
            */

            this.IsRunningTxt = false;
            this.IsVisibleTxt = false;
            this.IsEnabledTxt = true;

        }

        public async void ResetPasswordEmail()
        {
            var WebAPIkey = "AIzaSyBFwirWkzuv9RZX9lmhHFKqas9bYLHjwCE";

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
                await authProvider.SendPasswordResetEmailAsync(Email);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region Constructor
        public LoginViewModel()
        {
            this.IsEnabledTxt = true;
        }
        #endregion
    }
}
