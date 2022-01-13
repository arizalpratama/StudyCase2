using System;
using System.Collections.Generic;

#nullable disable

namespace KafkaApp.Models
{
    public partial class Profile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

        public virtual User User { get; set; }
    }
}
