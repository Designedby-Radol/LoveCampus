using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Interfaces
{
    public interface IInteractionRepository : IRepository<Interaction>
    {
        IEnumerable<Interaction> GetInteractionsByUserId(int userId);
        bool HasLiked(int sourceUserId, int targetUserId);
        int CountDailyLikes(int userId);
    }
}