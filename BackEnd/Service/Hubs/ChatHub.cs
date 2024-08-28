using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;

public class ChatHub : Hub
{
    private readonly IChatRoomService _chatRoomService;
    private readonly IMessageService _messageService;
    private readonly IChatInvitationService _chatInvitationService;

    public ChatHub(IChatRoomService chatRoomService, IMessageService messageService, IChatInvitationService chatInvitationService)
    {
        _chatRoomService = chatRoomService;
        _messageService = messageService;
        _chatInvitationService = chatInvitationService;
    }

    public override async Task OnConnectedAsync()
    {
        // Khi người dùng kết nối, mặc định tham gia phòng "Global"
        var globalChatRoom = await _chatRoomService.GetChatRoomByCountryCodeAsync("GLOBAL");

        if (globalChatRoom != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, globalChatRoom.Name);

            // Lấy 10 tin nhắn mới nhất từ phòng Global Chat
            var recentMessages = await _messageService.GetRecentMessagesByChatRoomIdAsync(globalChatRoom.Id);

            await Clients.Caller.SendAsync("LoadRecentMessages", recentMessages);
        }

        await base.OnConnectedAsync();
    }

    public async Task SendMessage(int roomId, string messageContent)
    {
        // Tìm ChatRoom theo tên
        var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(roomId);

        // Get UserId

        if (chatRoom != null)
        {
            var userId = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (userId == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "You must be logged in to send messages.");
                return;
            }

            var messageEntity = new Message
            {
                ChatRoomId = chatRoom.Id,
                UserId = int.Parse(userId),
                Content = messageContent
            };
            var message = await _messageService.SendMessageAsync(messageEntity);

            // Gửi tin nhắn tới tất cả các thành viên trong phòng
            await Clients.Group(chatRoom.Name).SendAsync("ReceiveMessage", message);
        }
    }

    //public async Task JoinRoomByCode(string privateCode)
    //{
    //    // Kiểm tra mã phòng riêng tư
    //    var invitation = await _chatInvitationService.ValidateInvitationAsync(privateCode);

    //    if (invitation != null)
    //    {
    //        var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(invitation.ChatRoomId);

    //        if (chatRoom != null && chatRoom.IsPrivate)
    //        {
    //            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom.Name);

    //            // Lấy 10 tin nhắn mới nhất từ phòng chat này
    //            var recentMessages = await _messageService.GetRecentMessagesByChatRoomIdAsync(chatRoom.Id, 10);

    //            await Clients.Caller.SendAsync("LoadRecentMessages", recentMessages);

    //            // Thông báo cho phòng chat rằng người dùng đã tham gia
    //            await Clients.Group(chatRoom.Name).SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name} has joined the chat.");
    //        }
    //    }
    //    else
    //    {
    //        await Clients.Caller.SendAsync("ErrorMessage", "Invalid private code or chat room does not exist.");
    //    }
    //}

    public async Task LeaveRoom(string roomId)
    {
        // Tìm phòng chat theo tên
        var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(int.Parse(roomId)); // Adjust according to how you find chat rooms

        if (chatRoom != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoom.Name);
            await Clients.Group(chatRoom.Name).SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name} has left the chat.");
        }
    }
}
