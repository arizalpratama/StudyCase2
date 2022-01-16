using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class CommentTweet
    {
        public CommentTweet(Comment comment)
        {
            Comment = comment;
        }

        public Comment Comment { get; }
    }
}