using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebChat.Models;
using WebChat.Models.Chat;

namespace WebChat.ViewModel
{
    public class ChatViewModel
    {
        public IList<IdentityUser> Users { get; set; }
        public IList<Message> Messages { get; set; }
        public string SenderUsername { get; set; }
    }
}
