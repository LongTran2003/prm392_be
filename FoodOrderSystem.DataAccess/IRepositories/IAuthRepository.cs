namespace FoodOrderSystem.DataAccess.IRepositories
{
    public interface IAuthRepository
    {
        Task<bool> DoesPhoneNumberExistAsync(string phoneNumber);
        Task<bool> DoesEmailExistAsync(string email);
        Task<bool> DoesUserNameExistAsync(string userName);
    }
}
