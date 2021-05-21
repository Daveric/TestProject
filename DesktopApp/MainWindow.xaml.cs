using QRCoder;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using Timer = System.Timers.Timer;

namespace DesktopApp
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private static readonly RestClient Client = new RestClient("https://appauthorization.azurewebsites.net/api");
    private static readonly string  StrGuid = Guid.NewGuid().ToString();
    private static Timer _timer;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void StartListening(object sender, RoutedEventArgs e)
    {
      _timer = new Timer (5000 );
      _timer.Elapsed += Timer_Elapsed;
      _timer.Start();
    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      Dispatcher.Invoke(() =>
      {
        InfoText.Text += GetAccessValueAsync();
      });
    }

    private static string GetAccessValueAsync()
    {
      var request = new RestRequest("/Applications/GetAccess", Method.GET) { Timeout = 3000 };
      request.AddQueryParameter("guid", StrGuid);
      var response = Client.Execute(request);
      if (!response.IsSuccessful)
      {
        return "Request parameters incorrect...\n";
      }

      if (response.StatusCode != HttpStatusCode.OK)
      {
        return "Serve is unavailable to answer...\n";
      }
      var jsonSerializer = new JsonSerializer();
      if (jsonSerializer.Deserialize<bool>(response))
      {
        _timer.Stop();
        return "Application Unlocked\n";
      }
      else
      {
        return "User has not the permissions\n";
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
      InfoText.Text += "Please Scan the QRCode\n";
      using var qrGenerator = new QRCodeGenerator();
      var qrCodeData = qrGenerator.CreateQrCode(StrGuid, QRCodeGenerator.ECCLevel.Q);
      var qrCode = new PngByteQRCode(qrCodeData);
      var qrCodeAsPng = qrCode.GetGraphic(20);
      QrCodeImage.Source = ToImage(qrCodeAsPng);

      Client.Timeout = 1000;
    }

    private void Stop(object sender, RoutedEventArgs e)
    {
      _timer.Stop();
      InfoText.Text += "Please click on Start Listening\n";
    }
  }
}
