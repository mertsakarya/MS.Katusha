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
        private KatushaContext context;

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            Debug.WriteLine("Assembly Init");
        }

        //[ClassInitialize()]
        //public static void ClassInit(TestContext context)
        //{
        //    Debug.WriteLine("ClassInit");
        //}


        [TestCleanup()]
        public void Cleanup()
        {
            context.Dispose();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Debug.WriteLine("ClassCleanup");
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            Debug.WriteLine("AssemblyCleanup");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            context = new KatushaContext();
        }


        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Database.DefaultConnectionFactory = new SqlConnectionFactory( @"Data Source=localhost;Initial Catalog=Test;Integrated Security=True;Pooling=False");
            Database.SetInitializer<KatushaContext>(new KatushaContextInitializer());

        }

        [TestMethod]
        public void ShouldBeAbleToFindBoy()
        {

            var boy = context.Boys.Find(1);
            var photos = (from photo in context.Photos where photo.ProfileId == boy.Id select photo);
            foreach (var photo in photos)
                boy.Photos.Add(photo);

            var boys = (context.Boys.Where(b => b.LanguagesSpoken.Count > 0));
            var girls = (from b in context.Girls select b);
            Debug.WriteLine(String.Format("Found {0} boy and {1} photos, out of {2} boys and {3} girls.", boy.Id,
                                          boy.Photos.Count, boys.Count(), girls.Count()));
        }

        [TestMethod]
        public void ShouldBeAbleToFindAUser()
        {
            IKatushaRepository repository = new KatushaRepository();
            var user = (from u in context.Users where u.UserName == "mertsakarya4" select u).SingleOrDefault();
            var profile = repository.GetProfile(user);
            var girl = profile as Girl;
            if (girl != null) 
                Debug.WriteLine(String.Format("Result {0}", girl.State.Expires));
        }
    }
}
