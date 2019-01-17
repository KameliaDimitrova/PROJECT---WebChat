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


        static List<User> SignalRUsers = new List<User>();

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;

            if (SignalRUsers.Count(x => x.ConnectionId == id) == 0)
            {
                SignalRUsers.Add(new User { ConnectionId = id, UserName = userName });
            }
        }

       

        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("NewMessage", new Message
            {
                User = this.Context.User.Identity.Name,
                Text = message,
            });
        }

        public async Task SendPrivateMessage(string message, string connId)
        {
            var senderId = this.dbContext.Users.FirstOrDefault(x => x.UserName == this.Context.User.Identity.Name).Id;
            await this.Clients.Users(connId, senderId).SendAsync("NewMessage", new Message
            {
                User = this.Context.User.Identity.Name,
                Text = message,
            });

        }
    }
}
