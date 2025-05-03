using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace ArticulateWebApp.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public string Bio { get; set; }

        public string ProfilePicturePath { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
