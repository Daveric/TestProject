using System.Threading.Tasks;
using System.Windows.Input;
using Common.Helpers;
using Common.Models;
using Common.Services;
using GalaSoft.MvvmLight.Command;
using MobileApp.Views;
using Xamarin.Forms;
using ZXing;

namespace MobileApp.ViewModels
{
  public class ScanViewModel : BaseViewModel
  {
    public Result Result { get; set; }

    private readonly ApiService _apiService;

    private bool _isAnalyzing = true;
    public bool IsAnalyzing
    {
      get => _isAnalyzing;
      set
      {
        if (Equals(_isAnalyzing, value)) return;
        _isAnalyzing = value;
        OnPropertyChanged(nameof(IsAnalyzing));
      }
    }

    private bool _isScanning = true;
    public bool IsScanning
    {
      get => _isScanning;
      set
      {
        if (Equals(_isScanning, value)) return;
        _isScanning = value;
        OnPropertyChanged(nameof(IsScanning));
      }
    }
    
    public ICommand ScanResultCommand => new RelayCommand(ExecuteScanResultCommand);

    public ICommand ScanCommand => new RelayCommand(ExecuteScanCommand);


    public ScanViewModel()
    {
      _apiService = new ApiService();
    }

    private async void SetUserToApp()
    {
      var request = new SetUserToAppRequest()
      {
        Email = Settings.UserEmail,
        AppId = Result.Text
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
    private async void ExecuteScanCommand()
    {
      IsAnalyzing = true;
      IsScanning = true; 

      await App.Navigator.PushAsync(new ScanQRCodePage());
    }

    private void ExecuteScanResultCommand()
    {
      IsAnalyzing = false;
      IsScanning = false;

      Device.BeginInvokeOnMainThread(SetUserToApp);
    }
  }
}
