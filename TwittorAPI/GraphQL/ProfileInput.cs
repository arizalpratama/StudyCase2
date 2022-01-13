namespace TwittorAPI.GraphQL
{
    public record ProfileInput
    (
       int? Id,
       int UserId,
       string Name
    );
}
