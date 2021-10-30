using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepoistory;
        public AccountController(IUserRepository userRepository, ITokenService tokenService) 
        {
            this._userRepoistory = userRepository;
            this._tokenService = tokenService;
            
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
        {
            if(await UserNameExist(registerDTO.Username)) return BadRequest("Username already exist");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };
            await this._userRepoistory.Add(user);
            await this._userRepoistory.SaveAllAsync();
            return new UserDto{Username = user.UserName, Token = _tokenService.CreateToken(user)};
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user =  this._userRepoistory.GetUsersAsync().Result
                      .SingleOrDefault(x=>x.UserName == loginDto.Username);
            if(user == null) return Unauthorized("Invalid userName");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i=0; i<ComputeHash.Length; i++)
            {
                if(ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }          
            return new UserDto{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url};
        }
        private async Task<bool> UserNameExist(string userName)
        {
         return await this._userRepoistory.GetUserByUsernameAsync(userName) != null;
        }
    
    }
}