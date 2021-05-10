using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileApp.Services;
using Xamarin.Forms;

namespace MobileApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            var scanner = DependencyService.Get<IQrScanningService>();
            var result = await scanner.ScanAsync();
            if (result != null)
            {
                txtBarcode.Text = result;
            }
        }
    }
}
