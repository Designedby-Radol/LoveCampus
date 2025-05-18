using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Interfaces
{
    public interface IMatchRepository : IRepository<Match>
    {
        IEnumerable<Match> GetMatchesByUserId(int userId);
        bool IsMatch(int user1Id, int user2Id);
        void CreateMatch(int user1Id, int user2Id);
    }
}