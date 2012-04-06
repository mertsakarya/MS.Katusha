using System;
using System.Collections.Generic;

namespace MS.Katusha.Enumerations
{
    public enum Sex : byte { Male = 1, Female = 2, MAX = 2 }
    public enum Status : byte { Online = 1, Away, Offline }
    public enum MembershipType : byte { Normal = 1, Gold = 2, Platinium = 3 }
    public enum Existance : byte { Active = 1, Expired = 2 }
    public enum BreastSize : byte { Small = 1, Medium = 2, Large = 3, ExtraLarge = 4, MAX = 4 }
    public enum DickSize : byte { Small = 1, Medium = 2, Large = 3, ExtraLarge = 4, MAX = 4 }
    public enum DickThickness : byte { Narrow = 1, Wide = 2, Thick = 3, VeryThick = 4, MAX = 4 }
    public enum Language : byte { Turkish = 1, Russian = 2, English = 3, MAX = 3, DefaultLanguage = English }
    public enum Country : byte { Turkey = 1, Ukraine = 2, Russia = 3, UnitedStates = 4, MAX = 4 }

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

    public enum MailType: byte { MailConfirm = 1, PasswordChanged, YouveGotMessage }

    public enum PhotoType : byte  { Original = 0, Thumbnail = 1, Medium = 2, Large = 3, MaxPhotoType = 3 }


    public class AgeHelper
    {
        private static readonly int Year = DateTime.UtcNow.Year;
        public static int[][] Array = new int[11][] {
                                                           new[] {00, 17},
                                                           new[] {18, 24},
                                                           new[] {25, 29},
                                                           new[] {30, 34},
                                                           new[] {35, 39},
                                                           new[] {40, 44},
                                                           new[] {45, 49},
                                                           new[] {50, 54},
                                                           new[] {55, 59},
                                                           new[] {60, 69},
                                                           new[] {70, 00},
                                                       };

        public static readonly List<string> Ranges = new List<string> {
                                                                       String.Format("[{0} TO NULL]", Year - Array[0][1]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[1][1], Year - Array[1][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[2][1], Year - Array[2][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[3][1], Year - Array[3][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[4][1], Year - Array[4][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[5][1], Year - Array[5][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[6][1], Year - Array[6][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[7][1], Year - Array[7][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[8][1], Year - Array[8][0]),
                                                                       String.Format("[{0} TO {1}]", Year - Array[9][1], Year - Array[9][0]),
                                                                       String.Format("[NULL TO {0}]", Year - Array[10][0])
                                                                   };
        public static Age GetEnum(string range)
        {
            byte i = 0;
            foreach (var val in Ranges) {
                i++;
                if (range.Equals(val)) return (Age)i;
            }
            return (byte)0;
        }

        public static Age GetRangeFromAge(int age)
        {
            if (age > Array[Array.Length - 1][0]) return (Age)(Array.Length - 1);
            for (byte i = 0; i < Array.Length - 1; i++)
                if (age >= Array[i][0] && age <= Array[i][1]) return (Age)i;
            return 0;
        }

        public static Age GetRangeFromBirthYear(int birthYear)
        {
            var age = Year - birthYear;
            return GetRangeFromAge(age);
        }

        public static string GetRange(Age age)
        {
            return Ranges[(byte)age - 1];
        }

        public static int[] GetArrayItem(Age age)
        {
            return Array[(byte)age - 1];
        }

        public static IList<int[]> GetArrayItems(IList<Age?> age)
        {
            var result = new List<int[]>();
            if(age != null)
                foreach (var val in age) {
                    if ((byte) val == 0 || (byte) val >= Array.Length) continue;
                    var items = GetArrayItem((Age) val);
                    result.Add(new int[] {(items[1] == 0)? 0 : Year - items[1], (items[0] == 0) ? 0 : Year - items[0]});
                }
            return result;
        }
    }

    public class HeightHelper
    {
        public static readonly int[][] Array = new int[9][] {
                                                           new[] {000, 139},
                                                           new[] {140, 149},
                                                           new[] {150, 159},
                                                           new[] {160, 169},
                                                           new[] {170, 179},
                                                           new[] {180, 189},
                                                           new[] {190, 199},
                                                           new[] {200, 209},
                                                           new[] {210, 000}
                                                       };

        public static readonly List<string> Ranges = new List<string> {
                                                                       String.Format("[NULL TO {0}]", Array[0][1]),
                                                                       String.Format("[{0} TO {1}]", Array[1][0], Array[1][1]),
                                                                       String.Format("[{0} TO {1}]", Array[2][0], Array[2][1]),
                                                                       String.Format("[{0} TO {1}]", Array[3][0], Array[3][1]),
                                                                       String.Format("[{0} TO {1}]", Array[4][0], Array[4][1]),
                                                                       String.Format("[{0} TO {1}]", Array[5][0], Array[5][1]),
                                                                       String.Format("[{0} TO {1}]", Array[6][0], Array[6][1]),
                                                                       String.Format("[{0} TO {1}]", Array[7][0], Array[7][1]),
                                                                       String.Format("[{0} TO NULL]", Array[8][0]),
                                                                   };

        public static Height GetRangeFromHeight(int height)
        {
            if (height > Array[Array.Length - 1][0]) return (Height)(Array.Length - 1);
            for (byte i = 0; i < Array.Length - 1; i++)
                if (height >= Array[i][0] && height <= Array[i][1]) return (Height)i;
            return 0;
        }

        public static Height GetEnum(string range)
        {
            byte i = 0;
            foreach (var val in Ranges) {
                i++;
                if (range.Equals(val)) return (Height)i;
            }
            return (byte)0;
        }

        public static string GetRange(Height height)
        {
            return Ranges[(byte)height - 1];
        }

        public static int[] GetArrayItem(Height height)
        {
            return Array[(byte)height - 1];
        }

        public static IList<int[]> GetArrayItems(IList<Height?> height) {
            var result = new List<int[]>();
            if (height != null)
                foreach (var val in height) {
                    if ((byte) val == 0 || (byte) val >= Array.Length) continue;
                    result.Add(GetArrayItem((Height) val));
                }
            return result;
        }
    }
}
