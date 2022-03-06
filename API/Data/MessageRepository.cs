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

        public void AddGroup(Group group)
        {
            _dataContext.Groups.Add(group);
        }

        public void Delete(Messages message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
           return await _dataContext.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _dataContext.Groups.Include(x=>x.Connections).Where(c=>c.Connections.Any(x=>x.ConnectionId==connectionId)).FirstOrDefaultAsync();
        }

        public async Task<Messages> GetMessage(int id)
        {
            return await _dataContext.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _dataContext.Groups.Include(x=>x.Connections).FirstOrDefaultAsync(x=>x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
          var query = _dataContext.Messages
          .OrderByDescending(m=>m.DateSent)
          .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
          .AsQueryable();

          query = messageParams.Container switch {
              "Inbox" => query.Where(x=>x.RecipientUserName == messageParams.Username && x.ReceiverDeleted == false),
              "Outbox" => query.Where(x=>x.SenderUserName == messageParams.Username && x.SenderDeleted == false),
              _ => query.Where(x=> x.RecipientUserName == messageParams.Username && x.ReceiverDeleted == false
                                 && x.DateRead == null)
          };
          return await PagedList<MessageDto>.CreateAsync(query , messageParams.PageNumber, messageParams.PageSize);
        }

        public async  Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername)
        {
             var query =   _dataContext.Messages
                .Where(m => m.Recipient.UserName == currentUsername && m.ReceiverDeleted == false
                        && m.Sender.UserName == recipientUsername
                        || m.Recipient.UserName == recipientUsername
                        && m.Sender.UserName == currentUsername
                         && m.SenderDeleted == false
                );
                 var unreadMessages = query.Where(m => m.DateRead == null
                && m.RecipientUserName == currentUsername).ToList();
              var messages = await  query.OrderBy(x=>x.DateSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

               

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                    
                }
            }

            
               return  messages;
            
        }

        public void RemoveConnection(Connection connection)
        {
            _dataContext.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await   _dataContext.SaveChangesAsync() >0;
        }
    }
}