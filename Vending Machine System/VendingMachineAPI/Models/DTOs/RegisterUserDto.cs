using System.ComponentModel.DataAnnotations;

namespace VendingMachineAPI.Models.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [AllowedValues(["seller", "buyer"])]
        public string Role { get; set; }
    }
}
