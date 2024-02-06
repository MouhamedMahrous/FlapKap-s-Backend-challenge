using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using VendingMachineAPI.Business;
using VendingMachineAPI.Models;

namespace VendingMachineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MachineOperationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public MachineOperationsController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("deposit/{cents}")]
        [Authorize(Roles = "buyer")]
        public async Task<IActionResult> Deposit(int cents)
        {
            Log.Information("The amount of money that have been deposited = {@cents} at {@time}", cents, DateTime.Now);

            if (cents != 5 && cents != 10 && cents != 20 && cents != 50 && cents != 100)
                return BadRequest("The amount must be in [5, 10, 20, 50, 100] cents");

            var currentBuyerName = User?.Identity?.Name;
            var currentBuyer = await _userManager.FindByNameAsync(currentBuyerName);
            if (currentBuyer == null)
                return BadRequest();

            currentBuyer.Deposit += cents;
            await _userManager.UpdateAsync(currentBuyer);

            return Ok("Successfull Operation");
        }

        [HttpGet]
        [Route("buy/{prdid}/{amount}")]
        [Authorize(Roles = "buyer")]
        public async Task<IActionResult> Buy(int prdid, int amount) 
        { 
            var prd = await _unitOfWork.Products.GetByIdAsync(prdid);
            if (prd == null)
                return NotFound();

            var currentBuyerName = User?.Identity?.Name;
            var currentBuyer = await _userManager.FindByNameAsync(currentBuyerName);
            if (currentBuyer == null)
                return BadRequest();

            var totalCost = prd.Cost * amount;

            if (totalCost > currentBuyer.Deposit)
                return BadRequest("You Don't have enough money");

            Log.Information("The of money before buying = {@money} at {@time}", currentBuyer.Deposit, DateTime.Now);

            currentBuyer.Deposit -= totalCost;
            int change = CalculateChange(currentBuyer.Deposit);
            currentBuyer.Deposit -= change;
            await _userManager.UpdateAsync(currentBuyer);

            Log.Information("The of money After buying = {@money} at {@time}", currentBuyer.Deposit, DateTime.Now);
            return Ok(new
            {
                TotalCost = totalCost,
                ProductName = prd.ProductName,
                AmountOfProducts = amount,
                Change = change
            });
        }

        [HttpPost]
        [Route("reset")]
        [Authorize(Roles = "buyer")]
        public async Task<IActionResult> Reset()
        {
            var currentBuyerName = User?.Identity?.Name;
            var currentBuyer = await _userManager.FindByNameAsync(currentBuyerName);
            if (currentBuyer == null)
                return BadRequest();

            currentBuyer.Deposit = 0;
            await _userManager.UpdateAsync(currentBuyer);
            return Ok("Your deposit has been reset successfully");
        }


        private int CalculateChange(decimal remain)
        {
            int change = 0;
            while (remain > 0)
            {
                if ((remain - 100) >= 0) { remain -= 100; change += 100; }
                else if ((remain - 50) >= 0) { remain -= 50; change += 50; }
                else if ((remain - 20) >= 0) { remain -= 20; change += 20; }
                else if ((remain - 10) >= 0) { remain -= 10; change += 10; }
                else if ((remain - 5) >= 0) { remain -= 5; change += 5; }
                else { break; }
            }
            return change;
        }
    }
}
