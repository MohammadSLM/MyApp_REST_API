using Common.Utilities;
using Core.Exceptions;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.UserRepositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {

        }

        public Task<User> GetByUserAndPass(string username, string password, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(password);
            return Table.Where(p => p.UserName == username && p.PasswordHash == passwordHash).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<User> FindByPhoneNumberAsync(string phonenumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(phonenumber))
            {
                throw new ArgumentException($"'{nameof(phonenumber)}' نمیتواند خالی باشد.", nameof(phonenumber));
            }

            return Table.Where(a => a.PhoneNumber.Equals(phonenumber, StringComparison.Ordinal)).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
        {
            var exists = await TableNoTracking.AnyAsync(a => a.UserName.Equals(user.UserName));
            if (exists)
                throw new BadRequestException("نام کاربری تکراری است.");

            var passwordHash = SecurityHelper.GetSha256Hash(password);
            user.PasswordHash = passwordHash;
            await AddAsync(user, cancellationToken);
        }
    }
}
