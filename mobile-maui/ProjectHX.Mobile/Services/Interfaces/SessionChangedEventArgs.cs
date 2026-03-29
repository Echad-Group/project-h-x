namespace ProjectHX.Mobile.Services.Interfaces;

public enum SessionChangeReason
{
    SignedIn,
    SignedOut,
    Unauthorized,
    Expired,
    InvalidToken
}

public sealed class SessionChangedEventArgs : EventArgs
{
    public SessionChangedEventArgs(bool hasActiveSession, SessionChangeReason reason)
    {
        HasActiveSession = hasActiveSession;
        Reason = reason;
    }

    public bool HasActiveSession { get; }

    public SessionChangeReason Reason { get; }
}