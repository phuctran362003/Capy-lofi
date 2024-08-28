using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Service.Interfaces;

public class ChatHub : Hub
{
    private readonly IChatRoomService _chatRoomService;
    private readonly IMessageService _messageService;
    private readonly IChatInvitationService _chatInvitationService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IChatRoomService chatRoomService,
        IMessageService messageService,
        IChatInvitationService chatInvitationService,
        ILogger<ChatHub> logger)
    {
        _chatRoomService = chatRoomService;
        _messageService = messageService;
        _chatInvitationService = chatInvitationService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            _logger.LogInformation("User connected: {ConnectionId}", Context.ConnectionId);

            var globalChatRoom = await _chatRoomService.GetChatRoomByIdAsync(1);

            if (globalChatRoom != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, globalChatRoom.Name);
                var recentMessages = await _messageService.GetRecentMessagesByChatRoomIdAsync(globalChatRoom.Id);

                await Clients.Caller.SendAsync("LoadRecentMessages", recentMessages);
            }


            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in OnConnectedAsync for ConnectionId: {ConnectionId}", Context.ConnectionId);
            throw;
        }
    }
    public async Task SendMessage(string messageContent)
    {
        try
        {
            _logger.LogInformation("Sending message to room {RoomId} from ConnectionId: {ConnectionId}", 1, Context.ConnectionId);

            var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(1);

            if (chatRoom != null)
            {
                var userId = 1;

                if (userId == null)
                {
                    _logger.LogWarning("User is not authenticated. ConnectionId: {ConnectionId}", Context.ConnectionId);
                    await Clients.Caller.SendAsync("ErrorMessage", "You must be logged in to send messages.");
                    return;
                }

                var messageEntity = new Message
                {
                    ChatRoomId = chatRoom.Id,
                    UserId = 1,
                    Content = messageContent
                };
                var message = await _messageService.SendMessageAsync(messageEntity);

                await Clients.Group(chatRoom.Name).SendAsync("ReceiveMessage", message);
                _logger.LogInformation("Message sent to room {RoomId} by user {UserId}", 1, userId);
            }
            else
            {
                _logger.LogWarning("Chat room {RoomId} not found. ConnectionId: {ConnectionId}", 1, Context.ConnectionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending a message in room {RoomId} for ConnectionId: {ConnectionId}", 1, Context.ConnectionId);
            throw;
        }
    }

    //public async Task SendMessage(string roomId, string messageContent)
    //{
    //    try
    //    {
    //        _logger.LogInformation("Sending message to room {RoomId} from ConnectionId: {ConnectionId}", roomId, Context.ConnectionId);

    //        var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(int.Parse(roomId));

    //        if (chatRoom != null)
    //        {
    //            var userId = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    //            if (userId == null)
    //            {
    //                _logger.LogWarning("User is not authenticated. ConnectionId: {ConnectionId}", Context.ConnectionId);
    //                await Clients.Caller.SendAsync("ErrorMessage", "You must be logged in to send messages.");
    //                return;
    //            }

    //            var messageEntity = new Message
    //            {
    //                ChatRoomId = chatRoom.Id,
    //                UserId = int.Parse(userId),
    //                Content = messageContent
    //            };
    //            var message = await _messageService.SendMessageAsync(messageEntity);

    //            await Clients.Group(chatRoom.Name).SendAsync("ReceiveMessage", message);
    //            _logger.LogInformation("Message sent to room {RoomId} by user {UserId}", roomId, userId);
    //        }
    //        else
    //        {
    //            _logger.LogWarning("Chat room {RoomId} not found. ConnectionId: {ConnectionId}", roomId, Context.ConnectionId);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "An error occurred while sending a message in room {RoomId} for ConnectionId: {ConnectionId}", roomId, Context.ConnectionId);
    //        throw;
    //    }
    //}

    public async Task LeaveRoom(string roomId)
    {
        try
        {
            _logger.LogInformation("Leaving room {RoomId} for ConnectionId: {ConnectionId}", roomId, Context.ConnectionId);

            var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(int.Parse(roomId));

            if (chatRoom != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoom.Name);
                await Clients.Group(chatRoom.Name).SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name} has left the chat.");
                _logger.LogInformation("User {UserName} left room {RoomId}", Context.User.Identity.Name, roomId);
            }
            else
            {
                _logger.LogWarning("Chat room {RoomId} not found while attempting to leave. ConnectionId: {ConnectionId}", roomId, Context.ConnectionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while leaving room {RoomId} for ConnectionId: {ConnectionId}", roomId, Context.ConnectionId);
            throw;
        }
    }
}
