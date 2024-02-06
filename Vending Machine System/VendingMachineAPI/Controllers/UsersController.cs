using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VendingMachineAPI.Models;
using VendingMachineAPI.Models.DTOs;

namespace VendingMachineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase 
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public UsersController(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IConfiguration config
            ) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        // Create new account
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                // Save to database 

                ApplicationUser user = new ApplicationUser();
                user.UserName = userDto.UserName;
                IdentityResult result =  await _userManager.CreateAsync(user, userDto.Password);
                if (result.Succeeded)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(userDto.Role);
                    if (!roleExists) 
                    {
                        await _roleManager.CreateAsync(new IdentityRole(userDto.Role));
                    }
                    await _userManager.AddToRoleAsync(user, userDto.Role);
                    Log.Information("Registered User: {@user} at {@time}", user, DateTime.Now);
                    return Ok("User has been successfully added");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(var error in  result.Errors)
                    {
                        sb.Append($"{error.Description}\n");
                    }

                    return BadRequest(sb.ToString());
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                // Validate the user inputs & Create the token
                ApplicationUser user = await _userManager.FindByNameAsync(userDto.UserName);
                if (user !=  null)
                {
                    bool found = await _userManager.CheckPasswordAsync(user, userDto.Password);
                    if (found)
                    {
                        // Claims Token
                        var Claims = new List<Claim>();
                        Claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        Claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            Claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        SecurityKey securityKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
                            );

                        SigningCredentials signingCred = new SigningCredentials(
                            securityKey, SecurityAlgorithms.HmacSha256);
                        // Create Token
                        JwtSecurityToken MyToken = new JwtSecurityToken(
                            issuer: _config["Jwt:Issuer"],  // url of web api
                            audience: _config["Jwt:Audience"], // url of consumer
                            claims: Claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: signingCred
                            );
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(MyToken),
                            expiration = MyToken.ValidTo
                        });
                    }
                }
                return Unauthorized();
            }
            return BadRequest(ModelState);
        }

        // Gets the details of the currently authenticated user
        [HttpGet]
        [Route("UserDetails")]
        public async Task<IActionResult> Details()
        {
            var Username = User?.Identity?.Name;
            var user = await _userManager.FindByNameAsync(Username); 
            var Deposit = user?.Deposit;
            var Role = User?.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .FirstOrDefault();
            if (Role == "seller")
                return Ok(new { Username, Role });

            return Ok(new { Username, Deposit, Role });
        }

        // Changing the password of the currently authenticated user
        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromHeader]string oldPassword,[FromHeader] string newPassword)
        {
            var userName = User?.Identity?.Name;
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound();
            IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
            {
                return Ok("Your password has been changed successfully");
            }
            return BadRequest();
        }

        // Delete the currently authenticated user
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> Delete()
        {
            var userName = User?.Identity?.Name;
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound();
            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("Your Account Has been deleted successfuly");
            }
            return BadRequest();
        }


    }
}
