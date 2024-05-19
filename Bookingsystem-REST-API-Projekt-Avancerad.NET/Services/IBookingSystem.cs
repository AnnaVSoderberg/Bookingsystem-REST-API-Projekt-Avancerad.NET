namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public interface IBookingSystem<T>  
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> GetSingle(int id);

        Task<T> Add(T newEntity);

        Task<T> Update (T entity);

        Task<T> Delete (int id);

    }
}
