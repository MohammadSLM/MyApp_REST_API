using Domain.User;

namespace Services.JwtServices
{
    public interface IJwtService
    {
        Task<string> GenerateAsync(User user);
    }
}