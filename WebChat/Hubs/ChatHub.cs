using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Serialization;
using WebChat.Data;
using WebChat.Models;
using WebChat.Models.Chat;

namespace WebChat.Hubs
{

    [Authorize]
    public class ChatHub : Hub
    {
        private ApplicationDbContext dbContext;

        public ChatHub(ApplicationDbContext db)
        {
            this.dbContext = db;
        }
        static HashSet<string> CurrentConnections = new HashSet<string>();

        public async Task OnConnected()
        {
            var id = Context.ConnectionId;
            CurrentConnections.Add(id);

            await base.OnConnectedAsync();
        }



        public List<string> GetAllActiveConnections()
        {
            return CurrentConnections.ToList();
        }


        public async  Task Connect(string userId)
        {
            var senderId = this.dbContext.Users.FirstOrDefault(x => x.UserName == this.Context.User.Identity.Name).Id;

            var chatHistory = this.dbContext.Messages.Where(x => x.FromUserID == senderId && x.ToUserID == userId||
            x.ToUserID==senderId&&x.FromUserID==userId)
                .ToList();

          await Clients.Users(senderId).SendAsync("GetHistory", chatHistory);
        }

        public async Task Send(string message)
        {
            var sendMessage = new Message
            {
                ConnectionId = this.Context.ConnectionId,
                FromUserName = this.Context.User.Identity.Name,
                IsPrivate = false,
                Text = message,
                FromUserID = this.dbContext.Users.FirstOrDefault(x=>x.UserName== this.Context.User.Identity.Name).Id
            };
            this.dbContext.Messages.Add(sendMessage);
            this.dbContext.SaveChanges();
            await this.Clients.All.SendAsync("NewMessage", sendMessage);
        }

        public async Task SendPrivateMessage(string message, string reciverId)
        {

            var senderId = this.dbContext.Users.FirstOrDefault(x => x.UserName == this.Context.User.Identity.Name).Id;
            var sendMessage = new Message
            {
                ConnectionId = senderId+reciverId,
                FromUserName = this.Context.User.Identity.Name,
                ToUserName = this.dbContext.Users.FirstOrDefault(x => x.Id == reciverId).UserName,
                FromUserID = senderId,
                IsPrivate = true,
                ToUserID = reciverId,
                Text = message,
            };
            this.dbContext.Messages.Add(sendMessage);
            this.dbContext.SaveChanges();
            await this.Clients.Users(reciverId, senderId).SendAsync("NewMessage", sendMessage);
        }
    }
}
