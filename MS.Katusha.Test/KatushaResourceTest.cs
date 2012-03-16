using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Diagnostics;

namespace MS.Katusha.Test
{
    [TestClass]
    public class KatushaResourceTest
    {
        private KatushaDbContext _dbContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContext = new KatushaDbContext();
        }


        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=localhost;Initial Catalog=Test;Integrated Security=True;Pooling=False");
            Database.SetInitializer<KatushaDbContext>(new KatushaContextInitializerTest());
        }

        [TestMethod]
        public void InitializeResourcesTest()
        {
            Assert.AreEqual(1,1);
        }
    }
}
