using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByEmail(string email);
        IEnumerable<User> GetPotentialMatches(int userId, int limit = 10);
        bool ValidateCredentials(string email, string password);
        void UpdateLastAccess(int userId);
        void UpdateCredits(int userId, int newCreditAmount);
        void UpdateCapcoins(int userId, int newCapcoinAmount);
    }
}