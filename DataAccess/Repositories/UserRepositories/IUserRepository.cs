using Domain.User;

namespace DataAccess.Repositories.UserRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindByPhoneNumberAsync(string phonenumber, CancellationToken cancellationToken);
        Task AddAsync(User user, string password, CancellationToken cancellationToken);
    }
}