using Core.Models;

namespace Core.Entities
{
    public class AppConfig
    {
        public TimeSpan TokenLifeSpan { get; set; }
        public TimeSpan ResetPasswordTokenLifeTime { get; set; }
        public TimeSpan BearerTokenLifeSpan { get; set; }
        public TimeSpan RecalculateUserScoreLifeSpan { get; set; }
        public EmailConfiguration? EmailConfiguration { get; set; }
        public ConfigActionManager? LoginActionManager { get; set; }
        public ConfigActionManager? ConfigPasswordActionManager { get; set; }
        public ConfigActionManager? ConfigEmailActionManager { get; set; }
        public TimeSpan DefaultLockoutTimeSpan { get; set; }
        public ConfigActionManager? ConfigTaskAnswerAmountActionManager { get; set; }
        public TimeSpan CacheLifeTime { get; set; }
    }
}
