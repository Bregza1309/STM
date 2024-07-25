using StudentTransportManagement_STM_.Server.Context;
using StudentTransportManagement_STM_.Shared.DataModels;
using System.Text.RegularExpressions;
namespace STM.Services
{
    public class SchoolRepository
    {
        StmContext Context { get; set; }
        public SchoolRepository(StmContext context)
        {
                this.Context = context;
        }
        public List<School> GetAll() => 
            Context.Schools.OrderBy(s => s.Name).ToList();

        public void Add(School school)
        {
            Context.Schools.Add(school);
            Context.SaveChanges();  
        }
        public List<School> Search(string key)
        {
            try
            {
                Regex rx = new Regex($@"{key.ToLower()}");
                var schools = Context.Schools.ToList();
                return schools.Where(s => rx.IsMatch(s.Name.ToLower()) || rx.IsMatch(s.City.ToLower())  || rx.IsMatch(s.Provice.ToLower())).ToList();
            }
            catch(RegexParseException)
            {
                return new List<School>(); 
            }
        }

    }
}
