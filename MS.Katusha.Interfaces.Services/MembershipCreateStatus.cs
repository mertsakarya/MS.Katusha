namespace MS.Katusha.Interfaces.Services
{
    public enum KatushaMembershipCreateStatus : byte
    {
        Success=1, DuplicateUserName, DuplicateEmail, InvalidPassword, InvalidEmail, InvalidAnswer, InvalidQuestion, InvalidUserName, ProviderError, UserRejected
    }
}
