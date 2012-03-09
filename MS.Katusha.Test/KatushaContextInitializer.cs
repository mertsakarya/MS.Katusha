using System;
using System.Diagnostics;
using System.Globalization;
using MS.Katusha.Domain;
using System.Data.Entity;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Enums;
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

        private void CreateSampleUser(int id, Sex gender)
        {
            var userRepository = new UserRepositoryDB(_dbContext);
            var user = new User
            {
                Email = "mertsakarya@bigmail.com",
                UserName = "mertsakarya" + id.ToString(CultureInfo.InvariantCulture),
                Password = "690514",
                EmailValidated = true,
                Expires = DateTime.Now.AddYears(1)
          };
            user = userRepository.Add(user);

            Profile profile;
            if (gender == Sex.Male)
                profile = CreateSampleBoy(id, user.Guid);
            else
                profile = CreateSampleGirl(id, user.Guid);
            user.Profile = profile;
            Debug.WriteLine(user);
        }

        private Profile CreateSampleGirl(int id, Guid guid)
        {
            var now = DateTime.Now.ToUniversalTime();
            var girlRepository = new GirlRepositoryDB(_dbContext);
            var stateRepository = new StateRepositoryDB(_dbContext);
            var countriesToVisitRepository = new CountriesToVisitRepositoryDB(_dbContext);
            var languagesSpokenRepository = new LanguagesSpokenRepositoryDB(_dbContext);
            var photoRepository = new PhotoRepositoryDB(_dbContext);

            var girl = new Girl
            {
                Description = "TestGirl" + id.ToString(CultureInfo.InvariantCulture),
                BreastSize = (byte)BreastSize.Large,
                Height = 170,
                ModifiedDate = now,
                FriendlyName = "girl_mertiko" + id.ToString(CultureInfo.InvariantCulture),
                City = "Ankara" + id.ToString(CultureInfo.InvariantCulture),
                From = (byte)Country.Turkey
            };

            girl.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { Country = (byte)Country.Turkey }));
            girl.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { Country = (byte)Country.Ukraine }));
            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { Language = (byte)Language.English }));
            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { Language = (byte)Language.Russian }));
            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { Language = (byte)Language.Turkish }));
            var photo = photoRepository.Add(new Photo { Description = "Açıklama" + id.ToString(CultureInfo.InvariantCulture) });
            var photo2 = photoRepository.Add(new Photo { Description = "Açıklama2" + id.ToString(CultureInfo.InvariantCulture) });
            girl.Photos.Add(photo);
            girl.Photos.Add(photo2);

            girl = girlRepository.Add(girl, guid);
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

        private Profile CreateSampleBoy(int id, Guid guid)
        {
            var now = DateTime.Now.ToUniversalTime();
            var boyRepository = new BoyRepositoryDB(_dbContext);
            var stateRepository = new StateRepositoryDB(_dbContext);
            var countriesToVisitRepository = new CountriesToVisitRepositoryDB(_dbContext);
            var languagesSpokenRepository = new LanguagesSpokenRepositoryDB(_dbContext);
            var photoRepository = new PhotoRepositoryDB(_dbContext);
            var searchingForRepository = new SearchingForRepositoryDB(_dbContext);

            var boy = new Boy
            {
                Description = "TestBoy" + id.ToString(CultureInfo.InvariantCulture),
                DickSize = (byte)DickSize.Medium,
                DickThickness = (byte)DickThickness.Thick,
                Height = 170,
                FriendlyName = "mertiko" + id.ToString(CultureInfo.InvariantCulture),
                City = "Istanbul" + id.ToString(CultureInfo.InvariantCulture),
                From = (byte)Country.Turkey,
            };

            boy.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { Country = (byte)Country.Turkey }));
            boy.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { Country = (byte)Country.Ukraine }));
            boy.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { Language = (byte)Language.English }));
            boy.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { Language = (byte)Language.Russian }));
            boy.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { Language = (byte)Language.Turkish }));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { Search = (byte)LookingFor.LongTimeRelationship }));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { Search = (byte)LookingFor.Friend }));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { Search = (byte)LookingFor.OneNight}));
            boy.Searches.Add(searchingForRepository.Add(new SearchingFor { Search = (byte)LookingFor.Sex }));
            var photo = photoRepository.Add(new Photo { Description = "Açıklama" + id.ToString(CultureInfo.InvariantCulture) });
            var photo2 = photoRepository.Add(new Photo { Description = "Açıklama2" + id.ToString(CultureInfo.InvariantCulture) });
            boy.Photos.Add(photo);
            boy.Photos.Add(photo2);

            boy = boyRepository.Add(boy, guid);
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

        public void CreateSampleData()
        {
            CreateSampleUser(1, Sex.Male);
            CreateSampleUser(2, Sex.Male);
            CreateSampleUser(3, Sex.Male);
            CreateSampleUser(4, Sex.Female);
            CreateSampleUser(5, Sex.Female);
            CreateSampleUser(6, Sex.Female);
            Debug.WriteLine("Created Sample Data");
        }
    }

}
