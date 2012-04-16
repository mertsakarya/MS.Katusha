using System;
using System.Collections.Generic;

namespace MS.Katusha.Enumerations
{
    public class HeightHelper
    {
        public static readonly int[][] Array = new [] {
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
            return 0;
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