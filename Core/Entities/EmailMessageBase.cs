using Core.Enums;

namespace Core.Entities
{
	public abstract class EmailMessageBase
	{
		public Guid UserId { get; set; }
		public string? UserEmail { get; set; }
		public string? Title { get; set; }
		public string? TemplatePath { get; set; }
        public string? RootLink { get; set; }
    }
}
