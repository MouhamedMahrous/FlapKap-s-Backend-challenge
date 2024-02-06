using System.ComponentModel.DataAnnotations;

namespace VendingMachineAPI.Models.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public int AmountAvailable { get; set; }

    }
}
