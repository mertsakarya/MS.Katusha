using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Web.Caching;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Exceptions;
using MS.Katusha.Exceptions.Services;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers.BaseControllers
{
    public class ProfileController<T, TModel> : KatushaController where T : Profile where TModel : ProfileModel
    {
        private readonly IProfileService<T> _profileService;
        private readonly IResourceManager _resourceManager;

        public ProfileController(IProfileService<T> profileService, IUserService userService, IResourceManager resourceManager)
            : base(userService)
        {
            _profileService = profileService;
            _resourceManager = resourceManager;
        }

        public ActionResult Index(int? page)
        {

            var pageIndex = (page ?? 1);
            const int pageSize = 2;
            const int totalUserCount = 1000; // will be set by call to GetAllUsers due to _out_ paramter :-|
            var profiles = _profileService.GetNewProfiles(pageIndex, pageSize);
            var profilesModel = Mapper.Map<IList<TModel>>(profiles);

            var profilesAsIPagedList = new StaticPagedList<TModel>(profilesModel, pageIndex, pageSize, totalUserCount);
            var model = new PagedListModel<ProfileModel> {List = profilesAsIPagedList};
            return View(model);

        }

        public ActionResult Photos(string key)
        {
            try {
                var profile = _profileService.GetProfile(key, p => p.Photos);
                ViewBag.SameProfile = (IsKeyForProfile(key));
                var model = MapToModel(profile);
                return View(model);
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            }
        }

        public ActionResult Details(string key)
        {
            try {
                var profile = _profileService.GetProfile(key);
                ViewBag.SameProfile = IsKeyForProfile(key);
                var model = MapToModel(profile);
                return View(model);
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            }
        }

        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender > 0 || KatushaProfile != null) return View("KatushaError", new KatushaNotAllowedException(KatushaProfile, KatushaUser, "CREATE GET"));
            ProfileModel model = null;
            var controllerName = RouteData.Values["Controller"].ToString().ToLower();
            if (controllerName == "boys")
                model = new BoyModel();
            else if (controllerName == "girls")
                model = new GirlModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(TModel model)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender > 0 || KatushaProfile != null) return View("KatushaError", new KatushaNotAllowedException(KatushaProfile, KatushaUser, "CREATE POST"));
            try {
                if (!ModelState.IsValid) return View(model);
                var profile = MapToEntity(model);
                profile.UserId = KatushaUser.Id;
                profile.Guid = KatushaUser.Guid;
                _profileService.CreateProfile(profile);
                KatushaProfile = _profileService.GetProfile(profile.Guid.ToString(), p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos);
                ValidateProfileCollections(model, (T) KatushaProfile);
                if (!ModelState.IsValid) return View(model);
                return RedirectToAction("Index");
            } catch {
                return View();
            }
        }

        public ActionResult Edit(string key)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !IsKeyForProfile(key)) return View("KatushaError", new KatushaNotAllowedException(KatushaProfile, KatushaUser, key));
            try {
                ViewBag.SameProfile = true;
                var model = MapToModel((T) KatushaProfile);
                return View(model);
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            }
        }

        [HttpPost]
        public ActionResult Edit(string key, TModel model)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !IsKeyForProfile(key)) return View("KatushaError", new KatushaNotAllowedException(KatushaProfile, KatushaUser, key));
            try {
                ViewBag.SameProfile = true;
                if (!ModelState.IsValid) return View(model);
                var profileModel = (T) KatushaProfile;
                ValidateProfileCollections(model, profileModel);
                if (!ModelState.IsValid) return View(model);

                T profile = MapToEntity(model);
                profile.Id = profileModel.Id;
                profile.Guid = profileModel.Guid;
                _profileService.UpdateProfile(profile);
                return RedirectToAction("Index");
            } catch (KatushaException ex) {
                return View("KatushaError", ex);
            } catch {
                return View();
            }
        }

        private void ValidateProfileCollections(TModel model, T profileModel)
        {
            var llp = new LookupListProcessor<TModel, T, CountriesToVisitModel, CountriesToVisit, Country>(
                p => p.CountriesToVisit,
                p => p.CountriesToVisit,
                p => (Country) p.Country,
                (modelData, country) => _profileService.DeleteCountriesToVisit(modelData.Id, country),
                (modelData, country) => _profileService.AddCountriesToVisit(modelData.Id, country)
                );

            var llp2 = new LookupListProcessor<TModel, T, LanguagesSpokenModel, LanguagesSpoken, Language>(
                p => p.LanguagesSpoken,
                p => p.LanguagesSpoken,
                p => (Language) p.Language,
                (modelData, language) => _profileService.DeleteLanguagesSpoken(modelData.Id, language),
                (modelData, language) => _profileService.AddLanguagesSpoken(modelData.Id, language)
                );

            var llp3 = new LookupListProcessor<TModel, T, SearchingForModel, SearchingFor, LookingFor>(
                p => p.Searches,
                p => p.Searches,
                p => (LookingFor) p.Search,
                (modelData, search) => _profileService.DeleteSearches(modelData.Id, search),
                (modelData, search) => _profileService.AddSearches(modelData.Id, search)
                );

            llp.Process(Request, ModelState, model, profileModel);
            llp2.Process(Request, ModelState, model, profileModel);
            llp3.Process(Request, ModelState, model, profileModel);
        }

        public ActionResult Delete(string key)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !IsKeyForProfile(key)) return View("KatushaError", new KatushaNotAllowedException(KatushaProfile, KatushaUser, key));
            try {
                var profile = (T) KatushaProfile;
                var model = MapToModel(profile);
                return View("DeleteProfile", model);
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(string key, FormCollection collection)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !IsKeyForProfile(key)) return View("KatushaError", new KatushaNotAllowedException(KatushaProfile, KatushaUser, key));
            try {
                _profileService.DeleteProfile(KatushaProfile.Guid);
                return RedirectToAction("Index");
            } catch (KatushaFriendlyNameNotFoundException ex) {
                return View("KatushaError", ex);
            }
        }

        [System.Web.Http.HttpGet]
        public void DeletePhoto(string key, string photoGuid)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !IsKeyForProfile(key)) throw new HttpException(404, "Photo not found!");
            var found = false;
            foreach (var photo in KatushaProfile.Photos.Where(photo => photo.Guid == Guid.Parse(photoGuid))) {
                _profileService.DeletePhoto(photo.Guid);
                found = true;
                break;
            }
            if (!found)
                throw new HttpException(404, "Photo not found!");
        }

        [HttpGet]
        public void MakeProfilePhoto(string key, string photoGuid)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !IsKeyForProfile(key)) throw new HttpException(404, "Photo not found!");
            var found = false;
            foreach (var photo in KatushaProfile.Photos.Where(photo => photo.Guid == Guid.Parse(photoGuid))) {
                _profileService.MakeProfilePhoto(KatushaProfile.Guid, photo.Guid);
                found = true;
                break;
            }
            if (!found)
                throw new HttpException(404, "Photo not found!");
        }
                

        [System.Web.Http.HttpGet]
        public ActionResult Download(string key, string size)
        {
            return GetImage(key, size, true);
        }

        [System.Web.Http.HttpGet]
        public ActionResult Photo(string key, string size)
        {
            return GetImage(key, size);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UploadFiles(string key)
        {
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !IsKeyForProfile(key)) return View("KatushaError", new KatushaNotAllowedException(KatushaProfile, KatushaUser, key));
            string description = Request.Form["description[]"];
            if (String.IsNullOrWhiteSpace(description))
                description = "";
            return new ContentResult {
                Content = new JavaScriptSerializer {MaxJsonLength = Int32.MaxValue}.Serialize(((from string file in Request.Files select Request.Files[file] into hpf where hpf != null where hpf.ContentLength != 0 && hpf.FileName != null select ProcessPhoto(description, KatushaProfile, hpf)).ToList())),
                ContentType = "application/json"
            };
        }

        private ViewDataUploadFilesResult ProcessPhoto(string message, Profile profile, HttpPostedFileBase hpf)
        {
            var guid = Guid.NewGuid();
            var photo = new Photo {Description = message, ProfileId = profile.Id, FileName = hpf.FileName, ContentType = "image/jpeg" /*hpf.ContentType*/, Guid = guid};
            var id = (String.IsNullOrEmpty(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName;
            var controllerName = RouteData.Values["Controller"];
            var image = Image.FromStream(hpf.InputStream);

            const int width = 400;
            const int height = 530;
            const int thumbWidth = 80;
            const int thumbHeight = 106;
            const int compressRatio = 90;

            float scaleWidth = ((float) width/(float) image.Width);
            float scaleHeight = ((float) height/(float) image.Height);
            float scale = scaleHeight < scaleWidth ? scaleHeight : scaleWidth;
            var destWidth = (int) ((image.Width*scale) + 0.5);
            var destHeight = (int) ((image.Height*scale) + 0.5);

            Image resizedImage = ResizeImage(image, destWidth, destHeight);
            Image compressedImage = CompressImage(resizedImage, compressRatio);
            Image thumbnailImage = ResizeImage(resizedImage, thumbWidth, thumbHeight);
            Image compressedThumbnailImage = CompressImage(thumbnailImage, compressRatio);
            photo.FileContents = ToBytes(compressedImage);
            photo.SmallFileContents = ToBytes(compressedThumbnailImage);

            _profileService.AddPhoto(photo);

            return new ViewDataUploadFilesResult {
                name = hpf.FileName,
                size = hpf.ContentLength,
                type = hpf.ContentType,
                url = String.Format("/{0}/Download/{1}/{2}", controllerName, id, guid),
                delete_url = String.Format("/{0}/DeletePhoto/{1}/{2}", controllerName, id, guid),
                delete_type = "GET",
                thumbnail_url = @"data:" + ((!String.IsNullOrEmpty(photo.ContentType)) ? photo.ContentType : "image/jpeg") + ";base64," + EncodeBytes(photo.SmallFileContents)
            };
        }

        public class PhotoResult : ActionResult
        {
            private readonly Photo _photo;
            private readonly PhotoType _type;
            private readonly bool _download;

            public PhotoResult(Photo photo, PhotoType type, bool download = false)
            {
                this._photo = photo;
                _type = type;
                _download = download;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.RequestContext.HttpContext.Response;
                response.Clear();
                response.ContentType = _photo.ContentType;
                var bytes = (_type == PhotoType.Original) ? _photo.FileContents : _photo.SmallFileContents;
                response.AddHeader("Content-Length", bytes.Length.ToString(CultureInfo.InvariantCulture));
                if (_download) {
                    response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", _photo.FileName));
                    response.ContentType = "application/octet-stream";
                } else {
                    response.Cache.SetCacheability(HttpCacheability.Public);
                    response.Cache.SetExpires(Cache.NoAbsoluteExpiration);
                    response.AddFileDependency(_photo.Guid.ToString());
                    response.Cache.SetLastModified(_photo.ModifiedDate);
                    response.ContentType = _photo.ContentType;
                }
                response.BinaryWrite(bytes);
                response.Flush();
                response.End();
            }
        }

        private ActionResult GetImage(string key, string size, bool download = false)
        {
            var guid = Guid.Parse(key);
            var photo = _profileService.GetPhotoByGuid(guid);
            if (photo != null) {
                var smallSize = (!String.IsNullOrEmpty(size) && size.ToLowerInvariant() == "small");
                PhotoType type = (smallSize) ? PhotoType.Thumbnail : PhotoType.Original;
                return new PhotoResult(photo, type, download);
            }
            throw new HttpException(404, "Photo not found!");
        }

        private static byte[] ToBytes(Image image)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        private static Image ResizeImage(Image image, int width, int height)
        {
            Image thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
            return thumbnail;
        }

        private static Image CompressImage(Image image, int ratio = 80)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            var stream = new MemoryStream();
            foreach (var codec in codecs.Where(codec => codec.MimeType == "image/jpeg")) ici = codec;
            if (ici != null) {
                var ep = new EncoderParameters();
                ep.Param[0] = new EncoderParameter(Encoder.Quality, ratio);
                image.Save(stream, ici, ep);
            }
            return Image.FromStream(stream);
        }

        private static string EncodeBytes(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        private T MapToEntity(TModel model)
        {
            if (model is BoyModel)
                return Mapper.Map<Boy>(model) as T;
            if (model is GirlModel)
                return Mapper.Map<Girl>(model) as T;
            return null;
        }

        private TModel MapToModel(T model)
        {
            if (model is Boy)
                return Mapper.Map<BoyModel>(model) as TModel;
            if (model is Girl)
                return Mapper.Map<GirlModel>(model) as TModel;
            return null;
        }
    }

}

