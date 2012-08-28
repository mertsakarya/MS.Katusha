using System;
using System.Collections.Generic;

namespace MS.Katusha.Enumerations
{
    public class AgeHelper
    {
        private static readonly int Year = DateTime.Now.Year;

        public static readonly int[][] Array = new [] {
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
                                                        new[] {70, 00}
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
            return 0;
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
                    result.Add(new [] {(items[1] == 0)? 0 : Year - items[1], (items[0] == 0) ? 0 : Year - items[0]});
                }
            return result;
        }
    }
}