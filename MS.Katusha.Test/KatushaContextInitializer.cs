using System;
using System.Diagnostics;
using System.Globalization;
using MS.Katusha.Domain;
using System.Data.Entity;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Test
{
    public class KatushaContextInitializer : DropCreateDatabaseAlways<KatushaDbContext> //DropCreateDatabaseAlways<KatushaContext>// DropCreateDatabaseIfModelChanges<KatushaContext>
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
            CreateResources();
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Female);
            CreateSampleUser(Sex.Female);
            CreateSampleUser(Sex.Female);
            Debug.WriteLine("Created Sample Data");
        }

        private void CreateResources()
        {
            var resourceRepository = new ResourceRepositoryDB(_dbContext);
            resourceRepository.Add(new Resource { ResourceKey = "FriendlyNameRegularExpression", Language = (byte)Language.English, Value = "^[a-zA-Z0-9_]*$" });
            resourceRepository.Add(new Resource { ResourceKey = "FriendlyNameRegularExpressionError", Language = (byte)Language.English, Value = "Invalid characters in this field!" });
            resourceRepository.Add(new Resource { ResourceKey = "DescriptionEmptyError", Language = (byte)Language.English, Value = "Description field cannot be empty!" });
            
            _dbContext.SaveChanges();
        }
    }

}
