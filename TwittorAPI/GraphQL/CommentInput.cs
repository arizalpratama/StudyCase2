namespace TwittorAPI.GraphQL
{
    public record CommentInput
    (
       int? Id,
       int UserId,
       int TwitId,
       string Comments
    );
}
