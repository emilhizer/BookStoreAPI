using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore_API.DTOs;
using BookStore_API.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers {

  [Route("api/[controller]")]
  [ApiController]

  public class UsersController : Controller {

    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILoggerService _logger;
    private readonly IConfiguration _config;

    // Initial setup of UsersController
    public UsersController(
      SignInManager<IdentityUser> signInManager,
      UserManager<IdentityUser> userManager,
      ILoggerService logger,
      IConfiguration config) {

      _signInManager = signInManager;
      _userManager = userManager;
      _logger = logger;
      _config = config;
    } // init


    // Resuble return result method
    private ObjectResult InternalError(string message) {
      _logger.LogError(message);
      return StatusCode(500, "Something wehn wrong. Please contact the administrator.");
    }

    // Get Controller Name as a String
    private string GetControllerActionNames() {
      var controller = ControllerContext.ActionDescriptor.ControllerName;
      var action = ControllerContext.ActionDescriptor.ActionName;
      return $"{controller} - {action}";
    }


    /// <summary>
    /// User Login Endpoint
    /// </summary>
    /// <param name="userDTO"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserDTO userDTO) {
      var location = GetControllerActionNames();

      try {
        var username = userDTO.Username;
        var password = userDTO.Password;
        _logger.LogInfo($"{location}: Login attempt from user: {username}");

        var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
        if (result.Succeeded) { // .Succeeded mean un/pw match was found
          _logger.LogInfo($"{location}: {username} Successfully authenticated");
          var user = await _userManager.FindByNameAsync(username);
          var tokenString = await GenerateJSONWebToken(user);
          return Ok(new { token = tokenString }); // returns json of token: tokenString
        }
        _logger.LogInfo($"{location}: {username} Not authenticated");
        return Unauthorized(userDTO);
      }
      catch (Exception ex) {
        return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
      }
    } // Login


    private async Task<string> GenerateJSONWebToken(IdentityUser user) {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
      var claims = new List<Claim> {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
      };
      var roles = await _userManager.GetRolesAsync(user);
      claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

      var token = new JwtSecurityToken(
        _config["Jwt:Issuer"],
        _config["Jwt:Issuer"],
        claims,
        null,
        expires: DateTime.Now.AddMinutes(5),
        signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    } // GenerateJSONWebToken


  } // UsersController


} // namespace
