using Microsoft.AspNetCore.Identity;

namespace VendingMachineAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public decimal Deposit { get; set; } 
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
