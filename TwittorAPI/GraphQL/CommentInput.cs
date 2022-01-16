namespace TwittorAPI.GraphQL
{
    public record CommentInput
    (
       int UserId,
       int TwitId,
       string Comments
    );
}
