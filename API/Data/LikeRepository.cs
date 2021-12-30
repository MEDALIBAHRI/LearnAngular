using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.IServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public LikeRepository(DataContext dataContext, IMapper mapper)
        {
            this._mapper = mapper;
            this._dataContext = dataContext;
            
        }
        public async Task<PagedList<LikeDto>> GetLikes(LikesParams likesParams)
        {
            var users = _dataContext.Users.OrderBy(x=>x.UserName).AsQueryable();
            var likes =_dataContext.Likes.AsQueryable();

            if(likesParams.Predicate == "Liked")
            {
                likes = likes.Where(x=>x.SourceUserId == likesParams.UserId);
                users = likes.Select(x=> x.LikedUser);
            }

            if(likesParams.Predicate == "LikedBy")
            {
                likes = likes.Where(x=>x.LikedUserId == likesParams.UserId);
                users = likes.Select(x=> x.SourceUser);
            }

            var result =  users.ProjectTo<LikeDto>(_mapper.ConfigurationProvider).AsQueryable();
            return await PagedList<LikeDto>.CreateAsync(result, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
           return await _dataContext.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _dataContext.Users
                        .Include(x=>x.LikedUsers)
                        .FirstOrDefaultAsync(x=>x.Id == userId);
        }
    }
}