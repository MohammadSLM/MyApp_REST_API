using DataAccess.Repositories.UserRepositories;
using Domain.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<List<User>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
            return users;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            return Ok(user);
        }

        [HttpPost]
        public async Task Create(User user, CancellationToken cancellationToken)
        {
            await _userRepository.AddAsync(user, cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, User user, CancellationToken cancellationToken)
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

            return Ok(oldUser);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            await _userRepository.DeleteAsync(user, cancellationToken);

            return Ok();
        }
    }
}
