using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Interfaces
{
    public interface IStoreRepository : IRepository<StoreItem>
    {
        IEnumerable<StoreItem> GetItemsByType(string type);
    }
}