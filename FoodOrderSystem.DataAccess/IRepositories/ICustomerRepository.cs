using FoodOrderSystem.Models.Domains;

namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerByEmailAsync(string email, string? includeProperties = null);
        Task<Customer?> GetCustomerByPhoneNumberAsync(string phoneNumber, string? includeProperties = null);

        void Update(Customer target, Customer source);

    }
}
