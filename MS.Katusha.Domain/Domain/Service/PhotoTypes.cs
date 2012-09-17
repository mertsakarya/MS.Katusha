using System.Collections.Generic;
using MS.Katusha.Enumerations;
using System;

namespace MS.Katusha.Domain.Service
{
    public class PhotoFormat {
        public PhotoType PhotoType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public int Quality { get; set; }
        public string Format { get; set; }
        public string Crop { get; set; }

        public override string ToString() {
            var list = new List<string>();
            if (Width > 0) list.Add("width=" + Width);
            if (Height > 0) list.Add("height=" + Height);
            if (MaxWidth > 0) list.Add("maxwidth=" + MaxWidth);
            if (MaxHeight > 0) list.Add("maxheight=" + MaxHeight);
            if (Quality > 0) list.Add("quality=" + Quality);
            if (!String.IsNullOrWhiteSpace("Format")) list.Add("format=" + Format);
            if (!String.IsNullOrWhiteSpace("Crop")) list.Add("crop=" + Crop);
            return String.Join("&", list);
        }
    }

    public static class PhotoTypes
    {
        public static readonly IDictionary<byte, PhotoFormat> Versions = new Dictionary<byte, PhotoFormat> {
            {(byte) PhotoType.Thumbnail, new PhotoFormat {Width=80, Height=106, Crop="auto", Format="jpg", Quality=90}},
            {(byte) PhotoType.Medium, new PhotoFormat {Width=400, Height=530, Format="jpg", Quality=90}},
            {(byte) PhotoType.Large, new PhotoFormat {MaxWidth=800, MaxHeight=1060, Format="jpg", Quality=90}},
            {(byte) PhotoType.Icon, new PhotoFormat {Width=40, Height=53, Format="jpg", Quality=90}},
            {(byte) PhotoType.Original, new PhotoFormat {Format="jpg"}} 
        };
    }
}