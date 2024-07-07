
using System.IO;


namespace DAL
{
    internal class Injector() { public static void Main() { Console.WriteLine("DAL init"); } }

    public class Book {

        // Once a book has been entered into the record, we know that its title, author, genre, and id cannot change defacto

        private int bookId;
        private bool isAvailable;
        private string title, author, genre;

        public Book(int bookId, bool isAvailable, string title, string author, string genre) {
            // ASSUMPTION MADE: all strings are exactly 1 word long

            this.bookId = bookId;

            author = author.Trim();
            author = author.ToLower();
            this.author = author;

            this.isAvailable = isAvailable;
            
            title = title.Trim();
            title = title.ToLower();
            this.title = title;

            genre = genre.Trim();
            genre = genre.ToLower();
            this.genre = genre;
        }

        public int getBookID() {
            return bookId;
        }

        public bool getIsAvailable() { return isAvailable; }
        public void setIsAvailable(bool isAvailable) { this.isAvailable = isAvailable; }

        public string getTitle() { return title; }

        public string getAuthor() { return author; }

        public string getGenre() { return genre; }

        public void print() {
            Console.Write("\nID: ", bookId);
            Console.Write("\nTitle: ", title);
            Console.Write("\nAuthor: ", author);
            Console.Write("\nGenre: ", genre);
            Console.Write("\nIs Available: ", isAvailable);
        }
    }

    public class Borrower {
        private int borrowerId;
        private string name, email;

        public Borrower(int borrowerId, string name, string email)
        {
            this.borrowerId = borrowerId;
            
            name = name.Trim();
            name = name.ToLower();
            this.name = name;
            
            email = email.Trim();
            email = email.ToLower();
            this.email = email;
        }

        public int getBorrowerId() { return borrowerId; }

        public string getName() { return name; }

        public string getEmail() { return email; }
    }

    public class Transaction {
        private int transactionId, bookId, borrowerId;
        private bool isBorrowed;
        private DateTime date;

        public Transaction(int transactionId, int bookId, int borrowerId, bool isBorrowed, DateTime date) {
            this.transactionId = transactionId;
            this.bookId = bookId;
            this.borrowerId = borrowerId;
            this.isBorrowed = isBorrowed;
            this.date = date;
        }

        public int getTransactionId() { return transactionId; }

        public int getBookId() { return bookId; }

        public int getBorrowerId() { return borrowerId; }

        public bool getIsBorrowed() { return isBorrowed; }

        public DateTime getDate() { return date; }
    }

    public class Library {
        private List<Book> books;
        private List<Borrower> borrowers;
        private List<Transaction> transactions;

        private const string BOOKS_PATH = "books.txt";
        private const string BORROWERS_PATH = "borrowers.txt";
        private const string TRANSACTIONS_PATH = "transactions.txt";

        public Library() {
            books = new List<Book>();
            borrowers = new List<Borrower>();
            transactions = new List<Transaction>();
            this.load();
        }

        public int getTransactionsCount() { return this.transactions.Count(); }

        public void addBook(Book book) {
            Book match = this.getBookByID(book.getBookID());

            if (match.getBookID() != -1) { Console.WriteLine("Failed, already in DB"); return; }; // TODO replace with try-catch

            this.books.Add(book);
            this.saveBooks();
        }
   
        public void removeBook(int bookID) {
            foreach (Book book in books) {
                if (book.getBookID() != bookID) continue;

                this.books.Remove(book);
                this.saveBooks(true);
                return;
            }
        }

        public void updateBook(Book book) {
            int bookID = book.getBookID(), size = this.books.Count();

            for (int i = 0; i < size; i++) {
                if (this.books[i].getBookID() == bookID) {
                    this.books[i] = book;
                    this.saveBooks(true);
                    return;
                }
            }

            Console.WriteLine("Not found in DB");
        }

        public List<Book> getAllBooks() { List<Book> returnMe = books; return returnMe; } // Done this way to avoid direct reference

