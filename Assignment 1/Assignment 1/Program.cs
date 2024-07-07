
using DAL;
using System.Text.RegularExpressions;

namespace PresentationLayer {
    class InjectionClass {
        public static void Main() {

            Library library = new Library();

            while (true)
            {
                Console.WriteLine("Welcome! What would you like to do?");
                Console.Write("1. Add a new book\r\n2. Remove a book\r\n3. Update a book\r\n4. Register a new borrower\r\n5. Update a borrower\r\n6. Delete a borrower\r\n7. Borrow a book\r\n8. Return a book\r\n9. Search for books by title, author, or genre\r\n10.View all books\r\n11.View borrowed books by a specific borrower\r\n12.Exit the application\r\n");
                Console.Write(">> ");
                int choice = int.Parse(Console.ReadLine().Trim());
                try {
                    switch (choice)
                    {
                        case 1:
                            Console.Write("ID: ");
                            int id = int.Parse(Console.ReadLine());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Title: ");
                            string title = Console.ReadLine().Trim();
                            if (title.Length == 0 || title.Contains(' ') || title.Contains('\t')) { Console.WriteLine("Invalid Title! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Author: ");
                            string author = Console.ReadLine().Trim();
                            if (author.Length == 0 || author.Contains(' ') || author.Contains('\t')) { Console.WriteLine("Invalid Author! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }


                            Console.Write("Genre: ");
                            string genre = Console.ReadLine().Trim();
                            if (genre.Length == 0 || genre.Contains(' ') || genre.Contains('\t')) { Console.WriteLine("Invalid Genre! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }


                            Console.Write("Is Available: ");
                            bool isAvailable = bool.Parse(Console.ReadLine().Trim());

                            library.addBook(new Book(id, isAvailable, title, author, genre));
                            break;

                        case 2:
                            Console.Write("ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            library.removeBook(id);
                            break;

                        case 3:
                            Console.Write("ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Title: ");
                            title = Console.ReadLine().Trim();
                            if (title.Length == 0 || title.Contains(' ') || title.Contains('\t')) { Console.WriteLine("Invalid Title! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Author: ");
                            author = Console.ReadLine().Trim();
                            if (author.Length == 0 || author.Contains(' ') || author.Contains('\t')) { Console.WriteLine("Invalid Author! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }


                            Console.Write("Genre: ");
                            genre = Console.ReadLine().Trim();
                            if (genre.Length == 0 || genre.Contains(' ') || genre.Contains('\t')) { Console.WriteLine("Invalid Genre! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }


                            Console.Write("Is Available: ");
                            isAvailable = bool.Parse(Console.ReadLine().Trim());

                            library.updateBook(new Book(id, isAvailable, title, author, genre));
                            break;

                        case 4:
                            Console.Write("ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Name: ");
                            string name = Console.ReadLine().Trim();
                            if (name.Length == 0 || name.Contains(' ') || name.Contains('\t')) { Console.WriteLine("Invalid Name! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Email: ");
                            string email = Console.ReadLine().Trim();
                            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                            if (email.Length == 0 || email.Contains(' ') || email.Contains('\t') || !isEmail) { Console.WriteLine("Invalid Email! It must be in valid email format\n Press enter key to continue;"); Console.ReadLine(); break; }

                            library.registerBorrower(new Borrower(id, name, email));
                            break;

                        case 5:
                            Console.Write("ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Name: ");
                            name = Console.ReadLine().Trim();
                            if (name.Length == 0 || name.Contains(' ') || name.Contains('\t')) { Console.WriteLine("Invalid Name! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Email: ");
                            email = Console.ReadLine().Trim();
                            isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                            if (email.Length == 0 || email.Contains(' ') || email.Contains('\t') || !isEmail) { Console.WriteLine("Invalid Email! It must be in valid email format\n Press enter key to continue;"); Console.ReadLine(); break; }

                            library.updateBorrower(new Borrower(id, name, email));
                            break;

                        case 6:
                            Console.Write("ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            library.deleteBorrower(id);
                            break;

                        case 7:
                            Console.Write("Book ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Borrower ID: ");
                            int id2 = int.Parse(Console.ReadLine().Trim());
                            if (id2 <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            bool isBorrowed = true;

                            library.recordTransaction(new Transaction(library.getTransactionsCount() + 1, id, id2, isBorrowed, DateTime.Now));
                            break;

                        case 8:
                            Console.Write("Book ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            Console.Write("Borrower ID: ");
                            id2 = int.Parse(Console.ReadLine().Trim());
                            if (id2 <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            isBorrowed = false;

                            library.recordTransaction(new Transaction(library.getTransactionsCount() + 1, id, id2, isBorrowed, DateTime.Now));
                            break;

                        case 9:
                            Console.Write("Title, Author, or Genre: ");
                            string query = Console.ReadLine().Trim();
                            if (query.Length == 0 || query.Contains(' ') || query.Contains('\t')) { Console.WriteLine("Invalid Query! It must be 1 word\n Press enter key to continue;"); Console.ReadLine(); break; }

                            List<Book> books = library.searchBooks(query);
                            foreach (Book book in books) book.print();
                            break;

                        case 10:
                            books = library.getAllBooks(); // C# has a garbage collector, no need to worry about dangling pointers
                            foreach (Book book in books) book.print();
                            break;

                        case 11:
                            Console.Write("ID: ");
                            id = int.Parse(Console.ReadLine().Trim());
                            if (id <= 0) { Console.WriteLine("ID must be positive!\n Press enter key to continue;"); Console.ReadLine(); break; }

                            books = library.getBorrowedBooksByBorrower(id);
                            break;

                        case 12:
                            return;

                        default:
                            Console.WriteLine("Please pick one of the given options!");
                            break;
                    } // end of switch
                } // end of try
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.Error.WriteLine(e.StackTrace);
                } // end of catch
            } // end of while
        } // end of main
    } // end of class
} // end of namespace