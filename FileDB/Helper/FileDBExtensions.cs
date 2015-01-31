using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    public partial class FileDB
    {
        /// <summary>
        /// Store a file inside the database
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        /// <param name="fileName">Filename/Path to read file (eg: C:\Temp\MyPhoto.jpg)</param>
        /// <returns>EntryInfo with </returns>
        public static EntryInfo Store(string dbFileName, string fileName)
        {
            using (FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return Store(dbFileName, fileName, input);
            }
        }

        /// <summary>
        /// Store a file inside the database
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        /// <param name="fileName">Filename to associate with file (eg: MyPhoto.jpg)</param>
        /// <param name="input">Stream with a file content</param>
        /// <returns>EntryInfo with file information</returns>
        public static EntryInfo Store(string dbFileName, string fileName, Stream input)
        {
            using (var db = new FileDB(dbFileName, FileAccess.ReadWrite))
            {
                return db.Store(fileName, input);
            }
        }

        /// <summary>
        /// Read a file inside the database file
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        /// <param name="id">File ID</param>
        /// <param name="fileName">Filename/Path to save the file (eg: C:\Temp\MyPhoto.jpg)</param>
        /// <returns>EntryInfo with file information</returns>
        public static EntryInfo Read(string dbFileName, Guid id, string fileName)
        {
            using (var db = new FileDB(dbFileName, FileAccess.Read))
            {
                return db.Read(id, fileName);
            }
        }

        /// <summary>
        /// Read a file inside the database file
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        /// <param name="id">File ID</param>
        /// <param name="output">Stream to save the file</param>
        /// <returns>EntryInfo with file information</returns>
        public static EntryInfo Read(string dbFileName, Guid id, Stream output)
        {
            using (var db = new FileDB(dbFileName, FileAccess.Read))
            {
                return db.Read(id, output);
            }
        }

        /// <summary>
        /// Delete a file inside a database
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        /// <returns>Array with all files identities</returns>
        public static EntryInfo[] ListFiles(string dbFileName)
        {
            using (var db = new FileDB(dbFileName, FileAccess.Read))
            {
                return db.ListFiles();
            }
        }

        /// <summary>
        /// Delete a file inside a database
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        /// <param name="id">Guid of file</param>
        /// <returns>True with found and delete the file, otherwise false</returns>
        public static bool Delete(string dbFileName, Guid id)
        {
            using (var db = new FileDB(dbFileName, FileAccess.ReadWrite))
            {
                return db.Delete(id);
            }
        }

        /// <summary>
        /// Create a new database file
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        public static void CreateEmptyFile(string dbFileName)
        {
            CreateEmptyFile(dbFileName, true);
        }

        /// <summary>
        /// Create a new database file
        /// </summary>
        /// <param name="dbFileName">Database path/filname (eg: C:\Temp\MyDB.dat)</param>
        /// <param name="ignoreIfExists">True to ignore the file if already exists, otherise, throw a exception</param>
        public static void CreateEmptyFile(string dbFileName, bool ignoreIfExists)
        {
            if (File.Exists(dbFileName))
            {
                if (ignoreIfExists)
                    return;
                else
                    throw new FileDBException("Database file {0} already exists", dbFileName);
            }

            using (FileStream fileStream = new FileStream(dbFileName, FileMode.CreateNew, FileAccess.Write))
            {
                using(BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    FileFactory.CreateEmptyFile(writer);
                }
            }
        }

        /// <summary>
        /// Shrink database file
        /// </summary>
        /// <param name="dbFileName">Path to database file (eg: C:\Temp\MyDB.dat)</param>
        public static void Shrink(string dbFileName)
        {
            using (var db = new FileDB(dbFileName, FileAccess.Read))
            {
                db.Shrink();
            }
        }

        /// <summary>
        /// Export all file inside a database to a directory
        /// </summary>
        /// <param name="dbFileName">FileDB database file</param>
        /// <param name="directory">Directory to export files</param>
        /// <param name="filePattern">File Pattern. Use keys: {id} {extension} {filename}. Eg: "{filename}.{id}.{extension}"</param>
        public static void Export(string dbFileName, string directory, string filePattern)
        {
            using (var db = new FileDB(dbFileName, FileAccess.Read))
            {
                db.Export(directory, filePattern);
            }
        }

    }
}
