using System.ComponentModel.DataAnnotations;

namespace VendingMachineAPI.Models.DTOs
{
    public class LoginUserDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
