namespace FootballWebApp.Model.Common;

public class UserNotFoundException : ApiException
{
    public UserNotFoundException()
        : base("notFound", "User not found")
    {
    }
}