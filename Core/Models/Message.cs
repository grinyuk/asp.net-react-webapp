using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Message
    {
        public List<MailAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailAddress>();
            To.AddRange(to.Select(x => new MailAddress(x)));
            Subject = subject;
            Content = content;
        }
    }
}
