using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using API.IServices;
using AutoMapper;
using API.DTOs;

namespace API.Controllers
{
    [Authorize]
    public class UsersController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this._mapper = mapper;
            this._userRepository = userRepository;
            
        }
     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
            var users = _mapper.Map<IEnumerable<MemberDTO>>(await _userRepository.GetAllMemberAsync());
            return  Ok(users);
        }
        [HttpGet("{username}")]
         public async Task<ActionResult<MemberDTO>> GetUser(string username){
           return  Ok(_mapper.Map<MemberDTO>(await _userRepository.GetMemberAsync(username)));
        }

       
    }
}