using System.Threading.Tasks;

namespace MobileApp.Interfaces
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
