using System;
using System.Diagnostics;
using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MS.Katusha.Test
{
    [TestClass]
    public class ManRepositoryTest
    {
        private KatushaDbContext _dbContext;

        private IProfileRepositoryDB _repositoryProfile;
        private IConversationRepositoryDB _repositoryConverstaion;
        private IPhotoRepositoryDB _repositoryPhoto;
        private IUserRepositoryDB _repositoryUser;
        private IVisitRepositoryDB _repositoryVisit;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new KatushaDbContext();

            _repositoryProfile = new ProfileRepositoryDB(_dbContext);
            _repositoryConverstaion = new ConversationRepositoryDB(_dbContext);
            _repositoryPhoto = new PhotoRepositoryDB(_dbContext);
            _repositoryUser = new UserRepositoryDB(_dbContext);
            _repositoryVisit = new VisitRepositoryDB(_dbContext);
        }


        [TestMethod]
        public void ShouldFindUser()
        {
            var user = _repositoryUser.GetById(4); //p => p.Photos, p=> p.LanguagesSpoken, p=>p.Searches, p=>p.CountriesToVisit, p => p.State);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", user));
        }


        [TestMethod]
        public void ShouldBeAbleToFindMan()
        {
            var men = (_repositoryProfile.Query(b => b.LanguagesSpoken.Count > 0, null, true, b => b.Photos));
            var men2 = _repositoryProfile.Query(null, null, true, b => b.Photos, b => b.User);
            Debug.WriteLine(String.Format("Found {0} men and {1} men", men.Count(), men2.Count()));
        }

        [TestMethod]
        public void CreateNewPhoto()
        {
            var user = _repositoryUser.GetById(4);
            var photo = new Photo() { Description = "Veli", ProfileId = user.Id };


            photo = _repositoryPhoto.Add(photo);
            _dbContext.SaveChanges();
            Debug.WriteLine(photo);
        }


        [TestMethod]
        public void UpdatePhoto2()
        {
            var ph = (from photo in _dbContext.Photos where photo.Id == 8 select photo).SingleOrDefault();
            ph.Description = "Deneme2";
            _repositoryPhoto.FullUpdate(ph);
            _dbContext.SaveChanges();
            Debug.WriteLine(ph);
        }

        [TestMethod]
        public void TestVisitAndConversation()
        {

            var man = _repositoryProfile.GetById(1);
            var girl = _repositoryProfile.GetById(4);
            Conversation conversation = CreateConversation(man, girl, "Hi", "I would \r\nLike to meet you");
            Visit visit = CreateVisit(man, girl);
            Debug.WriteLine(visit);

            DeleteVisit(visit);
            Debug.WriteLine(conversation);

            var c = _repositoryConverstaion.GetByGuid(conversation.Guid);
            Debug.WriteLine(c);
        }

        [TestMethod]
        public void TestVisits()
        {
            var man1 = _repositoryProfile.GetById(1);
            var man2 = _repositoryProfile.GetById(2);
            var man3 = _repositoryProfile.GetById(3);
            var girl1 = _repositoryProfile.GetById(4);
            var girl2 = _repositoryProfile.GetById(5);
            var girl3 = _repositoryProfile.GetById(6);

            Visit visit1 = CreateVisit(man1, girl3);
            Visit visit3 = CreateVisit(man3, girl2);
            Visit visit4 = CreateVisit(man1, girl2);
            Visit visit5 = CreateVisit(man1, girl2);
            Visit visit6 = CreateVisit(man2, girl1);
            Visit visit7 = CreateVisit(man3, girl1);
            Visit visit8 = CreateVisit(man1, girl1);
            Visit visit9 = CreateVisit(girl1, girl2);
            Visit visit10 = CreateVisit(girl1, man1);
            Visit visit11 = CreateVisit(girl1, man2);
            Visit visit12 = CreateVisit(girl1, man3);
            Visit visit13 = CreateVisit(girl2, man3);
            Visit visit14 = CreateVisit(girl2, man1);
            _dbContext.SaveChanges();

            Debug.WriteLine(visit1);
            Debug.WriteLine(visit3);
            Debug.WriteLine(visit4);
            Debug.WriteLine(visit5);
            Debug.WriteLine(visit6);
            Debug.WriteLine(visit7);
            Debug.WriteLine(visit8);
            Debug.WriteLine(visit9);

            var c = _repositoryProfile.GetByGuid(girl1.Guid, p => p.Photos, p => p.Visited, p => p.RecievedMessages, p => p.SentMessages);
            Debug.WriteLine(c);
        }


        private void DeleteVisit(Visit visit)
        {
            _repositoryVisit.Delete(visit);
            _dbContext.SaveChanges();
        }

        private Conversation CreateConversation(Profile from, Profile to, string subject, string message)
        {
            var conversation = new Conversation {FromId = from.Id, ToId = to.Id, Subject = subject, Message = message};
            conversation =  _repositoryConverstaion.Add(conversation);
            _dbContext.SaveChanges();
            return conversation;
        }

        private Visit CreateVisit(Profile profile, Profile visitorProfile)
        {
            Visit visit = _repositoryVisit.Add(new Visit {ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id});
            _dbContext.SaveChanges();
            return visit;
        }


    }
}
