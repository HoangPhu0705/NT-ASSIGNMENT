

namespace SharedViewModels.Auth
{
    public class OAuthRequest
    {
        public string ExternalId { get; set; } // (e.g., Google ID)
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}
