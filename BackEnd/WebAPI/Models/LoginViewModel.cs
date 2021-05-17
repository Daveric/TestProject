
namespace WebAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
