using System;
using System.Diagnostics;
using System.Globalization;
using MS.Katusha.Domain;
using System.Data.Entity;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Enums;
using MS.Katusha.RepositoryRavenDB.Repositories;

namespace MS.Katusha.Test
{
    public class KatushaContextInitializer : DropCreateDatabaseIfModelChanges<KatushaContext> //DropCreateDatabaseAlways<KatushaContext>// DropCreateDatabaseIfModelChanges<KatushaContext>
    {
        private KatushaContext _context;
        protected override void Seed(KatushaContext context)
        {
            _context = context;
            CreateSampleData();
        }

        private void CreateSampleUser(int id, Sex gender)
        {
            var userRepository = new UserRepository(_context);
            var user = new User { Email = "mertsakarya@gmail.com", UserName = "mertsakarya" + id.ToString(CultureInfo.InvariantCulture), Password = "690514" };
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
            var girlRepository = new GirlRepository(_context);
            var stateRepository = new StateRepository(_context);
            var countriesToVisitRepository = new CountriesToVisitRepository(_context);
            var languagesSpokenRepository = new LanguagesSpokenRepository(_context);
            var photoRepository = new PhotoRepository(_context);

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
            _context.SaveChanges();

            var state = new State
            {
                Existance = (byte)Existance.Active,
                Expires = now.AddYears(1),
                LastOnline = now.AddMinutes(-1),
                Status = (byte)Status.Online,
                MembershipType = (byte)MembershipType.Normal,
                ModifiedDate = now,
                Profile = girl,
                ProfileId = girl.Id
            };
            stateRepository.Add(state);
            _context.SaveChanges();

            return girl;
        }

        private Profile CreateSampleBoy(int id, Guid guid)
        {
            var now = DateTime.Now.ToUniversalTime();
            var boyRepository = new BoyRepository(_context);
            var stateRepository = new StateRepository(_context);
            var countriesToVisitRepository = new CountriesToVisitRepository(_context);
            var languagesSpokenRepository = new LanguagesSpokenRepository(_context);
            var photoRepository = new PhotoRepository(_context);
            var searchingForRepository = new SearchingForRepository(_context);

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
            _context.SaveChanges();
            
            var state = new State
            {
                Existance = (byte)Existance.Active,
                Expires = now.AddYears(1),
                LastOnline = now.AddMinutes(-1),
                Status = (byte)Status.Online,
                MembershipType = (byte)MembershipType.Normal,
                ModifiedDate = now,
                Profile = boy,
                ProfileId = boy.Id
            };

            stateRepository.Add(state);
            _context.SaveChanges();
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
