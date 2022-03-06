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
        private readonly IMapper _mapper;
        private readonly PresenceTracker _presenceTracker;
        public IHubContext<PresenceHub> _presenceHub;
        private readonly IUnitOfWork _unitOfWork;
        public MessageHub(IUnitOfWork unitOfWork, IMapper mapper,
         PresenceTracker presenceTracker,IHubContext< PresenceHub> presenceHub)
        {
            this._unitOfWork = unitOfWork;
            
            this._presenceTracker = presenceTracker;
            this._presenceHub = presenceHub;
            this._mapper = mapper;
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

            var messages = await _unitOfWork.MessageRepository.GetMessagesThread(currentUsername, other);
            if(_unitOfWork.HasChanges())
              await _unitOfWork.Complete();
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

           var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
           var recepient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);

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
             var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
             if(group.Connections.Any(x=>x.Username == messageDto.RecipientUsername))
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
              _unitOfWork.MessageRepository.Add(message);
            if(await _unitOfWork.Complete())
              { 
                  await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
              }

       }

       public async Task<Group> AddToGroup(string groupName)
       {
           var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
          
           var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
           if(group == null)
           {
               group = new Group(groupName);
               _unitOfWork.MessageRepository.AddGroup(group);
           }

           group.Connections.Add(connection);

           if( await _unitOfWork.Complete())
            return group;

            throw new HubException("Failed to join group");
       }

       private async Task<Group> RemoveFromMessageGroup()
       {
           var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
           var connection = group.Connections.FirstOrDefault(x=>x.ConnectionId == Context.ConnectionId);
           _unitOfWork.MessageRepository.RemoveConnection(connection);
          if( await _unitOfWork.Complete())
            return group;

          throw new HubException("Failed to remove from group");
       }
    }
}