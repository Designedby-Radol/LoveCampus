namespace CampusLove.Domain.Interfaces
{
    public interface IRepositoryFactory
    {
        T CreateRepository<T>() where T : class;
    }
}