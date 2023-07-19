using Domain.User;

namespace Services.JwtServices
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}