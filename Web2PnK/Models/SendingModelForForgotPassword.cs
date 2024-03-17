using Core.Entities;

namespace Web2PnK.Models
{
    public class SendingModelForForgotPassword : EmailMessageBase
    {
        public string? Nickname { get; set; }
        public string? Link { get; set; }
        public DateTime TokenLifeTo { get; set; }
    }
}
