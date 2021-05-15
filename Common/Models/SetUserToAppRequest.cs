

namespace Common.Models
{
    using System.ComponentModel.DataAnnotations;
    public class SetUserToAppRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string AppId { get; set; }
    }
}
