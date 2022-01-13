namespace TwittorAPI.GraphQL
{
    public record UpdatePasswordInput
    (
        int? Id,
        string oldPassword,
        string newPassword
    );
}