using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class AddTwittorPayload
    {
        public AddTwittorPayload(Twittor twittor)
        {
            Twittor = twittor;
        }

        public Twittor Twittor { get; }
    }
}
