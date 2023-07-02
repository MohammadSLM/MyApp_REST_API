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

        public Task<User> FindByPhoneNumberAsync(string phonenumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(phonenumber))
            {
                throw new ArgumentException($"'{nameof(phonenumber)}' نمیتواند خالی باشد.", nameof(phonenumber));
            }

            return base.Table.Where(a => a.PhoneNumber.Equals(phonenumber, StringComparison.Ordinal)).SingleOrDefaultAsync(cancellationToken);
        }
    }
}
