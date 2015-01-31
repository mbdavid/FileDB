using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    /// <summary>
    /// FileDB main class.
    /// </summary>
    public partial class FileDB : IDisposable
    {
        private FileStream _fileStream = null;
        private Engine _engine = null;
        private DebugFile _debug = null;

        /// <summary>
        /// Open a database file
        /// </summary>
        /// <param name="fileName">Database filename (eg: C:\Data\MyDB.dat)</param>
        /// <param name="fileAccess">Acces mode (Read|ReadWrite|Write)</param>
        public FileDB(string fileName, FileAccess fileAccess)
        {
            Connect(fileName, fileAccess);
        }

        private void Connect(string fileName, FileAccess fileAccess)
        {
            if (!File.Exists(fileName))
                FileDB.CreateEmptyFile(fileName);

            // Não permite acesso somente gravação (transforma em leitura/gravação)
            var fa = fileAccess == FileAccess.Write || fileAccess == FileAccess.ReadWrite ? FileAccess.ReadWrite : FileAccess.Read;

            _fileStream = new FileStream(fileName, FileMode.Open, fa, FileShare.ReadWrite, (int)BasePage.PAGE_SIZE, FileOptions.None);

            _engine = new Engine(_fileStream);
        }

        /// <summary>
        /// Store a disk file inside database
        /// </summary>
        /// <param name="fileName">Full path to file (eg: C:\Temp\MyPhoto.jpg)</param>
        /// <returns>EntryInfo with information store</returns>
        public EntryInfo Store(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return Store(fileName, stream);
            }
        }

        /// <summary>
        /// Store a stream inside database
        /// </summary>
        /// <param name="fileName">Just a name of file, to get future reference (eg: MyPhoto.jpg)</param>
        /// <param name="input">Stream thats contains the file</param>
        /// <returns>EntryInfo with information store</returns>
        public EntryInfo Store(string fileName, Stream input)
        {
            var entry = new EntryInfo(fileName);
            _engine.Write(entry, input);
            return entry;
        }

        internal void Store(EntryInfo entry, Stream input)
        {
            _engine.Write(entry, input);
        }

        /// <summary>
        /// Retrieve a file inside a database
        /// </summary>
        /// <param name="id">A Guid that references to file</param>
        /// <param name="fileName">Path to save the file</param>
        /// <returns>EntryInfo with information about the file</returns>
        public EntryInfo Read(Guid id, string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                return Read(id, stream);
            }
        }

        /// <summary>
        /// Retrieve a file inside a database
        /// </summary>
        /// <param name="id">A Guid that references to file</param>
        /// <param name="output">Output strem to save the file</param>
        /// <returns>EntryInfo with information about the file</returns>
        public EntryInfo Read(Guid id, Stream output)
        {
            return _engine.Read(id, output);
        }

        /// <summary>
        /// Retrieve a file inside a database returning a FileDBStream to read
        /// </summary>
        /// <param name="id">A Guid that references to file</param>
        /// <returns>A FileDBStream ready to be readed or null if ID was not found</returns>
        public FileDBStream OpenRead(Guid id)
        {
            return _engine.OpenRead(id);
        }

        /// <summary>
        /// Search for a file inside database BUT get only EntryInfo information (don't copy the file)
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>EntryInfo with file information or null with not found</returns>
        public EntryInfo Search(Guid id)
        {
            var indexNode = _engine.Search(id);

            if (indexNode == null)
                return null;
            else
                return new EntryInfo(indexNode);
        }

        /// <summary>
        /// Delete a file inside database
        /// </summary>
        /// <param name="id">Guid ID from a file</param>
        /// <returns>True when the file was deleted or False when not found</returns>
        public bool Delete(Guid id)
        {
            return _engine.Delete(id);
        }

        /// <summary>
        /// List all files inside a FileDB
        /// </summary>
        /// <returns>Array with all files</returns>
        public EntryInfo[] ListFiles()
        {
            return _engine.ListAllFiles();
        }

        /// <summary>
        /// Export all files inside FileDB database to a directory
        /// </summary>
        /// <param name="directory">Directory name</param>
        public void Export(string directory)
        {
            this.Export(directory, "{filename}.{id}.{extension}");
        }

        /// <summary>
        /// Export all files inside FileDB database to a directory
        /// </summary>
        /// <param name="directory">Directory name</param>
        /// <param name="filePattern">File Pattern. Use keys: {id} {extension} {filename}. Eg: "{filename}.{id}.{extension}"</param>
        public void Export(string directory, string filePattern)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var files = ListFiles();

            foreach (var file in files)
            {
                var fileName = filePattern.Replace("{id}", file.ID.ToString())
                    .Replace("{filename}", Path.GetFileNameWithoutExtension(file.FileName))
                    .Replace("{extension}", Path.GetExtension(file.FileName).Replace(".", ""));

                Read(file.ID, Path.Combine(directory, fileName));
            }
        }

        /// <summary>
        /// Shrink datafile
        /// </summary>
        public void Shrink()
        {
            var dbFileName = _fileStream.Name;
            var fileAccess = _fileStream.CanWrite ? FileAccess.ReadWrite : FileAccess.Read;
            var tempFile = Path.GetDirectoryName(dbFileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(dbFileName) + ".temp" + Path.GetExtension(dbFileName);

            if (File.Exists(tempFile))
                File.Delete(tempFile);

            var entries = ListFiles();

            FileDB.CreateEmptyFile(tempFile, false);

            using (var tempDb = new FileDB(tempFile, FileAccess.ReadWrite))
            {
                foreach (var entry in entries)
                {
                    using (var stream = new MemoryStream())
                    {
                        Read(entry.ID, stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        tempDb.Store(entry, stream);
                    }
                }
            }

            Dispose();

            File.Delete(dbFileName);
            File.Move(tempFile, dbFileName);

            Connect(dbFileName, fileAccess);
        }

        public void Dispose()
        {
            if (_engine != null)
            {
                _engine.PersistPages(); // Persiste as paginas/header que ficaram em memória

                if (_fileStream.CanWrite)
                    _fileStream.Flush();

                _engine.Dispose();

                _fileStream.Dispose();
            }
        }

        /// <summary>
        /// Print debug information about FileDB Structure
        /// </summary>
        public DebugFile Debug
        {
            get
            {
                if (_debug == null)
                    _debug = new DebugFile(_engine);

                return _debug;
            }
        }
    }
}
