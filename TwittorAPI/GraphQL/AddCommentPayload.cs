using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class AddCommentPayload
    {
        public AddCommentPayload(Comment comment)
        {
            Comment = comment;
        }

        public Comment Comment { get; }
    }
}
