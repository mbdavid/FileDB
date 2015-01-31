# FileDB

FileDB is a free, fast, lightweight C# (v3.5) DLL project to store, retrieve and delete files using a single archive file as a container on disk. It's ideal for storing files (all kind, all sizes) without databases and keeping them organized on a single disk file.

Let's see how to use FileDB with static helper methods.

```C#
using Numeria.IO;

var pathDB = @"C:\Temp\MyDB.fdb";

// Creating an empty FileDB archive
FileDB.CreateEmptyFile(pathDB);

// Store file from input stream
var info = FileDB.Store(pathDB, "MyFileName.jpg", inputStream);
// -or- store directly from the file itself
var info = FileDB.Store(pathDB, @"C:\Temp\MyPhoto.jpg");

// The 'info' variable returned contains information about your file (generated GUID, filename, file-length, mime-type) 
var fileGuid = info.ID;

// Reading file inside FileDB and writing it on an output stream (also available to write it directly to a file)
var info = FileDB.Read(pathDB, fileGuid, outputStream);

// Deleting a file
var ok = FileDB.Delete(pathDB, fileGuid);
```

## FileDB Methods

- `CreateEmptyFile` - Create an empty data file archive
- `Store` - Store from file/stream to the data file
- `Read` - Search a fileID and restore it to output file/stream
- `Delete` - Delete a file
- `ListFiles` - List all files inside the archive
- `Shrink` - Reorganize archive removing unused disk space
- `Export` - Export files inside archive to a directory

All operations have a static method helper or can be used from a FileDB instance.

###ASP.NET MVC Example

Below is a basic example to store/retrieve/delete information in a ASP.NET MVC Controller

```C#
private string pathDB = @"C:\Temp\MvcDemo.fdb";

// Uploading a file
[HttpPost]
public ActionResult Upload()
{
    HttpPostedFileBase file = Request.Files[0] as HttpPostedFileBase;

    if (file.ContentLength > 0)
        FileDB.Store(pathDB, file.FileName, file.InputStream);

    return RedirectToAction("Index");
}

// Download
[HttpGet]
public ActionResult Download(string id)
{
    // Using a classic way to download a file, instead of FileContentResult mode. 
    // Optimizing for big files. Your webserver will not consume CPU/Memory to download this file (even very large files)

    using (var db = new FileDB(pathDB, FileAccess.Read))
    {
        var info = db.Search(Guid.Parse(id));

        Response.Buffer = false;
        Response.BufferOutput = false;
        Response.ContentType = info.MimeType;
        Response.AppendHeader("Content-Length", info.FileLength.ToString());
        Response.AppendHeader("content-disposition", "attachment; filename=" + info.FileName);

        db.Read(info.ID, Response.OutputStream);

        return new EmptyResult();
    }
}
```

### Why?

Well, all web developers already had this problem: "I need to store same user files (photos, documents, ...) and I need to save them on my web server. But where? File system or database?" (See some discussion in http://stackoverflow.com/questions/3748/storing-images-in-db-yea-or-nay#3756)

The problem is: the database was not designed to store files. ADO.NET doesn't have a good method to work with streams (only byte[]). For small files (less then 100K) it works very nice. But what happens with a 10Mb file? 100Mb? It's terrible! The solution: work on filesystem!

The filesystem has a problem too: what if my user wants to upload 300 files? And what if I have 2000 users? You will have thousands of files to backup and manage, and this will be a pain.
My idea: a single archive (or a single archive per user) to store only user uploaded files. I use the SQL Server database to store the ID file reference (GUID) and FileDB does the rest of the job, storing and managing the files and bytes.

## How FileDB works?

FileDB was designed to be fast, very simple, file-based and works with all file sizes. FileDB doesn't consume lots of memory because it works only with streams (no byte[]). The data structure is built with two kinds of pages: IndexPage and DataPage.

- The IndexPage stores information about the file descriptor and it's organized in a binary tree structure.
- The DataPage stores the file bytes.

Both pages have 4096 bytes (plus 100 bytes to file header). Each page has its own header to store information about the data inside the page.

You can delete a file inside the archive and the empty data pages will be used on next insert. Or you can shrink database to get your non-used bytes.

FileDB caches (in memory) only index pages, to be faster on a second search.

## Limitations

I've made many tests to check performance and limitations with all file sizes. To protect against many clients changing data on same archive, FileDB uses read share mode. This way many users can search/read simultaneously but only one user can write (store/delete) at a time. FileDB class also implements IDisposable so you can use it inside a using code.

```C#
using(var db = new FileDB(pathDB, FileAccess.ReadWrite))
{
    db.Store(@"C:\Temp\MyPhoto.jpg");
}
```

The data size limitations are based on .NET MaxValue constants. FileDB works with UInt32 (4 bytes unsigned), which limits each file to 4GB and the database to 16TB (4096 Pages * UInt32.MaxValue).
