using System.Threading.Tasks;

namespace MobileApp.Services
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
