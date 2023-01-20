using System.Threading.Tasks;
using Uni.Scan.Transfer.Requests.Mail;

namespace Uni.Scan.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}