using System.ComponentModel.DataAnnotations;

namespace VendingMachineAPI.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Cost { get; set; }
        public int AmountAvailabe { get; set; }

        [Required]
        public virtual ApplicationUser Seller { get; set; } // navigational property
    }
}
