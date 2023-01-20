using Uni.Scan.Transfer.Requests;

namespace Uni.Scan.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}