using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numeria.IO;
using System.IO;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            // Parallel Insert
            //

            string dbFile = @"C:\Temp\MvcDemo.dat";

            string[] files = new string[] {
                @"C:\Temp\DSC04901.jpg", @"C:\Temp\DSC04902.jpg", @"C:\Temp\ZipFile.zip" };

            Parallel.For(0, 3, (i) =>
            {
                Console.WriteLine("Starting " + Path.GetFileName(files[i]));
                FileDB.Store(dbFile, files[i]);
                Console.WriteLine("Ended " + Path.GetFileName(files[i]));

            });


            Console.ReadLine();

        }
    }
}
