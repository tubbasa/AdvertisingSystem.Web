using System.ComponentModel.DataAnnotations;

namespace AdvertisingSystem.Web.Models
{
    public class SignUpModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(6,ErrorMessage = "Pasword must be at least siz character long!")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and its confirmation do not match.")]
        public string ConfirmPassword { get; set; }
    }
}