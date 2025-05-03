using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticulateWebApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        [Required]
        public string Category { get; set; } = null!;

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? UserId { get; set; }

        public IdentityUser? User { get; set; }

        public List<Comment>? Comments { get; set; }
        public List<Reaction>? Reactions { get; set; }
    }
}
