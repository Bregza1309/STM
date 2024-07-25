using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
namespace STM.Services
{
    public interface IService<T>
    {
        Dictionary<ApplicationUser, T> Get(string? userId = null);
        void Update( T entity);
        void Create(T entity);
        Dictionary<ApplicationUser, T> Search(string key);
        
        List<Student> SearchStudents(string key, string driverId)
        {
             throw new NotImplementedException ();
        }
        void Delete(string Id);
    }
}
