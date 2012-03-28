using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq.Expressions;
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
using MS.Katusha.Exceptions.Web;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;
using MS.Katusha.Attributes;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class ProfilesController : KatushaController
    {
        private readonly IProfileService _profileService;
        private readonly IResourceManager _resourceManager;
        private const int PageSize = 50;

        public ProfilesController(IProfileService profileService, IUserService userService, IResourceManager resourceManager)
            : base(userService)
        {
            _profileService = profileService;
            _resourceManager = resourceManager;
        }

        public ActionResult Boys(int? page) { return LIndex(p => p.Gender == (byte)Sex.Male, page); }
        public ActionResult Girls(int? page) { return LIndex(p => p.Gender == (byte)Sex.Female, page); }
        private ActionResult LIndex(Expression<Func<Profile, bool>> controllerFilter, int? page = 1)
        {
            var pageIndex = (page ?? 1);
            int total;
            var profiles = _profileService.GetNewProfiles(controllerFilter, out total, pageIndex, PageSize);
            var profilesModel = Mapper.Map<IList<ProfileModel>>(profiles);

            var profilesAsIPagedList = new StaticPagedList<ProfileModel>(profilesModel, pageIndex, PageSize, total);
            var model = new PagedListModel<ProfileModel> { List = profilesAsIPagedList };
            return View("Index", model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Visitors(int? page)
        {
            var pageIndex = (page ?? 1);
            int total;

            var visitors = _profileService.GetVisitors(KatushaProfile.Id, out total, pageIndex, PageSize);
            var visitorsModel = Mapper.Map<IList<VisitModel>>(visitors);

            var visitorAsIPagedList = new StaticPagedList<VisitModel>(visitorsModel, pageIndex, PageSize, total);
            var model = new PagedListModel<VisitModel> { List = visitorAsIPagedList };

            return View(model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Photos()
        {
            var profile = _profileService.GetProfile(KatushaProfile.Id, null, p=>p.Photos);
            ViewBag.SameProfile = true;
            var model = Mapper.Map<ProfileModel>(profile);
            return View(model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult MyMessages(int? pageNo = 1)
        {
            int total;
            var messages = _profileService.GetMessages(KatushaProfile.Id, out total, pageNo ?? 1);
            var messagesModel = Mapper.Map<IList<ConversationModel>>(messages);
            var messagesAsIPagedList = new StaticPagedList<ConversationModel>(messagesModel, pageNo ?? 1, 20, total);
            var model = new PagedListModel<ConversationModel> {List = messagesAsIPagedList};
            return View(model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult SendMessage(string key)
        {
            var to = GetProfile(key);
            var toModel = Mapper.Map<ProfileModel>(to);
            var from = Mapper.Map<ProfileModel>(KatushaProfile);
            var model = new ConversationModel {To = toModel, ToId = toModel.Id, FromId = from.Id, From = from};
            return View(model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult SendMessage(string key, ConversationModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var to = GetProfile(key);
            var data = Mapper.Map<Conversation>(model);
            data.ToId = to.Id;
            data.FromId = KatushaProfile.Id;
            data.ReadDate = new DateTime(1900, 1, 1, 0, 0, 0);
            _profileService.SendMessage(data);
            return RedirectToAction("MyMessages");
        }

        [HttpGet]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public void ReadMessage(string key)
        {
            Guid guid;
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !Guid.TryParse(key, out guid)) throw new HttpException(404, "Message not found!");
            _profileService.ReadMessage(KatushaProfile.Id, guid);
        }

        [KatushaFilter(IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
        public ActionResult Show(string key)
        {
            var profile = GetProfile(key, KatushaProfile);
            ViewBag.SameProfile = IsKeyForProfile(key);
            var model = Mapper.Map<ProfileModel>(profile);
            return View(model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = false)]
        public ActionResult Create(string key)
        {
            if (KatushaUser.Gender > 0) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, "CREATE GET");
            var model = new ProfileModel();
            if(key == null)
                throw new KatushaGenderNotExistsException(null);
            var genderValue = key.ToLowerInvariant();
            if (genderValue == "girl")
                model.Gender = (byte)Sex.Female;
            else if (genderValue == "boy")
                model.Gender = (byte) Sex.Male;
            else 
                throw new KatushaGenderNotExistsException(null);
            return View(model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = false, MustHaveProfile = false)]
        public ActionResult Create(string key, ProfileModel model)
        {
            if (KatushaUser.Gender > 0) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, "CREATE GET");
            try {
                if (!ModelState.IsValid) return View(model);
                var genderValue = key.ToLowerInvariant();
                if (genderValue == "girl") {
                    model.Gender = (byte) Sex.Female;
                } else if (genderValue == "boy") {
                    model.Gender = (byte)Sex.Male;
                } else {
                    throw new KatushaGenderNotExistsException(null);
                }
                var profile = Mapper.Map<Profile>(model);
                profile.UserId = KatushaUser.Id;
                profile.Guid = KatushaUser.Guid;

                ValidateProfileCollections(model, profile, false);
                if (!ModelState.IsValid) return View(model);

                _profileService.CreateProfile(profile);
                ValidateProfileCollections(model, profile, true);
                //TODO: This is error prone. 
                KatushaProfile = GetProfile(profile.Guid.ToString(), null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos);
                ValidateProfileCollections(model, KatushaProfile);
                if (!ModelState.IsValid) return View(key, model);
                return RedirectToAction((model.Gender == (byte)Sex.Male)? "Girls": "Boys");
            } catch (KatushaFriendlyNameExistsException ex) {
                ModelState.AddModelError("FriendlyName", _resourceManager._R("FriendlyNameExists"));
                return View(model);
            } catch (KatushaException ex) {
                throw;
            } catch {
                return View();
            }
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Edit(string key)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            ViewBag.SameProfile = true;
            var model = Mapper.Map<ProfileModel>(KatushaProfile);
            return View(model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Edit(string key, ProfileModel model)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            try {
                ViewBag.SameProfile = true;
                if (!ModelState.IsValid) return View(model);
                var profileModel = KatushaProfile;
                ValidateProfileCollections(model, profileModel, false);
                if (!ModelState.IsValid) return View(model);
                var profile = Mapper.Map<Profile>(model);
                profile.Id = profileModel.Id;
                profile.Guid = profileModel.Guid;
                profile.Gender = profileModel.Gender;
                profile.ProfilePhotoGuid = profileModel.ProfilePhotoGuid;
                ValidateProfileCollections(model, profileModel);
                if (!ModelState.IsValid) return View(model);
                _profileService.UpdateProfile(profile);
                return RedirectToAction("Show", new { key = (String.IsNullOrWhiteSpace(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName });
            } catch (KatushaFriendlyNameExistsException ex) {
                ModelState.AddModelError("FriendlyName", _resourceManager._R("FriendlyNameExists"));
                return View(model);
            } catch (KatushaException ex) {
                throw;
            } catch {
                return View();
            }
        }

        private void ValidateProfileCollections(ProfileModel profileModel, Profile model, bool performDataOperation = true)
        {
            if (!performDataOperation) {
                var formValue = Request.Form["CountriesToVisitModelSelection[]"];
                if (!String.IsNullOrWhiteSpace(formValue)) {
                    var list = formValue.Split(',');
                    Country c;
                    foreach (var line in list) if (Enum.TryParse(line, out c)) profileModel.CountriesToVisit.Add(new CountriesToVisitModel { Country = c });
                }
                formValue = Request.Form["LanguagesSpokenModelSelection[]"];
                if (!String.IsNullOrWhiteSpace(formValue)) {
                    var list = formValue.Split(',');
                    Language c;
                    foreach (var line in list) if (Enum.TryParse(line, out c)) profileModel.LanguagesSpoken.Add(new LanguagesSpokenModel { Language = c });
                }
                formValue = Request.Form["SearchingForModelSelection[]"];
                if (!String.IsNullOrWhiteSpace(formValue)) {
                    var list = formValue.Split(',');
                    LookingFor c;
                    foreach (var line in list) if (Enum.TryParse(line, out c)) profileModel.Searches.Add(new SearchingForModel { Search = c });
                }
            }

            var llp = new LookupListProcessor<ProfileModel, Profile, CountriesToVisitModel, CountriesToVisit, Country>(
                p => p.CountriesToVisit,
                p => p.CountriesToVisit,
                p => (Country)p.Country,
                p => (Country)p.Country,
                (modelData, country) => _profileService.DeleteCountriesToVisit(modelData.Id, country),
                (modelData, country) => _profileService.AddCountriesToVisit(modelData.Id, country)
                );

            var llp2 = new LookupListProcessor<ProfileModel, Profile, LanguagesSpokenModel, LanguagesSpoken, Language>(
                p => p.LanguagesSpoken,
                p => p.LanguagesSpoken,
                p => (Language)p.Language,
                p => (Language)p.Language,
                (modelData, language) => _profileService.DeleteLanguagesSpoken(modelData.Id, language),
                (modelData, language) => _profileService.AddLanguagesSpoken(modelData.Id, language)
                );

            var llp3 = new LookupListProcessor<ProfileModel, Profile, SearchingForModel, SearchingFor, LookingFor>(
                p => p.Searches,
                p => p.Searches,
                p => (LookingFor)p.Search,
                p => (LookingFor)p.Search,
                (modelData, search) => _profileService.DeleteSearches(modelData.Id, search),
                (modelData, search) => _profileService.AddSearches(modelData.Id, search)
                );

            llp.Process(Request, ModelState, profileModel, model, performDataOperation);
            llp2.Process(Request, ModelState, profileModel, model, performDataOperation);
            llp3.Process(Request, ModelState, profileModel, model, performDataOperation);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Delete(string key)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            var profile = KatushaProfile;
            var model = Mapper.Map<ProfileModel>(profile);
            return View("Delete", model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Delete(string key, FormCollection collection)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            _profileService.DeleteProfile(KatushaProfile.Id);
            return RedirectToAction("Index");
        }

        [System.Web.Http.HttpGet]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public void DeletePhoto(string key, string photoGuid)
        {
            if (!IsKeyForProfile(key)) throw new HttpException(404, "Photo not found!");
            _profileService.DeletePhoto(KatushaProfile.Id, Guid.Parse(photoGuid));
        }

        [HttpGet]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public void MakeProfilePhoto(string key, string photoGuid)
        {
            if (!IsKeyForProfile(key)) throw new HttpException(404, "Photo not found!");
            _profileService.MakeProfilePhoto(KatushaProfile.Id, Guid.Parse(photoGuid));
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
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult UploadFiles(string key)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            string description = Request.Form["description[]"];
            if (String.IsNullOrWhiteSpace(description))
                description = "";
            return new ContentResult {
                Content = new JavaScriptSerializer {MaxJsonLength = Int32.MaxValue}.Serialize(((from string file in Request.Files select Request.Files[file] into hpf where hpf != null where hpf.ContentLength != 0 && hpf.FileName != null select ProcessPhoto(description, KatushaProfile, hpf)).ToList())),
                ContentType = "application/json"
            };
        }

        private Profile GetProfile(string key, Profile visitorProfile = null,  params Expression<Func<Profile, object>>[] includeExpressionParams)
        {
            Guid guid;
            long id = Guid.TryParse(key, out guid) ? _profileService.GetProfileId(guid) : _profileService.GetProfileId(key);
            if (id > 0) {
                return _profileService.GetProfile(id, visitorProfile, includeExpressionParams);
            }
            return null;
        }

        private ViewDataUploadFilesResult ProcessPhoto(string message, Profile profile, HttpPostedFileBase hpf)
        {
            var guid = Guid.NewGuid();
            var photo = new Photo {Description = message, ProfileId = profile.Id, FileName = hpf.FileName, ContentType = "image/jpeg" /*hpf.ContentType*/, Guid = guid};
            var id = (String.IsNullOrEmpty(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName;
            var controllerName = GenderHelper.GetControllerName(profile);
            var image = Image.FromStream(hpf.InputStream);

            const int width = 400;
            const int height = 530;
            const int thumbWidth = 80;
            const int thumbHeight = 106;
            const int compressRatio = 90;

            var scaleWidth = (width/(float) image.Width);
            var scaleHeight = (height/(float) image.Height);
            var scale = scaleHeight < scaleWidth ? scaleHeight : scaleWidth;
            var destWidth = (int) ((image.Width*scale) + 0.5);
            var destHeight = (int) ((image.Height*scale) + 0.5);

            var resizedImage = ResizeImage(image, destWidth, destHeight);
            var compressedImage = CompressImage(resizedImage, compressRatio);
            var thumbnailImage = ResizeImage(resizedImage, thumbWidth, thumbHeight);
            var compressedThumbnailImage = CompressImage(thumbnailImage, compressRatio);
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

        private class PhotoResult : ActionResult
        {
            private readonly Photo _photo;
            private readonly PhotoType _type;
            private readonly bool _download;

            public PhotoResult(Photo photo, PhotoType type, bool download = false)
            {
                _photo = photo;
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
            Guid guid;
            if (Guid.TryParse(key, out guid)) {
                var photo = _profileService.GetPhotoByGuid(guid);
                if (photo != null) {
                    var smallSize = (!String.IsNullOrEmpty(size) && size.ToLowerInvariant() == "small");
                    var type = (smallSize) ? PhotoType.Thumbnail : PhotoType.Original;
                    return new PhotoResult(photo, type, download);
                }
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
    }
}
