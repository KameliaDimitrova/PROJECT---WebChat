using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChat.Models.Chat
{
    public class Message
    {
        public int Id { get; set; }
        public string FromUserID { get; set; }
        public string FromUserName { get; set; }
        public string ToUserID { get; set; }
        public string ToUserName { get; set; }
        public string Text { get; set; }
        public string ConnectionId { get; set; }
        public bool IsPrivate { get; set; }
    }
}
