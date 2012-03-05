using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeFirst.Models;

namespace CodeFirst.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }

        public ActionResult Test2()
        {
            using (var db = new KatushaContext())
            {
                var a = db.Boys.Find(1);
                //var b = a.Photos.Select(x => x.Profile.Id == a.Id).ToList();
                ViewBag.Message = String.Format("Found {0} and {1}", a.Description, 0); //b.Count);
            }
            return View("Test");
        }

        public ActionResult Test()
        {
            using (var db = new KatushaContext())
            {


                DateTime now = DateTime.Now;
                //db.Database.ExecuteSqlCommand("DELETE FROM BreastSizes"); 
                //db.Database.ExecuteSqlCommand("DELETE FROM DickSizes"); 
                //db.Database.ExecuteSqlCommand("DELETE FROM DickThicknesses"); 
                //db.Database.ExecuteSqlCommand("DELETE FROM Existances");
                //db.Database.ExecuteSqlCommand("DELETE FROM MembershipTypes"); 
                //db.Database.ExecuteSqlCommand("DELETE FROM Status");

                //var statuses = new[] { 
                //    new Status { Id = 1, Name = "Online", LastModified = now } ,
                //    new Status { Id = 2, Name = "Away", LastModified = now },
                //    new Status { Id = 3, Name = "Offline", LastModified = now }
                //};

                //var countries = new[] { 
                //    new Country { Id = 1, Name = "England", LastModified = now } ,
                //    new Country { Id = 2, Name = "Turkey", LastModified = now },
                //    new Country { Id = 3, Name = "Ukraine", LastModified = now },
                //    new Country { Id = 4, Name = "Russia", LastModified = now }
                //};

                //var languages = new[] { 
                //    new Language { Id = 1, Name = "English", LastModified = now } ,
                //    new Language { Id = 2, Name = "Turkish", LastModified = now },
                //    new Language { Id = 3, Name = "Ukrainian", LastModified = now },
                //    new Language { Id = 4, Name = "Russian", LastModified = now }
                //};

                //var membershipTypes = new[] { 
                //    new MembershipType { Id = 1, Name = "Normal", LastModified = now },
                //    new MembershipType { Id = 2, Name = "Gold", LastModified = now },
                //    new MembershipType { Id = 3, Name = "Platinium", LastModified = now }
                //};

                //var existances = new[] { 
                //    new Existance { Id = 1, Name = "Active", LastModified = now },
                //    new Existance { Id = 2, Name = "Expired", LastModified = now }
                //};

                //var breastSizes = new[] { 
                //    new BreastSize { Id = 1, Name = "Small", LastModified = now },
                //    new BreastSize { Id = 2, Name = "Medium", LastModified = now },
                //    new BreastSize { Id = 3, Name = "Large", LastModified = now },
                //    new BreastSize { Id = 4, Name = "ExtraLarge", LastModified = now }
                //};

                //var dickSizes = new[] {
                //    new DickSize {Id = 1, Name = "Small", LastModified = now},
                //    new DickSize {Id = 2, Name = "Medium", LastModified = now},
                //    new DickSize {Id = 3, Name = "Large", LastModified = now},
                //    new DickSize {Id = 4, Name = "ExtraLarge", LastModified = now}
                //};


                //var dickThicknesses = new[] { 
                //    new DickThickness { Id = 1, Name = "Narrow", LastModified = now },
                //    new DickThickness { Id = 2, Name = "Wide", LastModified = now },
                //    new DickThickness { Id = 3, Name = "Thick", LastModified = now },
                //    new DickThickness { Id = 4, Name = "VeryThick", LastModified = now }
                //};

                //foreach (var t in statuses) db.Statuses.Add(t);
                //foreach (var t in membershipTypes) db.MembershipTypes.Add(t);
                //foreach (var t in existances) db.Existances.Add(t);
                //foreach (var t in breastSizes) db.BreastSizes.Add(t);
                //foreach (var t in dickSizes) db.DickSizes.Add(t);
                //foreach (var t in dickThicknesses) db.DickThicknesses.Add(t);
                //foreach (var t in countries) db.Countries.Add(t);
                //foreach (var t in languages) db.Languages.Add(t);
                //db.SaveChanges();

                var profileStatus = new ProfileStatus
                                        {
                                            Existance = (byte) Existance.Active,
                                            Expires = now.AddYears(1),
                                            Guid = Guid.NewGuid(),
                                            LastModified = now,
                                            LastOnline = now.AddMinutes(1),
                                            Status = (byte) Status.Online,
                                            MembershipType = (byte) MembershipType.Normal
                                        };

                var boy = new Boy { 
                    Id = 11,
                    Description = "Test",
                    DickSize = (byte) DickSize.Medium,
                    DickThickness = (byte) DickThickness.Thick, 
                    Guid = Guid.NewGuid(), 
                    Height = 170, 
                    LastModified = now, 
                    UrlFriendlyId = "mertiko", 
                    City = "Istanbul",
                    From = (byte) Country.Turkey,
                    ProfileStatus = profileStatus
                };
                boy.CountriesToVisit.Add(new CountriesToVisit { Country = (byte)Country.Turkey, LastModified = now });
                boy.CountriesToVisit.Add(new CountriesToVisit { Country = (byte)Country.Ukraine, LastModified = now });
                boy.LanguagesSpoken.Add(new LanguagesSpoken { Language = (byte)Language.English });
                boy.LanguagesSpoken.Add(new LanguagesSpoken { Language = (byte)Language.Russian });
                boy.LanguagesSpoken.Add(new LanguagesSpoken { Language = (byte)Language.Turkish });
                var photo = new Photo { Description = "Açıklama", Guid = Guid.NewGuid(), LastModified = now };
                var photo2 = new Photo { Description = "Açıklama2", Guid = Guid.NewGuid(), LastModified = now };
                boy.Photos.Add(photo);
                boy.Photos.Add(photo2);
                db.Boys.Add(boy);
                int recordsAffected = 0;
                try
                {
                    recordsAffected = db.SaveChanges();
                    ViewBag.Message = String.Format("Saved {0} entities to the database, press any key to exit.", recordsAffected);
                } catch(Exception ex)
                {
                    var a = db.Boys.Find(11);
                    
                    ViewBag.Message = String.Format("Found {0}", a.Description);
                }
                

            }

            return View();
        }
    }
}
