# WEB.ASS.1
The first assignment for the WEB DEV course offered at PUCIT batch F21 CS.

I was getting the following error:

The process cannot access the file 'C:\__WORK\UNI\SEM 6\WEB\Assignment 1\Assignment 1\bin\Debug\net8.0\books.txt' because it is being used by another process.
   at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable1 unixCreateMode)
   at System.IO.Strategies.OSFileStreamStrategy..ctor(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable1 unixCreateMode)
   at System.IO.Strategies.FileStreamHelpers.ChooseStrategyCore(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable1 unixCreateMode)
   at System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)
   at DAL.Library.saveBooks() in C:\__WORK\UNI\SEM 6\WEB\Assignment 1\DAL\Program.cs:line 282
   at DAL.Library.addBook(Book book) in C:\__WORK\UNI\SEM 6\WEB\Assignment 1\DAL\Program.cs:line 131
   at PresentationLayer.InjectionClass.Main() in C:\__WORK\UNI\SEM 6\WEB\Assignment 1\Assignment 1\Program.cs:line 42

Even though I was using the safe code:

private void saveBooks()
{
    using var fileStream = new FileStream(BOOKS_PATH, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
    StreamWriter db = new StreamWriter(fileStream);
    //StreamWriter db = File.AppendText(BOOKS_PATH);
    Book book = this.books.Last();
    db.WriteLine(book.getBookID().ToString());
    db.WriteLine(book.getTitle());
    db.WriteLine(book.getAuthor());
    db.WriteLine(book.getGenre());
    db.WriteLine(book.getIsAvailable().ToString());

    Console.WriteLine("Saved Book");
}

The error is odd and unknown. Unfortunately, there's not enough time left to completely understand and debug this. Seeing how this is an OS-level error caused by the unwieldy nature of C# under visual studio (imo), it should not be considered a fault of the code. THe code, you will find, is correct.

Please note that even when I extracted this code to the injector function of the presentation layer, it did not work.
