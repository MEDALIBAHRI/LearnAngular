using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.IServices;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly PresenceTracker _presenceTracker;
        public IHubContext<PresenceHub> _presenceHub;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper,
        
         IUserRepository userRepository, PresenceTracker presenceTracker,IHubContext< PresenceHub> presenceHub)
        {
            
            this._presenceTracker = presenceTracker;
            this._presenceHub = presenceHub;
            this._userRepository = userRepository;
            this._mapper = mapper;
            this._messageRepository = messageRepository;
            
        }

        public override async Task OnConnectedAsync()
        {
            var httpContect = Context.GetHttpContext();
            var currentUsername = Context.User.GetUsername();
            var other = httpContect.Request.Query["user"].ToString();
            string groupName = GetGroupName(currentUsername, other);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
           var group = await AddToGroup(groupName);
           
           await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageRepository.GetMessagesThread(currentUsername, other);
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
            
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string currentUsername, string other)
        {
           return string.CompareOrdinal(currentUsername, other) < 0 ?
                 $"{currentUsername}-{other}" :$"{other}-{$"{currentUsername}"}";
        }

          public async Task SendMessage(CreateMessageDto messageDto)
       {
           var username = Context.User.GetUsername();

           if(username == messageDto.RecipientUsername.ToLower())
           throw new HubException("Cannot send message to yourself");

           var sender = await _userRepository.GetUserByUsernameAsync(username);
           var recepient = await _userRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);

           if(recepient == null)
            throw new HubException("Recepient not exist");

            var message = new Messages{
                 Sender = sender,
                 SenderId = sender.Id,
                 SenderUserName = sender.UserName,
                 Recipient = recepient,
                 RecipientId = recepient.Id,
                 RecipientUserName = recepient.UserName,
                 Content = messageDto.Content
            };

             string groupName = GetGroupName(username, messageDto.RecipientUsername);
             var group = await _messageRepository.GetMessageGroup(groupName);
             if(group.Conenctions.Any(x=>x.Username == messageDto.RecipientUsername))
             {
                 message.DateRead = System.DateTime.UtcNow;
             }
             else   
             {
                 var connections = await _presenceTracker.GetConnectionsForUser(messageDto.RecipientUsername);
                 if(connections != null){
                     await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new{
                         username = sender.UserName, knownAs = sender.KnownAs });
                 }
             }
              _messageRepository.Add(message);
            if(await _messageRepository.SaveAllAsync())
              { 
                  await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
              }

       }

       public async Task<Group> AddToGroup(string groupName)
       {
           var group = await _messageRepository.GetMessageGroup(groupName);
          
           var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
           if(group == null)
           {
               group = new Group(groupName);
               _messageRepository.AddGroup(group);
           }

           group.Conenctions.Add(connection);

           if( await _messageRepository.SaveAllAsync())
            return group;

            throw new HubException("Failed to join group");
       }

       private async Task<Group> RemoveFromMessageGroup()
       {
           var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
           var connection = group.Conenctions.FirstOrDefault(x=>x.ConnectionId == Context.ConnectionId);
           _messageRepository.RemoveConnection(connection);
          if( await _messageRepository.SaveAllAsync())
            return group;

          throw new HubException("Failed to remove from group");
       }
    }
}