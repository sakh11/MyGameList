using System.Threading.Tasks;

namespace MyGameList.Services
{
    public interface IActivityLogger
    {
        Task LogAsync(string userId, string description);
    }
}
