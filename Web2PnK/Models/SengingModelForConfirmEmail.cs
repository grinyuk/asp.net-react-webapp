using Core.Entities;

namespace Web2PnK.Models
{
    public class SengingModelForConfirmEmail : EmailMessageBase
    {
        public string? Nickname { get; set; }
        public string? Link { get; set; }
    }
}
