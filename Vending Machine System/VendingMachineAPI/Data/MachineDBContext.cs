using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Models;

namespace VendingMachineAPI.Data
{
    public class MachineDBContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Product> Products { get; set; }
        public MachineDBContext(DbContextOptions<MachineDBContext> options) : base(options)
        {
        }
    }
}
