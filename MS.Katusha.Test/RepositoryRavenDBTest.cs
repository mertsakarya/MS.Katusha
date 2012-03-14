using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MS.Katusha.Test
{
    [TestClass]
    public class RepositoryRavenDBTest
    {
        private IBoyRepositoryRavenDB _repositoryBoyRavenDB;
        private IGirlRepositoryRavenDB _repositoryGirlRavenDB;


        private IBoyRepositoryDB _repositoryBoyDb;
        private IGirlRepositoryDB _repositoryGirlDb;
        private IUserRepositoryDB _repositoryUserDb;
        private KatushaDbContext _dbContext;


        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _repositoryBoyRavenDB = new BoyRepositoryRavenDB();
            _repositoryGirlRavenDB = new GirlRepositoryRavenDB();

            _dbContext = new KatushaDbContext();
            _repositoryBoyDb = new BoyRepositoryDB(_dbContext);
            _repositoryGirlDb = new GirlRepositoryDB(_dbContext);
            _repositoryUserDb = new UserRepositoryDB(_dbContext);
        }

        [TestMethod]
        public void TestMail()
        {
            var user = _repositoryUserDb.GetById(2);
            Mailer.Mailer.SendMail(user.Email, "Welcome", "MailConfirm.cshtml", user);
        }

        [TestMethod]
        public void TestBoy()
        {
            var bs = _repositoryBoyDb.GetAll();
            foreach (var b in bs)
                _repositoryBoyRavenDB.Add(b);
            var gs = _repositoryGirlDb.GetAll();
            foreach (var g in gs)
                _repositoryGirlRavenDB.Add(g);
            var boy = _repositoryBoyDb.GetById(2, p => p.User, p => p.Photos, p => p.LanguagesSpoken, p => p.Searches,
                                               p => p.CountriesToVisit);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", boy.User));
            var boyRavenDB = _repositoryBoyRavenDB.GetById(2);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", boyRavenDB));
            var girls = _repositoryGirlRavenDB.Query(g => g.BreastSize == (byte) BreastSize.Large, null, p=>p.Name).ToArray();
            Guid guid = Guid.Empty;
            foreach (var girl in girls)
            {
                Debug.WriteLine(girl);
                if (girl.Id == 5)
                {
                    girl.Description = "DESCRIPTION";
                    _repositoryGirlRavenDB.FullUpdate(girl);
                }
                if (girl.Id == 4)
                    _repositoryGirlRavenDB.Delete(girl);
                if (girl.Id == 6)
                    guid = girl.Guid;
            }
            if (guid != Guid.Empty)
            {
                var girl6 = _repositoryGirlRavenDB.GetByGuid(guid);
                Debug.WriteLine(girl6);
            }
        }
    }
}
