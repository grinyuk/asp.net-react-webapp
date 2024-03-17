using Core.ResourcesFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class PnKUser : IdentityUser<Guid>
    {
        [StringLength(64)]
        public string? FullName { get; set; }

        [StringLength(512)]
        public string? Description { get; set; }
    }
}
