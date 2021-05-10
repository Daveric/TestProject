using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using QRCoder;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string Id = "1B7CA586-EB7D-4BFF-BAF7-BC1D0FC67FD5";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowQrCode_OnClick(object sender, RoutedEventArgs e)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(Id, QRCodeGenerator.ECCLevel.Q);
            
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeAsPng = qrCode.GetGraphic(20);
            QrCodeImage.Source = ToImage(qrCodeAsPng);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private static BitmapImage ToImage(byte[] array)
        {
            using var ms = new MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
