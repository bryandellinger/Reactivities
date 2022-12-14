using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Domain;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;

        public AccountController(
            UserManager<AppUser> userManager,
             SignInManager<AppUser> signInManager,
             TokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {

            var user = await _userManager.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.Email == loginDTO.Email);

            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _signInManager.CheckPasswordSignInAsync(
                user, loginDTO.Password, false);

            if (result.Succeeded)
            {
               return CreateUserObject(user);
            }

            return Unauthorized();

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){
         if (await _userManager.Users.AnyAsync(x => x.Email == registerDTO.Email)){
            ModelState.AddModelError("email", "Email taken");
               return ValidationProblem();
         }
          if (await _userManager.Users.AnyAsync(x => x.UserName == registerDTO.UserName)){
            ModelState.AddModelError("userName", "User name taken");
            return ValidationProblem();
         }

         var user = new AppUser{
            DisplayName = registerDTO.DisplayName,
            Email = registerDTO.Email,
            UserName = registerDTO.UserName

         };

         var result = await _userManager.CreateAsync(user, registerDTO.Password);
        if (result.Succeeded)
        {
            return CreateUserObject(user);
        }
        return BadRequest("problem registering user");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetCurrentUser() =>
           CreateUserObject(
            await _userManager.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(
                x => x.Email == User.FindFirstValue(ClaimTypes.Email)));
        

        private UserDTO CreateUserObject(AppUser user){
              return new UserDTO{
                DisplayName = user.DisplayName, 
                Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Token = _tokenService.CreateToken(user),
                UserName = user.UserName
            };
        }
    }
}