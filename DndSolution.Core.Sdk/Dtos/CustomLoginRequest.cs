namespace Core.Sdk;

public sealed class CustomLoginRequest
{
    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    public String Login { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public String Password { get; init; }
}