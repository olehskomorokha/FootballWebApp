namespace FootballWebApp.Model.Common;

public class UserAlreadyExistsException : ApiException
{
    public UserAlreadyExistsException()
        : base("alreadyExist", "User already exist")
    {
    }
}