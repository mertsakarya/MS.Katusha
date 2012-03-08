using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MS.Katusha.MembershipService
{
    public enum KatushaMembershipCreateStatus : byte
    {
        Success=1, DuplicateUserName, DuplicateEmail, InvalidPassword, InvalidEmail, InvalidAnswer, InvalidQuestion, InvalidUserName, ProviderError, UserRejected
    }
}
