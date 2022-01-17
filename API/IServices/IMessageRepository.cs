using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.IServices
{
    public interface IMessageRepository
    {
        void Add(Messages message);
         void Delete(Messages message);
         
         Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername);
         Task<Messages> GetMessage(int id);
         Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
         Task<bool> SaveAllAsync();
    }
}