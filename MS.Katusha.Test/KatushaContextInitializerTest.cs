using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using MS.Katusha.Domain;
using System.Data.Entity;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Test
{
    public class KatushaContextInitializerTest : DropCreateDatabaseAlways<KatushaDbContext> //DropCreateDatabaseAlways<KatushaContext>// DropCreateDatabaseIfModelChanges<KatushaContext>
    {
        private KatushaDbContext _dbContext;
        protected override void Seed(KatushaDbContext dbContext)
        {
            _dbContext = dbContext;
            CreateSampleData();
        }

        private void CreateSampleUser(Sex gender)
        {
            var userRepository = new UserRepositoryDB(_dbContext);
            var user = new User
            {
                Gender = (byte) gender,
                Email = "mertsakarya@bigmail.com",
                UserName = "mertsakarya"+DateTime.Now.Millisecond.ToString(),
                Password = "690514",
                EmailValidated = true,
                Expires = DateTime.Now.AddYears(1)
          };
            user = userRepository.Add(user);
            _dbContext.SaveChanges();

            Profile profile;
            if (gender == Sex.Male)
            {
                profile = CreateSampleBoy(user, user.Guid);
                //profile.User = user;
            } 
            else
                profile = CreateSampleGirl(user, user.Guid);
            Debug.WriteLine(user);
        }

        private Profile CreateSampleBoy(User user, Guid guid)
        {
            long id = user.Id;
            var now = DateTime.Now.ToUniversalTime();
            var boyRepository = new BoyRepositoryDB(_dbContext);
            var stateRepository = new StateRepositoryDB(_dbContext);
            var countriesToVisitRepository = new CountriesToVisitRepositoryDB(_dbContext);
            var languagesSpokenRepository = new LanguagesSpokenRepositoryDB(_dbContext);
            var photoRepository = new PhotoRepositoryDB(_dbContext);
            var searchingForRepository = new SearchingForRepositoryDB(_dbContext);

            var boy = new Boy
            {
                //User = user,
                UserId = user.Id,
                Description = "TestBoy" + id.ToString(CultureInfo.InvariantCulture),
                DickSize = (byte)DickSize.Medium,
                DickThickness = (byte)DickThickness.Thick,
                Height = 170,
                FriendlyName = "mertiko" + id.ToString(CultureInfo.InvariantCulture),
                City = "Istanbul" + id.ToString(CultureInfo.InvariantCulture),
                From = (byte)Country.Turkey,
            };

            boy = boyRepository.Add(boy, guid);
            _dbContext.SaveChanges();

            boy.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = boy.Id, Language = (byte)Language.English }));
            boy.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = boy.Id, Language = (byte)Language.Russian }));
            boy.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = boy.Id, Language = (byte)Language.Turkish }));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = boy.Id, Search = (byte)LookingFor.LongTimeRelationship }));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = boy.Id, Search = (byte)LookingFor.Friend }));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = boy.Id, Search = (byte)LookingFor.OneNight }));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = boy.Id, Search = (byte)LookingFor.Sex }));
            var photo = photoRepository.Add(new Photo { ProfileId = boy.Id, Description = "Açıklama" + id.ToString(CultureInfo.InvariantCulture) });
            var photo2 = photoRepository.Add(new Photo { ProfileId = boy.Id, Description = "Açıklama2" + id.ToString(CultureInfo.InvariantCulture) });
            boy.Photos.Add(photo);
            boy.Photos.Add(photo2);

            boy.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = boy.Id, Country = (byte)Country.Turkey }));
            boy.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = boy.Id, Country = (byte)Country.Ukraine }));
            _dbContext.SaveChanges();

            var state = new State
            {
                Existance = (byte)Existance.Active,
                LastOnline = now.AddMinutes(-1),
                Status = (byte)Status.Online,
                MembershipType = (byte)MembershipType.Normal,
                ModifiedDate = now,
                Profile = boy,
                ProfileId = boy.Id
            };

            stateRepository.Add(state);
            _dbContext.SaveChanges();
            return boy;
        }

        private Profile CreateSampleGirl(User user, Guid guid)
        {
            long id = user.Id;
            var now = DateTime.Now.ToUniversalTime();
            var girlRepository = new GirlRepositoryDB(_dbContext);
            var stateRepository = new StateRepositoryDB(_dbContext);
            var countriesToVisitRepository = new CountriesToVisitRepositoryDB(_dbContext);
            var languagesSpokenRepository = new LanguagesSpokenRepositoryDB(_dbContext);
            var photoRepository = new PhotoRepositoryDB(_dbContext);

            var girl = new Girl
            {
                //User = user,
                UserId = id,
                Description = "TestGirl" + id.ToString(CultureInfo.InvariantCulture),
                BreastSize = (byte)BreastSize.Large,
                Height = 170,
                ModifiedDate = now,
                FriendlyName = "girl_mertiko" + id.ToString(CultureInfo.InvariantCulture),
                City = "Ankara" + id.ToString(CultureInfo.InvariantCulture),
                From = (byte)Country.Turkey
            };
            girl = girlRepository.Add(girl, guid);
            _dbContext.SaveChanges();

            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = girl.Id, Language = (byte)Language.English }));
            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = girl.Id, Language = (byte)Language.Russian }));
            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = girl.Id, Language = (byte)Language.Turkish }));
            var photo = photoRepository.Add(new Photo { ProfileId = girl.Id, Description = "Açıklama" + id.ToString(CultureInfo.InvariantCulture) });
            var photo2 = photoRepository.Add(new Photo { ProfileId = girl.Id, Description = "Açıklama2" + id.ToString(CultureInfo.InvariantCulture) });
            girl.Photos.Add(photo);
            girl.Photos.Add(photo2);


            girl.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = girl.Id, Country = (byte)Country.Turkey }));
            girl.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = girl.Id, Country = (byte)Country.Ukraine }));
            _dbContext.SaveChanges();

            var state = new State
            {
                Existance = (byte)Existance.Active,
                LastOnline = now.AddMinutes(-1),
                Status = (byte)Status.Online,
                MembershipType = (byte)MembershipType.Normal,
                ModifiedDate = now,
                Profile = girl,
                ProfileId = girl.Id
            };
            stateRepository.Add(state);
            _dbContext.SaveChanges();

            return girl;
        }


        public void CreateSampleData()
        {
            SetResources();
            SetResourceLookups();
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Female);
            CreateSampleUser(Sex.Female);
            CreateSampleUser(Sex.Female);
            Debug.WriteLine("Created Sample Data");
        }

        private void SetResourceLookups()
        {
            var repository = new ResourceLookupRepositoryDB(_dbContext);
            using (var stream = new StreamReader(@"..\..\..\MS.Katusha.Web\Content\ResourceLookup.csv")) {
                while (!stream.EndOfStream) {
                    string text = stream.ReadLine();
                    if (!String.IsNullOrWhiteSpace(text)) {
                        var arr = text.Trim().Split('\t');
                        var values = new List<string>();
                        foreach (string t in arr) {
                            var item = t.Trim();
                            if (!String.IsNullOrWhiteSpace(item))
                                values.Add(item.Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n"));
                            if (values.Count == 5) {
                                AddResourceLookup(repository, values);
                                break;
                            }
                        }
                        if (values.Count == 4) {
                            AddResourceLookup(repository, values, false);
                        }
                        Debug.WriteLineIf((values.Count > 0 && values.Count < 4), "ERROR! \t" + text);
                    }
                }
            }
            _dbContext.SaveChanges();
        }

        private void SetResources()
        {
            var repository = new ResourceRepositoryDB(_dbContext);
            using (var stream = new StreamReader(@"..\..\..\MS.Katusha.Web\Content\Resource.csv"))
            {
                while (!stream.EndOfStream)
                {
                    string text = stream.ReadLine();
                    if (!String.IsNullOrWhiteSpace(text))
                    {
                        var arr = text.Trim().Split('\t');
                        var values = new List<string>();
                        foreach (string t in arr)
                        {
                            var item = t.Trim();
                            if (!String.IsNullOrWhiteSpace(item))
                                values.Add(item.Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n"));
                            if (values.Count == 3)
                            {
                                AddResource(repository, values);
                                break;
                            }
                        }
                        Debug.WriteLineIf((values.Count > 0 && values.Count != 3), "ERROR! \t" + text);
                    }
                }
            }
            _dbContext.SaveChanges();
        }

        private static void AddResourceLookup(ResourceLookupRepositoryDB repository, List<string> values, bool hasOrder = true)
        {
            byte language;
            byte order;
            if (GetLanguage(values, out language)) return;
            if (!hasOrder)
                order = 0;
            else
            {
                if (!Byte.TryParse(values[4], out order))
                {
                    Debug.WriteLine(String.Format("ORDER ERROR: {0} {1} {2} {3} {4}", values[0], values[1], values[2],
                                                  values[3], values[4]));
                    return;
                }
            }
            repository.Add(new ResourceLookup { LookupName = values[1], ResourceKey = values[2], Language = language, Value = values[3], Order = order});
        }

        private static bool GetLanguage(List<string> values, out byte language)
        {
            Language ll = 0;
            byte lb = 0;
            if (!Byte.TryParse(values[0], out lb))
            {
                if (!Enum.TryParse(values[0], true, out ll))
                {
                    Debug.WriteLine(String.Format("LANGUAGE ERROR: {0} {1} {2}", values[0], values[1], values[2]));
                    language = 255;
                    return true;
                }
            }
            language = (byte)(lb + (byte) ll);
            if (language < 0 || language > (byte)Language.MaxLanguage) {
                Debug.WriteLine(String.Format("LANGUAGE ERROR: {0} {1} {2}", values[0], values[1], values[2]));
                return true;
            }
            return false;
        }

        private static void AddResource(ResourceRepositoryDB repository, List<string> values)
        {
            byte language;
            if (GetLanguage(values, out language)) return;
            repository.Add(new Resource { ResourceKey = values[1], Language = language, Value = values[2] });
        }
    }

}
