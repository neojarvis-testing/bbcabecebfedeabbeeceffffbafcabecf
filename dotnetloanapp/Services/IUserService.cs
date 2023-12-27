using BookStoreDBFirst.Models;
using System.Threading.Tasks;


namespace BookStoreDBFirst.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(User user);
        Task<string> LoginAsync(string username, string password);
    }
}
