using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.IServices
{
    public interface IUserRepository
    {
        Task<AppUser> Add(AppUser user);
         void Update(AppUser user);
         Task<bool> SaveAllAsync();
         Task<IEnumerable<AppUser>> GetUsersAsync();
         Task<AppUser> GetUserByIdAsync(int id);
         Task<AppUser> GetUserByUsernameAsync(string username);
         Task<MemberDTO> GetMemberAsync(string username);
         Task<PagedList<MemberDTO>> GetAllMemberAsync(UserParams userParams);
    }
}