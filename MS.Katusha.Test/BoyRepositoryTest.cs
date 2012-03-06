using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Enums;
using MS.Katusha.IRepositories.Interfaces;
using MS.Katusha.RepositoryRavenDB;
using MS.Katusha.RepositoryRavenDB.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MS.Katusha.Test
{
    [TestClass]
    public class BoyRepositoryTest
    {
        private KatushaDbContext _dbContext;

        private IBoyRepository _repositoryBoy;
        private IGirlRepository _repositoryGirl;
        private IConversationRepository _repositoryConverstaion;
        private ICountriesToVisitRepository _repositoryCountriesToVisit;
        private ILanguagesSpokenRepository _repositoryLanguagesSpoken;
        private IPhotoRepository _repositoryPhoto;
        private ISearchingForRepository _repositorySearchingFor;
        private IStateRepository _repositoryState;
        private IUserRepository _repositoryUser;
        private IVisitRepository _repositoryVisit;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new KatushaDbContext();

            _repositoryBoy = new BoyRepository(_dbContext);
            _repositoryGirl = new GirlRepository(_dbContext);
            _repositoryConverstaion = new ConversationRepository(_dbContext);
            _repositoryCountriesToVisit = new CountriesToVisitRepository(_dbContext);
            _repositoryLanguagesSpoken = new LanguagesSpokenRepository(_dbContext);
            _repositoryPhoto = new PhotoRepository(_dbContext);
            _repositorySearchingFor = new SearchingForRepository(_dbContext);
            _repositoryState = new StateRepository(_dbContext);
            _repositoryUser = new UserRepository(_dbContext);
            _repositoryVisit = new VisitRepository(_dbContext);
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Database.DefaultConnectionFactory = new SqlConnectionFactory( @"Data Source=localhost;Initial Catalog=Test;Integrated Security=True;Pooling=False");
            //Database.SetInitializer<KatushaContext>(new KatushaContextInitializer());

        }

        [TestMethod]
        public void ShouldFindUser()
        {
            var user = _repositoryUser.GetById(4); //p => p.Photos, p=> p.LanguagesSpoken, p=>p.Searches, p=>p.CountriesToVisit, p => p.State);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", user));
        }

        [TestMethod]
        public void ShouldBeAbleToFindBoy2()
        {
            var boy = _repositoryBoy.GetById(2, null); //p => p.Photos, p=> p.LanguagesSpoken, p=>p.Searches, p=>p.CountriesToVisit, p => p.State);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", boy.User));
            var list = _repositoryBoy.Query(p => p.State.Status == (byte)Status.Online, p => p.Photos);
            foreach (var b in list)
                Debug.WriteLine(String.Format("Found boy with {0} photos.", b.Photos.Count));
        }

        [TestMethod]
        public void ShouldBeAbleToFindBoy()
        {
            var boys = (_repositoryBoy.Query(b => b.LanguagesSpoken.Count > 0, b => b.Photos));
            var boys2 = _repositoryBoy.Query(b => true, b => b.Photos, b => b.User);
            Debug.WriteLine(String.Format("Found {0} boys and {1} boys", boys.Count(), boys2.Count()));
        }

        [TestMethod]
        public void CreateNewPhoto()
        {
            var user = _repositoryUser.GetById(4);
            var photo = new Photo() { Description = "Veli", ProfileId = user.Profile.Id };


            photo = _repositoryPhoto.Add(photo);
            _dbContext.SaveChanges();
            Debug.WriteLine(photo);
        }

        [TestMethod]
        public void UpdatePhoto()
        {
            var user = _repositoryUser.GetById(4, p => p.Profile.Photos);
            var ph = (from photo in user.Profile.Photos where photo.Id == 8 select photo).SingleOrDefault();
            ph.Description = "Deneme";
            _repositoryPhoto.FullUpdate(ph);
            _dbContext.SaveChanges();
            Debug.WriteLine(ph);
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

            var boy = _repositoryBoy.GetById(1);
            var girl = _repositoryGirl.GetById(4);
            Conversation conversation = CreateConversation(boy, girl, "Hi", "I would \r\nLike to meet you");
            Visit visit = CreateVisit(boy, girl);
            Debug.WriteLine(visit);

            DeleteVisit(visit);
            Debug.WriteLine(conversation);

            var c = _repositoryConverstaion.GetByGuid(conversation.Guid, p => p.From.State);
            Debug.WriteLine(c);
        }

        [TestMethod]
        public void TestVisits()
        {
            var boy1 = _repositoryBoy.GetById(1);
            var boy2 = _repositoryBoy.GetById(2);
            var boy3 = _repositoryBoy.GetById(3);
            var girl1 = _repositoryGirl.GetById(4);
            var girl2 = _repositoryGirl.GetById(5);
            var girl3 = _repositoryGirl.GetById(6);

            Visit visit1 = CreateVisit(boy1, girl3);
            Visit visit3 = CreateVisit(boy3, girl2);
            Visit visit4 = CreateVisit(boy1, girl2);
            Visit visit5 = CreateVisit(boy1, girl2);
            Visit visit6 = CreateVisit(boy2, girl1);
            Visit visit7 = CreateVisit(boy3, girl1);
            Visit visit8 = CreateVisit(boy1, girl1);
            Visit visit9 = CreateVisit(girl1, girl2);
            Visit visit10 = CreateVisit(girl1, boy1);
            Visit visit11 = CreateVisit(girl1, boy2);
            Visit visit12 = CreateVisit(girl1, boy3);
            Visit visit13 = CreateVisit(girl2, boy3);
            Visit visit14 = CreateVisit(girl2, boy1);
            _dbContext.SaveChanges();

            Debug.WriteLine(visit1);
            Debug.WriteLine(visit3);
            Debug.WriteLine(visit4);
            Debug.WriteLine(visit5);
            Debug.WriteLine(visit6);
            Debug.WriteLine(visit7);
            Debug.WriteLine(visit8);
            Debug.WriteLine(visit9);

            var c = _repositoryGirl.GetByGuid(girl1.Guid, p => p.Photos, p => p.Visited, p=> p.RecievedMessages, p=> p.SentMessages);
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
