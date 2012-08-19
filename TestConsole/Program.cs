using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Autofac;
using MS.Katusha.DependencyManagement;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services.Generators;
using MS.Katusha.Services.Helpers;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace TestConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            var connectionString = "Server=9b87ca92-887e-495e-8260-a03100cdb926.sqlserver.sequelizer.com;Database=db9b87ca92887e495e8260a03100cdb926;User ID=neezyiesavjqrlst;Password=PZtqfpXkraxZUrmrUA8eT68hUKwxKFsasMcxCf6qu7L65s7RNg3pCMaSZkCB2zGS;";
            using (var con = new SqlConnection(connectionString)) {
                //
                // Open the SqlConnection.
                //
                con.Open();
                //
                // The following code shows how you can use an SqlCommand based on the SqlConnection.
                //
                using (SqlCommand command = new SqlCommand("SELECT * FROM PhotoBackups(nolock)", con))
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var num = reader.GetInt64(0);
                        
                        var binaryData = (byte[])reader.GetValue(1);
                        var guid = reader.GetGuid(2);
                        Console.WriteLine(guid);
                        using(var writer = new FileStream("p:\\photos\\"+ guid.ToString() + ".jpg", FileMode.Create, FileAccess.Write)) {
                            writer.Write(binaryData, 0, binaryData.Length);
                        }
                    }
                }
                con.Close();
            }

            Console.ReadLine();
            DependencyRegistrar.BuildContainer();
            MapperHelper.HandleMappings();
            var conversationService = DependencyRegistrar.Container.Resolve<IConversationService>();

            var profileService = DependencyRegistrar.Container.Resolve<IProfileService>();
            var utilityService = DependencyRegistrar.Container.Resolve<IUtilityService>();
            var userService = DependencyRegistrar.Container.Resolve<IUserService>();
            var profiles = new Profile[] {
                profileService.GetProfile(1),
                profileService.GetProfile(2)
            };
            var user = userService.GetUser("mertiko");
            var id = profileService.GetProfileId(user.Guid);
            var profile = utilityService.GetExtendedProfile(user, id);
            var list = new List<Guid>();
            var readCount = new Dictionary<string, int>();
            var subject = "POPP";
            for (var i = 0; i < 15; i++ ) {
                var message = GeneratorHelper.RandomString(100, true);
                var changeSubject = ((byte) (GeneratorHelper.RND.Next((int) 5))) == 2;
                if(changeSubject)
                    subject = GeneratorHelper.RandomString(4, true);
                var isRead = ((byte) (GeneratorHelper.RND.Next((int) 2))) == 1;
                var fromUserIndex = ((GeneratorHelper.RND.Next((int) 2)));
                var toUserIndex = (fromUserIndex == 0) ? 1 : 0;
                var fromProfile = profiles[fromUserIndex];
                var toProfile = profiles[toUserIndex];
                var key = String.Format("{0}->{1}", fromProfile.Id, toProfile.Id);

                var data = new Conversation() {
                                                  ToId = toProfile.Id,
                                                  ToName = toProfile.Name,
                                                  ToGuid = toProfile.Guid,
                                                  ToPhotoGuid = toProfile.ProfilePhotoGuid,

                                                  FromId = fromProfile.Id,
                                                  FromName = fromProfile.Name,
                                                  FromGuid = fromProfile.Guid,
                                                  FromPhotoGuid = fromProfile.ProfilePhotoGuid,
                                                  Subject = subject,
                                                  Message = message
                                              };
                var fromUser = userService.GetUser(fromProfile.UserId);
                fromUser.Expires = DateTime.Now.AddDays(1);
                var toUser = userService.GetUser(toProfile.UserId);
                toUser.Expires = DateTime.Now.AddDays(1);
                
                conversationService.SendMessage(fromUser, data);
                if (isRead) {
                    conversationService.ReadMessage(toUser, toProfile.Id, data.Guid);
                    if (readCount.ContainsKey(key)) {
                        readCount[key] = readCount[key] + 1;
                    } else {
                        readCount[key] = 1;
                    }

                }
                list.Add(data.Guid);
                Console.WriteLine("{0}. {1} {2} {3} -> {4} [{5}]", i, data.Guid, (isRead)?"YES":"no", data.FromId, data.ToId, data.Subject);
            }
            foreach(var item in readCount)
                Console.WriteLine("{0}  {1}", item.Key, item.Value);
            Console.ReadLine();
            int total1, total2;
            var list1 = conversationService.GetConversations(1, out total1);
            var list2 = conversationService.GetConversations(2, out total2);
            Console.ReadLine();
            foreach (var guid in list) {
                conversationService.DeleteMessage(guid);
                Console.WriteLine("Deleting {0}", guid);
            }

            //Console.ReadLine();
            //var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString);
            //try {
            //    Console.WriteLine("Connecting to MySQL...");
            //    conn.Open();
            //    // Perform database operations
            //} catch (Exception ex) {
            //    Console.WriteLine(ex.ToString());
            //}
            //conn.Close();
        }
    }
}
