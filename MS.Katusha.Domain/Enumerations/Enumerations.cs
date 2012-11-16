using System;

namespace MS.Katusha.Enumerations
{
    public enum LookingFor : byte { Friends = 1, Sex = 2, OneNight = 3, LongTimeRelationship = 4, Adventure = 5, Soulmate = 6, Job = 7, ShortTermRelation = 8, Marriage = 9, Fun = 10, MAX = 10 }	
    public enum HairColor : byte { Brunette = 1, Blond = 2, Scarlett = 3, Chestnut = 4, Black = 5, Grey = 6, Red = 7, Auburn = 8, Brown = 9, MAX = 9 }
	public enum BodyBuild : byte { Slim = 1, Athletic = 2, Average = 3, Curvy = 4, Heavy = 5, MAX = 5 }
	public enum EyeColor : byte { Black = 1, Hazel = 2, Brown = 3, Green = 4, Blue = 5, Grey = 6, Amber = 7, MAX = 7 }

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

    public enum Smokes : byte { Smokes = 1, DoesntSmoke = 2, MAX = 2 }
    public enum Alcohol : byte { No = 1, Sometimes = 2, Yes = 3, MAX = 3 }
    public enum Religion : byte { Christian = 1, Muslim = 2, Jewish = 3, Hindu = 4, Buddhist = 5, Atheist = 6, Agnostic = 7, Deist = 8, Other = 9, MAX = 9 }

    public enum Age : byte {LessThan18 = 1, Between18And24 = 2, Between25And29 = 3, Between30And34 = 4, Between35And39 = 5, Between40And44 = 6, Between45And49 = 7, Between50And54 = 8, Between55And59 = 9, Between60And69 = 10, After70 = 11, MAX = 11}
    public enum Height : byte { LessThan140 = 1, Between140And149 = 2, Between150And159 = 3, Between160And169 = 4, Between170And179 = 5, Between180And189 = 6, Between190And199 = 7, Between200And209 = 8, After210 = 9, MAX = 9 }
    
    public enum KatushaMembershipCreateStatus : byte { Success = 1, DuplicateUserName, DuplicateEmail, InvalidPassword, InvalidEmail, InvalidAnswer, InvalidQuestion, InvalidUserName, ProviderError, UserRejected }

    public enum MailType : byte { MailConfirm = 1, PasswordChanged, YouveGotMessage }

    public enum MessageType : byte { Received = 1, Sent = 2, MAX = 2 }

    public enum PhotoType : byte { Original = 0, Thumbnail = 1, Medium = 2, Large = 3, Icon = 4, MAX = 4 }

    public enum Action : int { LoggedIn = 1, LoggedOut, CreatedProfile, EditedProfile, VisitedProfile, SendMessage, ReadMessage, BecameMember, CanceledMembership, MembershipEXpired, AddedPhoto, ChangedProfilePicture }

    public enum CheckoutStatus { PaymentActionNotInitiated, PaymentActionFailed, PaymentActionInProgress, PaymentCompleted }
    public enum ProductNames { MonthlyKatusha = 1}

    public enum PaypalEnvironment { Sandbox = 1, Live = 2}

    public enum MSKatushaSource { Local = 1, Live = 2, LiveEU = 3 }
    public enum MSKatushaMode { Web = 1, Windows = 2 }


    [Flags]
    public enum UserRole : long { Normal = 1, Administrator = 2, Editor = 4, ApiUser = 8 }

    public enum PhotoStatus { Ready = 1, WaitingApproval = 2, Uploading = 3, NotExist = 4, Rejected = 5, MAX = 5 }
    
    public static class Folders
    {
        public static string Photos = "Photos";
        public static string PhotoBackups = "PhotoBackups";
        public static string ProfileBackups = "ProfileBackups";
        public static string DeletedProfiles = "DeletedProfiles";
        public static string Images = "Images";
        public static string DeletedPhotos = "DeletedPhotos";
    }
}
