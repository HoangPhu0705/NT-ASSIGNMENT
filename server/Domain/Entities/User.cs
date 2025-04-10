using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;


namespace Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [AllowNull]
        public string? ProfilePicture { get; set; }

        public DateTime Dob{ get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
    