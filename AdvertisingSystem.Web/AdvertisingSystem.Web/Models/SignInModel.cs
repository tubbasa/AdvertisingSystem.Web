using System.ComponentModel.DataAnnotations;

namespace AdvertisingSystem.Web.Models
{
    public class SignInModel
    {
        [Required(ErrorMessage = "Email is required for login")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required for login")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}