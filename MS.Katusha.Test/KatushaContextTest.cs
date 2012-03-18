using System;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Diagnostics;

namespace MS.Katusha.Test
{
    [TestClass]
    public class KatushaRepositoryTest
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
        public void ShouldBeAbleToFindBoy()
        {

            var boy = _dbContext.Boys.Find(1);
            var photos = (from photo in _dbContext.Photos where photo.ProfileId == boy.Id select photo);
            foreach (var photo in photos)
                boy.Photos.Add(photo);

            var boys = (_dbContext.Boys.Where(b => b.LanguagesSpoken.Count > 0));
            var girls = (from b in _dbContext.Girls select b);
            Debug.WriteLine(String.Format("Found {0} boy and {1} photos, out of {2} boys and {3} girls.", boy.Id,
                                          boy.Photos.Count, boys.Count(), girls.Count()));
        }
    }
}