        public Book getBookByID(int bookId) {
            foreach (Book book in books) {
                if (book.getBookID() == bookId) return book;
            }
            // TODO: throw an exception
            return new Book(-1, false, "", "", ""); // this represents an invalid object
        }

        public List<Book> searchBooks(string query) {
            // ASSUMPTIONS MADE: query will contain a string that can either be:
            // * the name of the author
            // * the title of the book
            // * the genre of the book
            // The query will not contain anything other than the aforementioned content
            // The query will only be 1 word long
            // The query should return any and all possible exact matches
            
            query = query.Trim();
            query = query.ToLower();

            List<Book> matches = new List<Book>();

            foreach (Book book in books) {
                if (this.compare(book.getGenre(), query) || this.compare(book.getAuthor(), query) || this.compare(book.getTitle(), query)) {
                    matches.Add(book);
                }
            }

            return matches;
        }

        public void registerBorrower(Borrower borrower) {
            foreach (Borrower inDB in this.borrowers) {
                if (inDB.getBorrowerId() == borrower.getBorrowerId()) {
                    Console.WriteLine("Failed, already in DB");
                    return;
                }
            }

            this.borrowers.Add(borrower);
            this.saveBorrowers();
        }

        public void updateBorrower(Borrower borrower) {
            int size = this.borrowers.Count();
            for (int i = 0; i < size; i++) {
                if (this.borrowers[i].getBorrowerId() == borrower.getBorrowerId()) {
                    this.borrowers[i] = borrower;
                    this.saveBorrowers(true);
                    return;
                }
            }

            Console.WriteLine("No matching borrower in DB");
        }

        public void deleteBorrower(int borrowerId) {
            foreach (Borrower borrower in borrowers) {
                if (borrower.getBorrowerId() == borrowerId) {
                    this.borrowers.Remove(borrower);
                    this.saveBorrowers(true);
                    return;
                }
            }
        }

        public void recordTransaction(Transaction transaction) {
            Book book = this.getBookByID(transaction.getBookId());
            
            if (book.getBookID() == -1) { Console.WriteLine("Referenced book doesn't exist"); return; }

            if (book.getIsAvailable() == false && transaction.getIsBorrowed()) { Console.WriteLine("Book is not availble"); return; }

            if (book.getIsAvailable() && transaction.getIsBorrowed() == false) { Console.WriteLine("Book has already been returned"); return; }

            foreach (Transaction inDB in transactions) {
                if (inDB.getTransactionId() == transaction.getTransactionId()) {
                    Console.WriteLine("Given transaction already exists in DB");
                    return;
                }
            }

            this.transactions.Add(transaction);

            this.saveTransactions();
        }

        public List<Book> getBorrowedBooksByBorrower(int borrowerId) {
            List<Transaction> searchSpace = this.getBorrowedBooksByBorrower(borrowerId, true);

            List<Book> matches = new List<Book>();

            foreach (Book book in books) {
                foreach (Transaction transaction in searchSpace) {
                    if (book.getBookID() == transaction.getBookId()) { matches.Add(book); break; }
                }
            }

            return matches;
        }
        public List<Transaction> getBorrowedBooksByBorrower(int borrowerId, bool transactionsRetrievalFlag) {
            List<Transaction> specificTransactions = new List<Transaction>();

            foreach (Transaction transaction in transactions) {
                if (transaction.getBorrowerId() == borrowerId) specificTransactions.Add(transaction);
            }

            return specificTransactions;
        }

        public bool compare(string left, string right) { 
            if (left.Length != right.Length) return false;

            for (int i = 0; i < left.Length; i++) {
                if (left[i] != right[i]) {  return false; }
            }

            return true;
        }

        private void saveBooks()
        {
            StreamWriter db = File.AppendText(BOOKS_PATH);
            Book book = this.books.Last();
            db.WriteLine(book.getBookID().ToString());
            db.WriteLine(book.getTitle());
            db.WriteLine(book.getAuthor());
            db.WriteLine(book.getGenre());
            db.WriteLine(book.getIsAvailable().ToString());

            Console.WriteLine("Saved Book");
        }

