using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<string> GetNextStudentCodeAsync();
    }
}
