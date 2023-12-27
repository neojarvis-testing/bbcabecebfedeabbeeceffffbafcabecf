using BookStoreDBFirst.Models;
using BookStoreDBFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStoreDBFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoanApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IUserService _userService;

        public AuthController(IUserService userService, LoanApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _userService = userService;
            _context = context;

        }

       [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] User user)
{
            Console.WriteLine(user.Username);
            Console.WriteLine(user.Role);
            if (user == null)
        return BadRequest("Invalid user data");

    if (user.Role == "admin" || user.Role == "user")
    {
        var identityUser = new IdentityUser
        {
            UserName = user.Username,
            // You can add other properties if needed
        };

        var result = await _userManager.CreateAsync(identityUser, user.Password);

        if (result.Succeeded)
        {
            // Assign roles to the user
            if (user.Role == "admin")
            {
                await _userManager.AddToRoleAsync(identityUser, "admin");
            }
            else if (user.Role == "user")
            {
                await _userManager.AddToRoleAsync(identityUser, "user");
            }

            return Ok("Registration successful");
        }
    }

    return BadRequest("Registration failed. Username may already exist.");
}

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        //{
        //    Console.WriteLine("sdfg"+request.Username);
        //    if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        //        return BadRequest("Invalid login request");

        //    var token = await _userService.LoginAsync(request.Username, request.Password);

        //    if (token == null)
        //        return Unauthorized("Invalid username or password");

        //    return Ok(new { Token = token });
        //}

       [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
{
    if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        return BadRequest("Invalid login request");

    var user = await _userManager.FindByNameAsync(request.Username);

    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        return Unauthorized("Invalid username or password");

    var roles = await _userManager.GetRolesAsync(user);

    // Generate the token and return it
    var token = await _userService.LoginAsync(request.Username, request.Password);

    return Ok(new { Token = token,
    UserId = user.Id,
        Username = user.UserName,
         Roles = roles });
}


    //     [Authorize(Roles = "admin")]
    //     [HttpGet("admin")]
    //     public IActionResult AdminProtected()
    //     {
    //         return Ok("This is an admin-protected endpoint.");
    //     }

    //     [Authorize(Roles = "customer")]
    //     [HttpGet("customer")]
    //     public IActionResult customerProtected()
    //     {
    //         return Ok("This is an customer-protected endpoint.");
    //     }
     }
}
