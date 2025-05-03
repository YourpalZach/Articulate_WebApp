using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticulateWebApp.Models
{
    public class Reaction
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; } = "Like"; // Can expand to other reactions later

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
