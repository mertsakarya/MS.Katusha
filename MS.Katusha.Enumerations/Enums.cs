namespace MS.Katusha.Enumerations
{
    public enum Sex : byte { Male = 1, Female }
    public enum Status : byte { Online = 1, Away, Offline }
    public enum MembershipType : byte { Normal = 1, Gold = 2, Platinium = 3 }
    public enum Existance : byte { Active = 1, Expired = 2 }
    public enum BreastSize : byte { Small = 1, Medium, Large, ExtraLarge }
    public enum DickSize : byte { Small = 1, Medium, Large, ExtraLarge }
    public enum DickThickness : byte { Narrow = 1, Wide, Thick, VeryThick }
    public enum Language : byte { Turkish = 1, Russian, English }
    public enum Country : byte { Turkey = 1, Ukraine, Russia, UnitedStates }

    public enum BodyBuild : byte { Thin = 1, Fit, Average, AboveAverage, Overweight }
    public enum EyeColor : byte { Black = 1, Hazel, Brown, Green, Blue, Gray, Red }
    public enum HairColor : byte { Brunette = 1, Blonde, Scarlett, Chestnut }
    public enum Smokes : byte { Smokes = 1, DoesntSmoke }
    public enum Alcohol : byte { No = 1, Sometimes, Yes }
    public enum Religion : byte { Christian = 1, Muslim, Jewish, Hindu, Buddhist, Atheist, Agnostic, Deist, Other }

    public enum LookingFor : byte { Friend = 1, Sex, OneNight, LongTimeRelationship }

    public enum KatushaMembershipCreateStatus : byte { Success = 1, DuplicateUserName, DuplicateEmail, InvalidPassword, InvalidEmail, InvalidAnswer, InvalidQuestion, InvalidUserName, ProviderError, UserRejected }

    public enum MailType: byte { MailConfirm, PasswordChanged, YouveGotMessage }
}
