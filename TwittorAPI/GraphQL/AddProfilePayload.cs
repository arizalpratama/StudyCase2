using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class AddProfilePayload
    {
        public AddProfilePayload(Profile profile)
        {
            Profile = profile;
        }

        public Profile Profile { get; }
    }
}
