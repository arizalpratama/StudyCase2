namespace TwittorAPI.GraphQL
{
    public record TransactionStatus
    (
         bool IsSucceed,
         string? Message
    );
}
