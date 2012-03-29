using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using MS.Katusha.Domain;
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
        private IProfileRepositoryRavenDB _repositoryProfileRavenDB;


        private IProfileRepositoryDB _repositoryProfileDb;
        private IUserRepositoryDB _repositoryUserDb;
        private KatushaDbContext _dbContext;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            //Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=localhost;Initial Catalog=Test;Integrated Security=True;Pooling=False");
            Database.SetInitializer<KatushaDbContext>(new KatushaContextInitializerTest());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _repositoryProfileRavenDB = new ProfileRepositoryRavenDB();

            _dbContext = new KatushaDbContext();
            _repositoryProfileDb = new ProfileRepositoryDB(_dbContext);
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
            var bs = _repositoryProfileDb.GetAll();
            foreach (var b in bs)
                _repositoryProfileRavenDB.Add(b);
            var boy = _repositoryProfileDb.GetById(2, p => p.User, p => p.Photos, p => p.LanguagesSpoken, p => p.Searches,
                                               p => p.CountriesToVisit, p=> p.User, p=>p.State);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", boy.User));
            var boyRavenDB = _repositoryProfileRavenDB.GetById(2);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", boyRavenDB));
            var girls = _repositoryProfileRavenDB.Query(g => g.BreastSize == (byte)BreastSize.Large, null, p => p.Name).ToArray();
            Guid guid = Guid.Empty;
            foreach (var girl in girls)
            {
                Debug.WriteLine(girl);
                if (girl.Id == 5)
                {
                    girl.Description = "DESCRIPTION";
                    _repositoryProfileRavenDB.FullUpdate(girl);
                }
                //if (girl.Id == 4)
                //    _repositoryGirlRavenDB.Delete(girl);
                if (girl.Id == 6)
                    guid = girl.Guid;
            }
            if (guid == Guid.Empty) return;
            var girl6 = _repositoryProfileRavenDB.GetByGuid(guid);
            Debug.WriteLine(girl6);
        }
    }
}
