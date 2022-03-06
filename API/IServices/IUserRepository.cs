using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.IServices
{
    public interface IUserRepository
    {
         void Update(AppUser user);
         Task<IEnumerable<AppUser>> GetUsersAsync();
         Task<AppUser> GetUserByIdAsync(int id);
         Task<AppUser> GetUserByUsernameAsync(string username);
         Task<MemberDTO> GetMemberAsync(string username);
         Task<PagedList<MemberDTO>> GetAllMemberAsync(UserParams userParams);
         Task<string> GetGenderByUsername(string username);
    }
}