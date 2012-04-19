using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MS.Katusha.Console
{
    class Program
    {
        static void Main(string[] args) { 
            var now = DateTime.Now;
            var dto = new DateTimeOffset(now);
            System.Console.WriteLine(now);
            System.Console.WriteLine(dto);
            System.Console.WriteLine(dto.ToLocalTime());
            System.Console.WriteLine(dto.ToUniversalTime());
            System.Console.WriteLine(now.ToLocalTime());
            System.Console.WriteLine(now.ToUniversalTime());

            now = DateTime.UtcNow;
            dto = new DateTimeOffset(now);
            System.Console.WriteLine(now);
            System.Console.WriteLine(dto);

            now = DateTime.UtcNow;
            dto = DateTimeOffset.UtcNow;
            System.Console.WriteLine(now);
            System.Console.WriteLine(dto);

            System.Console.ReadLine();
        }
    }
}
