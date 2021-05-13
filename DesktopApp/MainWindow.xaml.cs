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
        private const string ApplicationName = "Cambium 2021.07";
        private static readonly RestClient Client = new RestClient(new Uri("https://localhost:44315/api/"));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowQrCode_OnClick(object sender, RoutedEventArgs e)
        {
            InfoText.Text += "Status: Guid Loaded\n";
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(Load_Guid(), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeAsPng = qrCode.GetGraphic(20);
            QrCodeImage.Source = ToImage(qrCodeAsPng);
            InfoText.Text += GetAccessValue().ToString() ?? string.Empty;
        }

        private static async Task<bool> ResponseFromApiAsync()
        {
            var result = GetAccessValue();
            return await result;


            //var request = new RestRequest("Applications/GetAccess", Method.GET) {Timeout = 5000};
            //request.AddQueryParameter("name", ApplicationName);
            //var response = Client.Execute(request);
            //if (response.StatusCode != HttpStatusCode.OK)
            //    return string.Empty;
            //var jsonSerializer = new JsonSerializer();
            //return jsonSerializer.Deserialize<bool>(response) ? "Unlocked" : "User has not the permissions";
        }

        private static async Task<bool> GetAccessValue()
        {
            var request = new RestRequest("Applications/GetAccess", Method.GET) { Timeout = 10000 };
            request.AddQueryParameter("name", ApplicationName); 
            var response = await Client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return false;

            var jsonSerializer = new JsonSerializer();
            return jsonSerializer.Deserialize<bool>(response);
        } 

        private static string Load_Guid()
        {
            var request = new RestRequest("Applications/GetAppId", Method.GET);
            request.AddQueryParameter("name", ApplicationName);
            var response = Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK) 
                return Guid.NewGuid().ToString();
            var jsonSerializer = new JsonSerializer();
            return jsonSerializer.Deserialize<Guid>(response).ToString();
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

    }
}
