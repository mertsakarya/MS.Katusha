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
using MS.Katusha.RepositoryRavenDB.Repositories;
using MS.Katusha.RepositoryDB.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MS.Katusha.Test
{
    [TestClass]
    public class RepositoryRavenDBTest
    {
        private IBoyRepositoryRavenDB _repositoryBoyRavenDB;
        private IGirlRepositoryRavenDB _repositoryGirl;


        private IBoyRepositoryDB _repositoryBoyDb;
        private IUserRepositoryDB _repositoryUserDb;
        private KatushaDbContext _dbContext;


        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            //string url = "http://mertsakarya:8080/";

            _repositoryBoyRavenDB = new MS.Katusha.RepositoryRavenDB.Repositories.BoyRepositoryRavenDB();
            _repositoryGirl = new MS.Katusha.RepositoryRavenDB.Repositories.GirlRepositoryRavenDB();


            _dbContext = new KatushaDbContext();
            _repositoryBoyDb = new MS.Katusha.RepositoryDB.Repositories.BoyRepositoryDB(_dbContext);
            _repositoryUserDb = new MS.Katusha.RepositoryDB.Repositories.UserRepositoryDB(_dbContext);
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=localhost;Initial Catalog=Test;Integrated Security=True;Pooling=False");
            //Database.SetInitializer<KatushaContext>(new KatushaContextInitializer());
        }

        [TestMethod]
        public void TestBoy()
        {
            var users = _repositoryUserDb.Query(null, null, p => p.Profile, p => p.Profile.Photos, p => p.Profile.Searches,
                                                p => p.Profile.CountriesToVisit, p => p.Profile.LanguagesSpoken);
            foreach (var user in users)
                if (user.Gender == (byte) Sex.Male)
                    _repositoryBoyRavenDB.Add(user.Profile as Boy);
                else
                {
                    _repositoryGirl.Add(user.Profile as Girl);
                }
            var boy = _repositoryBoyDb.GetById(2, p => p.User, p => p.Photos, p => p.LanguagesSpoken, p => p.Searches,
                                               p => p.CountriesToVisit);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", boy.User));
            var boyRavenDB = _repositoryBoyRavenDB.GetById(2);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", boyRavenDB));
            var girls = _repositoryGirl.Query(g => g.BreastSize == (byte) BreastSize.Large, null, p=>p.Name).ToArray();
            Guid guid = Guid.Empty;
            foreach (var girl in girls)
            {
                Debug.WriteLine(girl);
                if (girl.Id == 5)
                {
                    girl.Description = "DESCRIPTION";
                    _repositoryGirl.FullUpdate(girl);
                }
                if (girl.Id == 4)
                    _repositoryGirl.Delete(girl);
                if (girl.Id == 6)
                    guid = girl.Guid;
            }
            if (guid != Guid.Empty)
            {
                var girl6 = _repositoryGirl.GetByGuid(guid);
                Debug.WriteLine(girl6);
            }
        }
    }
}