        private void saveBooks(bool overwriteFlag) {
            StreamWriter db = File.CreateText(BOOKS_PATH);

            foreach (Book book in books) {
                db.WriteLine(book.getBookID().ToString());
                db.WriteLine(book.getTitle());
                db.WriteLine(book.getAuthor());
                db.WriteLine(book.getGenre());
                db.WriteLine(book.getIsAvailable().ToString());
            }

            Console.WriteLine("Saved Books");
        }

        private void saveBorrowers() {
            StreamWriter db = File.AppendText(BORROWERS_PATH);
            Borrower borrower = this.borrowers.Last();
            db.WriteLine(borrower.getBorrowerId().ToString());
            db.WriteLine(borrower.getName());
            db.WriteLine(borrower.getEmail());

            Console.WriteLine("Saved Borrower");
        }
        private void saveBorrowers(bool overwriteFlag) {
            StreamWriter db = File.CreateText(BORROWERS_PATH);

            foreach (Borrower borrower in borrowers) {
                db.WriteLine(borrower.getBorrowerId().ToString());
                db.WriteLine(borrower.getName());
                db.WriteLine(borrower.getEmail());
            }

            Console.WriteLine("Saved Borrowers");
        }

        private void saveTransactions() {
            StreamWriter db = File.AppendText(TRANSACTIONS_PATH);
            Transaction transaction = this.transactions.Last();
            db.WriteLine(transaction.getTransactionId().ToString());
            db.WriteLine(transaction.getBookId().ToString());
            db.WriteLine(transaction.getBorrowerId().ToString());
            db.WriteLine(transaction.getIsBorrowed().ToString());
            db.WriteLine(transaction.getDate().ToString());

            Console.WriteLine("Saved Transaction");
        }
        private void saveTransactions(bool overwriteFlag) {
            StreamWriter db = File.CreateText(TRANSACTIONS_PATH);

            foreach (Transaction transaction in this.transactions) {
                db.WriteLine(transaction.getTransactionId().ToString());
                db.WriteLine(transaction.getBookId().ToString());
                db.WriteLine(transaction.getBorrowerId().ToString());
                db.WriteLine(transaction.getIsBorrowed().ToString());
                db.WriteLine(transaction.getDate().ToString());
            }

            Console.WriteLine("Saved transactions");
        }

        public void load() {
            StreamReader sr = File.OpenText(BOOKS_PATH);
            
            Console.WriteLine("Loading books");

            while (!sr.EndOfStream) {
                int id = int.Parse(sr.ReadLine().Trim());
                string title = sr.ReadLine().Trim();
                string author = sr.ReadLine().Trim();
                string genre = sr.ReadLine().Trim();
                bool isAvailable = bool.Parse(sr.ReadLine().Trim());
                Book book = new Book(id, isAvailable, title, author, genre);
                books.Add(book);
            }
            
            sr = File.OpenText(BORROWERS_PATH);

            Console.WriteLine("Loading borrowers");

            while (!sr.EndOfStream) {
                int id = int.Parse(sr.ReadLine().Trim());
                string name = sr.ReadLine().Trim();
                string email = sr.ReadLine().Trim();
                Borrower borrower = new Borrower(id, name, email);
                borrowers.Add(borrower);
            }

            sr = File.OpenText(TRANSACTIONS_PATH);

            Console.WriteLine("Loading transactions");

            while (!sr.EndOfStream) {
                int id = int.Parse(sr.ReadLine().Trim());
                int book = int.Parse(sr.ReadLine().Trim());
                int borrower = int.Parse(sr.ReadLine().Trim());
                bool isBorrowed = bool.Parse(sr.ReadLine().Trim());
                DateTime date = DateTime.Parse(sr.ReadLine().Trim());
                Transaction transaction = new Transaction(id, book, borrower, isBorrowed, date);
                transactions.Add(transaction);
            }

            Console.WriteLine("Finished Loading Data");
        }
    }
}