using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;

namespace MS.Katusha.Web.Controllers
{
    public class ImageController : KatushaController
    {
        private IResourceManager _resourceManager;
        private readonly IPhotoService _photoService;
        public ImageController(IUserService userService, IPhotoService photoService,  IResourceManager resourceManager)
            : base(userService)
        {
            _resourceManager = resourceManager;
            _photoService = photoService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Http.HttpGet]
        public void Delete(string key)
        {
            var guid = Guid.Parse(key);
            _photoService.DeletePhoto(guid);
        }

        [System.Web.Http.HttpGet]
        public void Download(string key, string size)
        {
            GetImage(key, size, false);
        }

        [System.Web.Http.HttpGet]
        public void Display(string key, string size)
        {
            GetImage(key, size, true);
        }

        private void GetImage(string key, string size, bool display = true)
        {
            var context = HttpContext;
            var guid = Guid.Parse(key);
            var photo = _photoService.GetPhotoByGuid(guid);
            if (photo != null) {
                var smallSize = (!String.IsNullOrEmpty(size) && size.ToLowerInvariant() == "small");
                var bytes = (smallSize) ? photo.SmallFileContents : photo.FileContents;
                if (!display) {
                    context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", photo.FileName));
                    context.Response.AddHeader("Content-Length", bytes.Length.ToString(CultureInfo.InvariantCulture));
                    context.Response.ContentType = "application/octet-stream";
                } else {
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetExpires(Cache.NoAbsoluteExpiration);
                    context.Response.AddFileDependency(photo.Guid.ToString());
                    context.Response.Cache.SetLastModified(photo.ModifiedDate);
                    context.Response.AddHeader("Content-Length", bytes.Length.ToString(CultureInfo.InvariantCulture));
                    context.Response.ContentType = photo.ContentType;
                }
                context.Response.ClearContent();
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                context.Response.Flush();
            } else
                context.Response.StatusCode = 404;
        }

        [System.Web.Http.HttpPost]
        public ActionResult UploadFiles()
        {
            var r = new List<ViewDataUploadFilesResult>();

            foreach (string file in Request.Files) {
                var hpf = Request.Files[file];
                if(hpf == null) continue;
                if (hpf.ContentLength == 0 || hpf.FileName == null)
                    continue;

                var guid = Guid.NewGuid();
                var photo = new Photo {FileName = hpf.FileName, ProfileId = 1, ContentType = hpf.ContentType, Guid = guid};

                using (var reader = new StreamReader(hpf.InputStream)) {
                    using (var memstream = new MemoryStream()) {
                        reader.BaseStream.CopyTo(memstream);
                        photo.FileContents = memstream.ToArray();
                    }
                }

                System.Drawing.Image i;
                using (var ms = new MemoryStream()) {
                    ms.Write(photo.FileContents, 0, photo.FileContents.Length);
                    i = Image.FromStream(ms);
                }
                System.Drawing.Image thumbnail = i.GetThumbnailImage(80, 106, () => false, IntPtr.Zero);
                var converter = new ImageConverter();
                photo.SmallFileContents = (byte[])converter.ConvertTo(thumbnail, typeof(byte[]));

                _photoService.AddPhoto(photo);
                r.Add(new ViewDataUploadFilesResult()
                {
                    name = hpf.FileName,
                    size = hpf.ContentLength,
                    type = hpf.ContentType,
                    url = "/Image/Download/" + guid,
                    delete_url = "/Image/Delete/" + guid,
                    delete_type = "GET",
                    thumbnail_url = @"data:" + ((!String.IsNullOrEmpty(photo.ContentType)) ? photo.ContentType : "image/png") + ";base64," + EncodeBytes(photo.FileContents)

                });
            }
            var serializer = new JavaScriptSerializer {MaxJsonLength = Int32.MaxValue};
            var result = new ContentResult {
                Content = serializer.Serialize(r),
                ContentType = "application/json"
            };
            return result; // Json(r, JsonRequestBehavior.AllowGet);
        }

        private string EncodeFile(string fileName)
        {
            return EncodeBytes(System.IO.File.ReadAllBytes(fileName));
        }

        private string EncodeBytes(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }


    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string delete_url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_type { get; set; }
    }

    //public static class ImageExtensions
    //{
    //    public static Image Resize(this Image image, int width, int height)
    //    {
    //        float scale;
    //        float scaleWidth = ((float)width / (float)image.Width);
    //        float scaleHeight = ((float)height / (float)image.Height);
    //        if (scaleHeight < scaleWidth) {
    //            scale = scaleHeight;
    //        } else {
    //            scale = scaleWidth;
    //        }

    //        int destWidth = (int)((image.Width * scale) + 0.5);
    //        int destHeight = (int)((image.Height * scale) + 0.5);

    //        Bitmap bitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
    //        bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

    //        using (Graphics graphics = Graphics.FromImage(bitmap)) {
    //            graphics.Clear(Color.White);
    //            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

    //            graphics.DrawImage(image,
    //                    new Rectangle(0, 0, destWidth, destHeight),
    //                    new Rectangle(0, 0, image.Width, image.Height),
    //                    GraphicsUnit.Pixel);
    //        }
    //        return bitmap;

    //        // Save a thumbnail of the file
    //    }

    //    //http://weblogs.asp.net/gunnarpeipman/archive/2009/04/02/resizing-images-without-loss-of-quality.aspx
    //    public void ResizeImage(double scaleFactor, Stream fromStream, Stream toStream)
    //    {
    //        var image = Image.FromStream(fromStream);
    //        var newWidth = (int)(image.Width * scaleFactor);
    //        var newHeight = (int)(image.Height * scaleFactor);
    //        var thumbnailBitmap = new Bitmap(newWidth, newHeight);

    //        var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
    //        thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
    //        thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
    //        thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

    //        var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
    //        thumbnailGraph.DrawImage(image, imageRectangle);

    //        thumbnailBitmap.Save(toStream, image.RawFormat);

    //        thumbnailGraph.Dispose();
    //        thumbnailBitmap.Dispose();
    //        image.Dispose();
    //    }
    //}
}

