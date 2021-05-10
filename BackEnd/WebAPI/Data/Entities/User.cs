using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Data.Entities
{
    public class User: IdentityUser
    {
        [Display(Name = "Display Name")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string DisplayName { get; set; }

        [Display(Name = "Phone Number")]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

        [Display(Name = "Email Confirmed")]
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }
        
        [NotMapped]
        [Display(Name = "Is Admin?")]
        public bool IsAdmin { get; set; }

    }
}
