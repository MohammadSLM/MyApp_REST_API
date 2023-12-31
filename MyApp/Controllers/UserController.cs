using Core.Exceptions;
using DataAccess.Repositories.UserRepositories;
using Domain.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.ViewModels;
using Services.JwtServices;
using WebFramework.Api;
using WebFramework.Filters;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiResultFilterAttribute]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(IUserRepository userRepository, IJwtService jwtService, UserManager<User> userManager,
            RoleManager<UserRole> roleManager, SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<ApiResult<List<User>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);

            if (users is null) return NotFound();

            return users;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user2 = await _userManager.FindByIdAsync(id.ToString());
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);

            await _userManager.UpdateSecurityStampAsync(user);

            if (user is null) return NotFound();

            return user;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<String> Token(string userName, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserAndPass(userName, password, cancellationToken);
            if (user is null) throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است.");

            var jwt = await _jwtService.GenerateAsync(user);

            return jwt;
        }

        [HttpPost]
        [AllowAnonymous]
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
            var result = await _userManager.CreateAsync(user, userDto.Password);
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
