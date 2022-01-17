using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.IServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext dataContext, IMapper mapper)
        {
            this._mapper = mapper;
            this._dataContext = dataContext;
            
        }
        public void Add(Messages message)
        {
            _dataContext.Messages.Add(message);
           
        }

        public void Delete(Messages message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Messages> GetMessage(int id)
        {
            return await _dataContext.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
          var query = _dataContext.Messages.OrderByDescending(m=>m.DateSent).AsQueryable();

          query = messageParams.Container switch {
              "Inbox" => query.Where(x=>x.RecipientUserName == messageParams.Username && x.ReceiverDeleted == false),
              "Outbox" => query.Where(x=>x.SenderUserName == messageParams.Username && x.SenderDeleted == false),
              _ => query.Where(x=> x.RecipientUserName == messageParams.Username && x.ReceiverDeleted == false
                                 && x.DateRead == null)
          };
         var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
          return await PagedList<MessageDto>.CreateAsync(messages , messageParams.PageNumber, messageParams.PageSize);
        }

        public async  Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername)
        {
             var messages = await _dataContext.Messages
                .Where(m => m.Recipient.UserName == currentUsername && m.ReceiverDeleted == false
                        && m.Sender.UserName == recipientUsername
                        || m.Recipient.UserName == recipientUsername
                        && m.Sender.UserName == currentUsername && m.SenderDeleted == false
                )
                .OrderBy(m => m.DateSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.RecipientUserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                 await _dataContext.SaveChangesAsync();
            }

            return messages;
            
        }

        public async Task<bool> SaveAllAsync()
        {
            return await   _dataContext.SaveChangesAsync() >0;
        }
    }
}