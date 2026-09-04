namespace Auth.Objects;

public enum UserResults
{
    Ok,
    NotFound,
    SystemCrash,
    InvalidToken,
    InvalidEmail,
    InvalidPasswordOrUnkownUser,
    AlreadyConfirmed,
    InvalidUsername,
    InvalidCountry,
    AlreadyExists,
    Undetermined,
    RemoteLoginFailed
}