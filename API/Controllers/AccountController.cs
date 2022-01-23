using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.IServices;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,
         ITokenService tokenService, IMapper mapper) 
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._tokenService = tokenService;
            this._mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
        {
            if(await UserNameExist(registerDTO.Username)) return BadRequest("Username already exist");

            var user =_mapper.Map<AppUser>(registerDTO);

            user.UserName = registerDTO.Username.ToLower();
            var result = await this._userManager.CreateAsync(user);
           
           if(!result.Succeeded) return BadRequest(result.Errors);
            
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if(!roleResult.Succeeded)
            return BadRequest(roleResult.Errors);
            
            return new UserDto{
                Username = user.UserName, 
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user =  this._userManager.Users
                      .Include(x=>x.Photos)
                      .SingleOrDefault(x=>x.UserName == loginDto.Username.ToLower());
            if(user == null) return Unauthorized("Invalid userName");
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            
            if(!result.Succeeded) return Unauthorized();
            
            

            return new UserDto{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                Gender = user.Gender,
                KnownAs = user.KnownAs};
        }
        private async Task<bool> UserNameExist(string userName)
        {
         return await this._userManager.FindByNameAsync(userName) != null;
        }
    
    }
}