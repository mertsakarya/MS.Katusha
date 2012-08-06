using System.ComponentModel;

namespace MS.Katusha.Enumerations
{
    public enum Sex : byte { Male = 1, Female = 2, MAX = 2 }
    public enum Status : byte { Online = 1, Away, Offline }
    public enum MembershipType : byte { Normal = 1, Gold = 2, Platinium = 3, Administrator = 4, MAX = 4 }
    public enum TimeFrameType : byte { Year = 1, Month = 2, Day = 3, Hour = 4, MAX = 4 }
    public enum Existance : byte { Active = 1, Expired = 2 }
    public enum BreastSize : byte { Small = 1, Medium = 2, Large = 3, ExtraLarge = 4, MAX = 4 }
    public enum DickSize : byte { Small = 1, Medium = 2, Large = 3, ExtraLarge = 4, MAX = 4 }
    public enum DickThickness : byte { Narrow = 1, Wide = 2, Thick = 3, VeryThick = 4, MAX = 4 }
    //public enum Language : byte { Turkish = 1, Russian = 2, English = 3, MAX = 3, DefaultLanguage = English }
    //public enum Country : byte { Turkey = 1, Ukraine = 2, Russia = 3, UnitedStates = 4, MAX = 4 }

    public enum BodyBuild : byte { Thin = 1, Fit = 2, Average = 3, AboveAverage = 4, Overweight = 5, MAX = 5 }
    public enum EyeColor : byte { Black = 1, Hazel = 2, Brown = 3, Green = 4, Blue = 5, Gray = 6, Red = 7, MAX = 7 }
    public enum HairColor : byte { Brunette = 1, Blonde = 2, Scarlett = 3, Chestnut = 4, MAX = 4 }
    public enum Smokes : byte { Smokes = 1, DoesntSmoke = 2, MAX = 2 }
    public enum Alcohol : byte { No = 1, Sometimes = 2, Yes = 3, MAX = 3 }
    public enum Religion : byte { Christian = 1, Muslim = 2, Jewish = 3, Hindu = 4, Buddhist = 5, Atheist = 6, Agnostic = 7, Deist = 8, Other = 9, MAX = 9 }

    public enum Age : byte {LessThan18 = 1, Between18And24 = 2, Between25And29 = 3, Between30And34 = 4, Between35And39 = 5, Between40And44 = 6, Between45And49 = 7, Between50And54 = 8, Between55And59 = 9, Between60And69 = 10, After70 = 11, MAX = 11}
    public enum Height : byte { LessThan140 = 1, Between140And149 = 2, Between150And159 = 3, Between160And169 = 4, Between170And179 = 5, Between180And189 = 6, Between190And199 = 7, Between200And209 = 8, After210 = 9, MAX = 9 }
    
    public enum LookingFor : byte { Friend = 1, Sex = 2, OneNight = 3, LongTimeRelationship = 4, MAX = 4 }

    public enum KatushaMembershipCreateStatus : byte { Success = 1, DuplicateUserName, DuplicateEmail, InvalidPassword, InvalidEmail, InvalidAnswer, InvalidQuestion, InvalidUserName, ProviderError, UserRejected }

    public enum MailType : byte { MailConfirm = 1, PasswordChanged, YouveGotMessage }

    public enum MessageType : byte { Received = 1, Sent = 2, MAX = 2 }

    public enum PhotoType : byte { Original = 0, Thumbnail = 1, Medium = 2, Large = 3, Icon = 4, MAX = 4 }

    public enum Action : int { LoggedIn = 1, LoggedOut, CreatedProfile, EditedProfile, VisitedProfile, SendMessage, ReadMessage, BecameMember, CanceledMembership, MembershipEXpired, AddedPhoto, ChangedProfilePicture }

    public enum CheckoutStatus { PaymentActionNotInitiated, PaymentActionFailed, PaymentActionInProgress, PaymentCompleted }
    public enum ProductNames { MonthlyKatusha }

    public enum PaypalEnvironment { Sandbox, Live }

}
