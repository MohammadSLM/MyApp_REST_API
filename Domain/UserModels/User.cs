using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public class User : IdentityUser<int> , IEntity
    {
        public User()
        {
            IsActive = true;
            FullName = FirstName + " " + LastName;
        }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        //public UserRole Role { get; set; }

    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p=> p.UserName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.PasswordHash).IsRequired();
            builder.Property(p => p.FirstName).HasMaxLength(200);
            builder.Property(p => p.LastName).HasMaxLength(200);
            builder.Property(p => p.PhoneNumber).IsRequired();
            //builder.HasOne(p => p.Role).WithMany(p => p.Users).HasForeignKey(p => p.RoleId);
        }
    }
}
