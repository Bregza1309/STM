using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Server.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace STM.Services
{
    public class StudentRepository : IStudentRepository
    {
        StmContext Context { get; set; }

        public StudentRepository(StmContext context)
        {
            this.Context = context;
        }
        public void Add(Student student , int Id)
        {
            Parent? parent = Context.Parents.Include(p => p.Students).Where(p => p.ParentId == Id).First();
            parent.Students!.Add(student);
            Context.SaveChanges();

        }

        public void Delete(int Id)
        {
            
            Student? student = Context.Students.SingleOrDefault(p => p.StudentId == Id);
            if(student != null)
            {
                var requests = Context.Requests.Where(r => r.StudentId == Id);
                Context.Requests.RemoveRange(requests);
                Context.Students.Remove(student);
                Context.SaveChanges();
            }
        }

        public List<Student> GetStudents(int parentId = 0)
        {
            if (parentId == 0)
            {
                return Context.Students.Include(s => s.Driver)
                    .Include(s => s.Parent)
                    .Include(s => s.School)
                    .OrderBy(s => s.Lastname)
                    .ThenBy(s => s.Firstname).ToList();
            }
            return Context.Students.Include(s => s.Parent).Include(s => s.Driver).Include( s => s.School).Where(s => s.Parent!.ParentId == parentId).ToList();
        }

        public void Update(Student student, bool requestMode)
        {
            if (requestMode)
            {
                var requests = Context.Requests.Where(r => r.StudentId == student.StudentId);
                Context.Requests.RemoveRange(requests);
                Context.SaveChanges();
            }
            Context.ChangeTracker.Clear();
            Context.Students.Update(student);
            Context.SaveChanges();
        }
       
        public Student GetStudent(int id)
        {
            Student student = Context.Students.Include(s => s.Driver).Include(s => s.Parent).SingleOrDefault(s => s.StudentId == id) ?? new Student();
            return student;
        }

        public bool IsRegistered(Student student)
        {
            Student? existing = Context.Students.SingleOrDefault(s => s.StudentId == student.StudentId);
            return existing != null;
        }

        public List<Student> Search(string key)
        {
            var students = GetAll();
            try
            {
                Regex rx = new($@"{key.ToLower()}");
               
                return students.Where(s => rx.IsMatch(s.Firstname.ToLower()) || rx.IsMatch(s.Lastname.ToLower()) || rx.IsMatch(s.School!.Name.ToLower()) || rx.IsMatch(s.Gender.ToLower())).ToList();
            }
            catch (Exception)
            {
                return students;
            }
        }

        public List<Student> GetAll()
        {
            return Context.Students.Include(s => s.Parent).Include(s => s.Driver).Include(s => s.School).ToList();
        }
    }
}
