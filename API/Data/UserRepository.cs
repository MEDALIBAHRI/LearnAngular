using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.IServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
            
        }

        public async Task<AppUser> Add(AppUser user)
        {
            this._context.Entry(user).State = EntityState.Added;
            return user;
        }

        public async Task<PagedList<MemberDTO>> GetAllMemberAsync(UserParams userParams)
        {
           var users = this._context.Users.Where(x=>x.UserName != userParams.CurrentUserName
                                                    && x.Gender == userParams.Gender);
           var minDob = DateTime.Now.AddYears( -userParams.MaxAge - 1);
           var maxDob = DateTime.Now.AddYears( -userParams.MinAge );
           
           users = users.Where(x=> x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
           users = userParams.OrderBy switch
           {
               "created"=> users.OrderByDescending(x=>x.Created),
               _ => users.OrderByDescending(x=>x.LastActive)
           };
           return await PagedList<MemberDTO>.CreateAsync(users
           .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
           userParams.PageNumber, userParams.PageSize);
        }

        public Task<MemberDTO> GetMemberAsync(string username)
        {
          return this._context.Users
           .Where(x=>x.UserName == username)
           .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
           .SingleOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await this._context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await this._context.Users
                        .Include(p=> p.Photos)
                        .SingleOrDefaultAsync(x=>x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await this._context.Users
                        .Include(p=>p.Photos)
                        .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
           return await this._context.SaveChangesAsync() >0;
        }

        public void Update(AppUser user)
        {
            this._context.Entry(user).State = EntityState.Modified;
        }
    }
}