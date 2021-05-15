
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Models;
using Menu = Common.Models.Menu;

namespace MobileApp.ViewModels
{
    public class MainViewModel
    {
        private static MainViewModel _instance;

        public User User { get; set; }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        public TokenResponse Token { get; set; }

        public string UserEmail { get; set; }

        public string UserPassword { get; set; }

        public LoginViewModel Login { get; set; }

        public RegisterViewModel Register { get; set; }

        public RememberPasswordViewModel RememberPassword { get; set; }

        public ProfileViewModel Profile { get; set; }

        public ChangePasswordViewModel ChangePassword { get; set; }

        public ScanViewModel Scan { get; set; }

        public MainViewModel()
        {
            _instance = this;
            LoadMenus();
        }

        private void LoadMenus()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "ic_info",
                    PageName = "AboutPage",
                    Title = "About"
                },

                new Menu
                {
                    Icon = "ic_person",
                    PageName = "ProfilePage",
                    Title = "Modify User"
                },

                new Menu
                {
                    Icon = "ic_phonelink_setup",
                    PageName = "ScanQRCodePage",
                    Title = "Scan QRCode"
                },

                new Menu
                {
                    Icon = "ic_exit_to_app",
                    PageName = "LoginPage",
                    Title = "Close session"
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title
                }).ToList());
        }

        public static MainViewModel GetInstance()
        {
            return _instance ?? new MainViewModel();
        }
    }
}
