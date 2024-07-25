using StudentTransportManagement_STM_.Shared.DataModels;
namespace STM.Services
{
    public interface IStudentRepository
    {
        void Add(Student student , int Id);
        void Update(Student student , bool requestMode = false);
        void Delete(int Id);
        List<Student> Search(string key);
        List<Student> GetAll();
        List<Student> GetStudents(int parentId);
        Student GetStudent(int id);
        bool IsRegistered(Student student);
        
    }
}
