using Core.Entities;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmail(Message message);
        Task SendEmail<T>(T model) where T : EmailMessageBase;
    }
}
