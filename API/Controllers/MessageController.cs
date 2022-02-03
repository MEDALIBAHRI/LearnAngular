using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.IServices;
using Microsoft.AspNetCore.Mvc;
using API.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using API.Extensions;
using AutoMapper;
using System;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository,IMessageRepository messageRepository, IMapper mapper) 
        {
            this._userRepository = userRepository;
            this._mapper = mapper;
            this._messageRepository = messageRepository;
        }

       [HttpGet]
       public async Task<ActionResult<PagedList<MessageDto>>> GetMessages([FromQuery]MessageParams messageParams)
       {
           messageParams.Username = User.GetUsername();
           var messages = await _messageRepository.GetMessagesForUser(messageParams);
           Response.AddPaginationHeader(messageParams.PageNumber, messages.PageSize, messages.TotalCount, messages.TotalPages);
           return Ok(messages);
       }
       
       [HttpGet("thread/{username}")]
       public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
       {
           var currentUsername = User.GetUsername();
           var recipient = await _userRepository.GetUserByUsernameAsync(username);

           if(recipient == null)
           return BadRequest("Recipient not defined");
 
           var messages = await _messageRepository.GetMessagesThread(currentUsername, username);
           return Ok(messages);
       }

       [HttpPost]
       public async Task<ActionResult<CreateMessageDto>> CreateMessage(CreateMessageDto messageDto)
       {
           var username = User.GetUsername();

           if(username == messageDto.RecipientUsername.ToLower())
           return BadRequest("Cannot send message to yourself");

           var sender = await _userRepository.GetUserByUsernameAsync(username);
           var recepient = await _userRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);

           if(recepient == null)
            return BadRequest("Recepient not exist");

            var message = new Messages{
                 Sender = sender,
                 SenderId = sender.Id,
                 SenderUserName = sender.UserName,
                 Recipient = recepient,
                 RecipientId = recepient.Id,
                 RecipientUserName = recepient.UserName,
                 Content = messageDto.Content
            };

            _messageRepository.Add(message);

            if(await _messageRepository.SaveAllAsync())
              return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
       }
       
       [HttpDelete("{id}")]
       public async Task<ActionResult> DeleteMessage( int id)
       {
         var username = User.GetUsername();

         var message = await _messageRepository.GetMessage(id);

         if(message?.SenderUserName != username && message?.RecipientUserName != username)
          return Unauthorized();

          if(message?.SenderUserName == username)
          message.SenderDeleted = true;

          if(message?.RecipientUserName == username)
          message.ReceiverDeleted = true;

          if(message.SenderDeleted && message.ReceiverDeleted)
          _messageRepository.Delete(message);

          if(await _messageRepository.SaveAllAsync()) return Ok();

          return BadRequest("Problem on delete message");
       }

        public override bool Equals(object obj)
        {
            return obj is MessagesController controller &&
                   EqualityComparer<IUserRepository>.Default.Equals(_userRepository, controller._userRepository);
        }
    }
}