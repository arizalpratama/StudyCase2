using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class AddUserRolePayload
    {
        public AddUserRolePayload(UserRole userrole)
        {
            UserRole = userrole;
        }

        public UserRole UserRole { get; }
    }
}
