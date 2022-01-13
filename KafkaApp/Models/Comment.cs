using System;
using System.Collections.Generic;

#nullable disable

namespace KafkaApp.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TwitId { get; set; }
        public string Comments { get; set; }
        public DateTime Created { get; set; }

        public virtual Twittor Twit { get; set; }
        public virtual User User { get; set; }
    }
}
