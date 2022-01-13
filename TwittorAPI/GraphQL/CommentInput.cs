namespace TwittorAPI.GraphQL
{
    public record CommentInput
    (
       int? Id,
       int TwitId,
       string Comments
    );
}
