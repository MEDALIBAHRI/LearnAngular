using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.IServices;
using Microsoft.AspNetCore.Mvc;
using API.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using API.Extensions;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepoistory;
        private readonly ILikeRepository _likeRepository;
        public LikesController(IUserRepository userRepository,
         ILikeRepository likeRepository) 
        {
            this._userRepoistory = userRepository;
            this._likeRepository = likeRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> Like(string username)
        {
           var sourceUserId = User.GetUserId();
           var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);
           var likedUser = await _userRepoistory.GetUserByUsernameAsync(username);

           if(likedUser == null) return BadRequest("User not found");

           if(sourceUserId == likedUser.Id) return BadRequest("Cannot like yourself");

           var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUser.Id);

           if(userLike != null) return BadRequest("User already liked");

           userLike = new UserLike{
               SourceUserId = sourceUserId,
               LikedUserId = likedUser.Id
           };

           sourceUser.LikedUsers.Add(userLike);

           if(await _userRepoistory.SaveAllAsync()) return Ok();

           return BadRequest("Failed to like User");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users =await _likeRepository.GetLikes(likesParams);
            Response.AddPaginationHeader(users.PageNumber, users.PageSize, 
            users.TotalCount, users.TotalPages);
            return Ok(users);
        }
        
       
    
    }
}