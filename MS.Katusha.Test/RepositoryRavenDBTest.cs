using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Embedded;

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
            var RavenStore = new EmbeddableDocumentStore { DataDirectory = ConfigurationManager.AppSettings["Root_Folder"] + @"App_Data\MS.Katusha.RavenDB" };
            RavenStore.Initialize();
            _repositoryProfileRavenDB = new ProfileRepositoryRavenDB(RavenStore);

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
        public void TestMan()
        {
            var bs = _repositoryProfileDb.GetAll();
            foreach (var b in bs)
                _repositoryProfileRavenDB.Add(b);
            var man = _repositoryProfileDb.GetById(2, p => p.User, p => p.Photos, p => p.LanguagesSpoken, p => p.Searches,
                                               p => p.CountriesToVisit, p=> p.User, p=>p.State);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", man.User));
            var manRavenDB = _repositoryProfileRavenDB.GetById(2);
            Debug.WriteLine(String.Format("Found User:\r\n {0}", manRavenDB));
            var girls = _repositoryProfileRavenDB.Query(g => g.BreastSize == (byte)BreastSize.Large, null, true, p => p.Name).ToArray();
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
