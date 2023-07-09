using DataAccess.Repositories.UserRepositories;
using Domain.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.ViewModels;
using WebFramework.Api;
using WebFramework.Filters;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiResultFilterAttribute]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ApiResult<List<User>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);

            if (users is null) return NotFound();

            return users;
        }

        [HttpGet("{id:int}")]
        public async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);

            if (user is null) return NotFound();

            return user;
        }

        [HttpPost]
        public async Task<ApiResult> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber,
            };
            await _userRepository.AddAsync(user, userDto.Password, cancellationToken);

            return Ok();
        }

        [HttpPut]
        public async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            var oldUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            oldUser.PhoneNumber = user.PhoneNumber;
            oldUser.UserName = user.UserName;
            oldUser.Email = user.Email;
            oldUser.FirstName = user.FirstName;
            oldUser.LastName = user.LastName;
            oldUser.IsActive = user.IsActive;
            oldUser.PasswordHash = user.PasswordHash;

            await _userRepository.UpdateAsync(oldUser, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        public async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            await _userRepository.DeleteAsync(user, cancellationToken);

            return Ok();
        }
    }
}
