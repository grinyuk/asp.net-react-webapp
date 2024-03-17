using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using DataBase.Models;

namespace DataBase
{
    public class PnkDbContext : IdentityDbContext<PnKUser, IdentityRole<Guid>, Guid>
    {
        internal virtual DbSet<UserRating> UserRating { get; set; }
        internal DbSet<Log> Logs {  get; set; }
        internal DbSet<PnKUser> PnKUsers {  get; set; }
        internal DbSet<UserPhotos> UserPhotos {  get; set; }
        internal DbSet<Models.UserActionManagment> UserActionManagments { get; set; }
        internal DbSet<GlobalSettings> GlobalSettings { get; set; }
        internal DbSet<Models.Assignment> Assignments { get; set; }
        internal DbSet<Models.AssignmentFile> AssignmentFile { get; set; }
        internal DbSet<Models.AssignmentAnswer> AssignmentAnswer { get; set; }
        internal DbSet<Models.Schedule> Schedule { get; set; }
        internal DbSet<Models.AssignmentUserResult> AssignmentUserResults { get; set; }
        internal DbSet<Models.Theme> Themes { get; set; }
        
        public PnkDbContext(DbContextOptions<PnkDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserRating>().HasNoKey().ToTable((string?)null);
            base.OnModelCreating(builder);
        }
    }
}