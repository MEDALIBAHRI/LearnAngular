using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.IServices
{
    public interface IMessageRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        void Add(Messages message);
         void Delete(Messages message);
         
         Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername);
         Task<Messages> GetMessage(int id);
         Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
         Task<bool> SaveAllAsync();
    }
}