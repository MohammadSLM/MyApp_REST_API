using System.ComponentModel.DataAnnotations;

namespace MyApp.ViewModels
{
    public class UserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند تست باشد", new[] { nameof(UserName) });
            if(Password.Equals("123456"))
                yield return new ValidationResult("از رمز عبور بهتری استفاده کنید", new[] { nameof(Password) });
        }
    }
}
