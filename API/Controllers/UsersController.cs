using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using API.IServices;
using AutoMapper;
using API.DTOs;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using API.Extensions;

namespace API.Controllers
{
    [Authorize]
    public class UsersController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper,
         IPhotoService photoService)
        {
            this._mapper = mapper;
            this._userRepository = userRepository;
            this._photoService = photoService;
        }
      
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams){
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            
            if(string.IsNullOrEmpty(userParams.CurrentUserName))
            {
                userParams.CurrentUserName = user.UserName;
            }

            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" :"male";
            }
            var users = await _userRepository.GetAllMemberAsync(userParams);
            Response.AddPaginationHeader(users.PageNumber, users.PageSize, 
            users.TotalCount, users.TotalPages);
            return  Ok(users);
        }

        [Authorize(Roles ="Member")]
        [HttpGet("{username}",Name = "GetUser")]
         public async Task<ActionResult<MemberDTO>> GetUser(string username){
           return  Ok(_mapper.Map<MemberDTO>(await _userRepository.GetMemberAsync(username)));
        }
         
         [HttpPut]
         public async Task<ActionResult> Update(MemberUpdateDTO member)
         {
             var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

             _mapper.Map(member, user);

             _userRepository.Update(user);
             if(await _userRepository.SaveAllAsync()) return NoContent();
             return BadRequest("Failed to update on server");
         }

         [HttpPost("add-photo")]
          public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
         {
             var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

             var result = await _photoService.AddPhotoAsync(file);
            
             if(result.Error != null) return BadRequest(result.Error.Message);

             var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
             };

            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new {username = user.UserName},_mapper.Map<PhotoDTO>(photo));
            }
             
             return BadRequest("Problem adding photo");
         }

         [HttpPut("set-main-photo/{photoId}")]
         public async Task<ActionResult> SetMainPhoto(int photoId)
         {
             var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

             var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

             if(photo.IsMain) return BadRequest("Phot is already main");

             var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain == true);
             if(currentMain != null)
             {
                 currentMain.IsMain = false;
             }

             photo.IsMain = true;
             if(await _userRepository.SaveAllAsync()) return NoContent();

             return BadRequest("Faild to set main photo");
         }

         [HttpDelete("delete-photo/{photoId}")]
         public async Task<ActionResult> DeletePhoto(int photoId)
         {
             var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

             var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

             if(photo is null) return NotFound();

             if(photo.IsMain) return BadRequest("Cannot delete main photo");

             if(photo.PublicId is not null)
             {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error is not null) return BadRequest(result.Error.Message);
             }
             
             user.Photos.Remove(photo);
             
             if(await _userRepository.SaveAllAsync()) return Ok();

             return BadRequest("Failed to delete a photo");
         }
    }
}