/*
                try {
                    if (Request.Form["CountriesToVisitSelection[]"] != null) {
                        var list = Request.Form["CountriesToVisitSelection[]"].Split(',');
                        HashSet<Country> setForm = new HashSet<Country>();
                        foreach (var line in list) {
                            Country c;
                            if (Enum.TryParse(line, out c))
                                setForm.Add(c);
                            else
                                validationResults.Add(line + " Can't Parse");
                        }

                        var setData = new HashSet<Country>();
                        foreach (var line in countriesToVisit) {
                            var country = (Country) line.Country;
                            setData.Add(country);
                            if (!setForm.Contains(country)) {
                                try {
                                    _profileService.DeleteCountriesToVisit(profileModel.Id, country);
                                } catch(Exception ex) {
                                    validationResults.Add(country.ToString() + " Can't Delete" );
                                }
                            }
                        }
                        foreach (var country in setForm) {
                            if (!setData.Contains(country)) {
                                try {
                                    _profileService.AddCountriesToVisit(profileModel.Id, country);
                                } catch (Exception ex) {
                                    validationResults.Add(country.ToString() + " Can't Add" );
                                }
                            }
                        }
                    }

                } catch(Exception ex) {
                    validationResults.Add(ex.Message);
                }
                if(validationResults.Count > 0) {
                    foreach(var item in validationResults)
                        ModelState.AddModelError("CountriesToVisit", item);
                    model.CountriesToVisit.Clear();
                    foreach (var ctv in countriesToVisit) {
                        var ctvModel = Mapper.Map<CountriesToVisitModel>(ctv);
                        model.CountriesToVisit.Add(ctvModel);
                    }
                    return View(model);
                }
 * 
 * * * */
