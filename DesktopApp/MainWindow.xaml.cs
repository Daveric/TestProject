using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using QRCoder;
using RestSharp;
using RestSharp.Serialization.Json;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ApplicationName = "Cambium1";
        private static readonly RestClient Client = new RestClient(new Uri("https://localhost:44315/api/"));

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ShowQrCode_OnClick(object sender, RoutedEventArgs e)
        {
            InfoText.Text += "Status: Guid Loaded\n";
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(await Load_Guid(), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeAsPng = qrCode.GetGraphic(20);
            QrCodeImage.Source = ToImage(qrCodeAsPng);
            InfoText.Text += await GetAccessValueAsync();
        }
        
        private static async Task<string> GetAccessValueAsync()
        {
            var request = new RestRequest("Applications/GetAccess", Method.GET) {Timeout = 5000};
            request.AddQueryParameter("name", ApplicationName);
            var response = await Client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                return "Request parameters incorrect...\n";
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Serve is unavailable to answer...\n";
            }
            var jsonSerializer = new JsonSerializer();
            return jsonSerializer.Deserialize<bool>(response) ? "Application Unlocked\n" : "User has not the permissions\n";
        } 

        private static async Task<string> Load_Guid()
        {
            try
            {
                var request = new RestRequest("Applications/GetAppId", Method.GET) { Timeout = 3000 };
                request.AddQueryParameter("name", ApplicationName);
                var response = await Client.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return string.Empty;
                var jsonSerializer = new JsonSerializer();
                return jsonSerializer.Deserialize<Guid>(response).ToString();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }
        
        private static BitmapImage ToImage(byte[] array)
        {
            using var ms = new MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; 
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Client.Timeout = 10000;
        }
    }
}
