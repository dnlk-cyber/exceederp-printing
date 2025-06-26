using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.InfoDesk
{
    public class ChatSession
    {
        [Key]
        public int ChatSessionId { get; set; }

        [Required]
        public string User1Id { get; set; }

        [Required]
        public string User2Id { get; set; }
    }

    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        [Required]
        public string SenderId { get; set; }
        [NotMapped]
        public string Sender { get; set; }
        [Required]
        public string ReceiverId { get; set; }
        [NotMapped]
        public string Receiver { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [Required]
        public int ChatSessionId { get; set; }
        public virtual ChatSession Session { get; set; }
    }
}
