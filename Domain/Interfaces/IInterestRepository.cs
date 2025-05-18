using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Interfaces
{
    public interface IInterestRepository : IRepository<Interest>
    {
        IEnumerable<Interest> GetInterestsByUserId(int userId);
        void SaveUserInterests(int userId, List<int> interestIds);
    }
}