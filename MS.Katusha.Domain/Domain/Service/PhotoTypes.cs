using System.Collections.Generic;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Service
{
    public static class PhotoTypes
    {
        public static readonly IDictionary<byte, string> Versions = new Dictionary<byte, string> {
            {(byte) PhotoType.Thumbnail, "width=80&height=106&crop=auto&format=jpg&quality=90"},
            {(byte) PhotoType.Medium, "maxwidth=400&maxheight=530&format=jpg&quality=90"},
            {(byte) PhotoType.Large, "maxwidth=800&maxheight=1060&format=jpg&quality=90"},
            {(byte) PhotoType.Icon, "width=40&height=53&format=jpg&quality=90"},
            {(byte) PhotoType.Original, "format=jpg"}
        };
    }
}