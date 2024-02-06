using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VendingMachineAPI.Business;
using VendingMachineAPI.Models;
using VendingMachineAPI.Models.DTOs;

namespace VendingMachineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductsController (IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _unitOfWork.Products.AllAsync());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Add(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product();
                product.ProductName = productDto.ProductName;
                product.Cost = productDto.Cost;
                product.AmountAvailabe = productDto.AmountAvailable;

                var currentSellerName = User?.Identity?.Name;
                var currentSeller = await _userManager.FindByNameAsync(currentSellerName);
                if (currentSeller == null)
                    return BadRequest();

                product.Seller = currentSeller;

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return Ok("The product has successfuly added");
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            var currentSellerName = User?.Identity?.Name;
            var currentSeller = await _userManager.FindByNameAsync(currentSellerName);
            if (currentSeller == null)
                return BadRequest();

            if(product.Seller.Id == currentSeller.Id)
            {
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.CompleteAsync();
                return Ok("The Product has been deleted succefully ");
            }

            return Unauthorized();

        }

        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Update(ProductDto productDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productDto.ProductId);
            if (product == null)
                return NotFound();
            var currentSellerName = User?.Identity?.Name;
            var currentSeller = await _userManager.FindByNameAsync(currentSellerName);
            if (currentSeller == null)
                return BadRequest();

            if (product.Seller.Id == currentSeller.Id)
            {
                product.ProductName = productDto.ProductName;
                product.Cost = productDto.Cost;
                product.AmountAvailabe = productDto.AmountAvailable;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
                return Ok("The product has been updated successfully");
            }

            return Unauthorized();
        }
    }
}
