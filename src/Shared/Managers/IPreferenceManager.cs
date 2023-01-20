using Uni.Scan.Shared.Settings;
using System.Threading.Tasks;
using Uni.Scan.Shared.Wrapper;

namespace Uni.Scan.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}