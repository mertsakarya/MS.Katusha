using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString);
            try {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                // Perform database operations
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            conn.Close();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
