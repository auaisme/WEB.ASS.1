
using Microsoft.Data.SqlClient;
using System.IO;


namespace DAL
{
    internal class Injector() { public static void Main() { Console.WriteLine("DAL init"); } }

    public class Library {
        private List<Book> books;
        private List<Borrower> borrowers;
        private List<Transaction> transactions;

        private const string CONNECTION_STRING = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        private SqlConnection connection;


        public Library() {
            books = new List<Book>();
            borrowers = new List<Borrower>();
            transactions = new List<Transaction>();

            connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            this.load();
        }

        ~Library() {
            connection.Close();
        }

        public void printCachedBorrowers() {
            foreach (Borrower borrower in borrowers)
                borrower.print();
        }

        public void printCachedTransactions() {
            foreach (Transaction transaction in transactions)
                transaction.print();
        }

        public int getLatestBookID() {
            string query = "SELECT MAX(ID) FROM BOOKS";
            SqlCommand cmd = new SqlCommand(query, connection);
            return int.Parse(cmd.ExecuteScalar().ToString());
        }

        public int getLatestBorrowerID() {
            string query = "SELECT MAX(ID) FROM BORROWERS";
            SqlCommand cmd = new SqlCommand(query, connection);
            return int.Parse(cmd.ExecuteScalar().ToString());
        }

        public int getLatestTransactionID() {
            string query = "SELECT MAX(ID) FROM TRANSACTIONS";
            SqlCommand cmd = new SqlCommand(query, connection);
            return int.Parse(cmd.ExecuteScalar().ToString());
        }

        public void addBook(Book book) {
            Book match = this.getBookByID(book.getBookID());

            if (match.getBookID() != -1) { Console.WriteLine("Failed, already in DB"); return; }; // TODO replace with try-catch

            this.saveBooks(book);
            book = new Book(this.getLatestBookID(), book.getIsAvailable(), book.getTitle(), book.getAuthor(), book.getGenre());
            this.books.Add(book);
        }
   
        public void removeBook(int bookID) {
            foreach (Book book in books) {
                if (book.getBookID() != bookID) continue;

                this.books.Remove(book);
                Console.WriteLine($"{this.remove("BOOKS", book.getBookID())} rows deleted");
                return;
            }
        }

        public void updateBook(Book book) {
            int bookID = book.getBookID(), size = this.books.Count();

            for (int i = 0; i < size; i++) {
                if (this.books[i].getBookID() == bookID) {
                    this.books[i] = book;
                    Console.WriteLine($"{this.updateBookInDB(book)} items updated");
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

        private Borrower getBorrowerByID(int id) {
            foreach (Borrower borrower in borrowers) {
                if (borrower.getBorrowerId() == id) return borrower;
            }
            return new Borrower(
                    borrowerId: -1,
                    name: "null",
                    email: "none@none.na"
                );
        }

        public void registerBorrower(Borrower borrower) {
            foreach (Borrower inDB in this.borrowers) {
                if (inDB.getBorrowerId() == borrower.getBorrowerId()) {
                    Console.WriteLine("Failed, already in DB");
                    return;
                }
            }

            this.saveBorrowers(borrower);
            borrower = new Borrower(this.getLatestBorrowerID(), borrower.getName(), borrower.getEmail());
            this.borrowers.Add(borrower);
        }

        public void updateBorrower(Borrower borrower) {
            int size = this.borrowers.Count();
            for (int i = 0; i < size; i++) {
                if (this.borrowers[i].getBorrowerId() == borrower.getBorrowerId()) {
                    this.borrowers[i] = borrower;
                    Console.WriteLine($"{this.updateBorrowerInDB(borrower)} items updated");
                    return;
                }
            }

            Console.WriteLine("No matching borrower in DB");
        }

        public void deleteBorrower(int borrowerId) {
            foreach (Borrower borrower in borrowers) {
                if (borrower.getBorrowerId() == borrowerId) {
                    this.borrowers.Remove(borrower);
                    Console.WriteLine($"{this.remove("BORROWERS", borrower.getBorrowerId())} items deleted");
                    return;
                }
            }
        }

        public void recordTransaction(Transaction transaction) {
            // TODO not all checks implemented
            Book book = this.getBookByID(transaction.getBookId());
            Borrower borrower = this.getBorrowerByID(transaction.getBorrowerId());

            if (borrower.getBorrowerId() == -1) { Console.WriteLine("Given borrower doesn't exist"); return; }

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

            if (transaction.getIsBorrowed())
            {
                this.updateBook(new Book(book.getBookID(), false, book.getTitle(), book.getAuthor(), book.getGenre()));
            }
            else {
                this.updateBook(new Book(book.getBookID(), true, book.getTitle(), book.getAuthor(), book.getGenre()));
            }

            this.saveTransactions(transaction);
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

        private void saveBooks(Book book)
        {
            string query = $"INSERT INTO BOOKS (TITLE, AUTHOR, GENRE, IS_AVAILABLE) VALUES (@title, @author, @genre, @is_available)";

            SqlParameter title = new SqlParameter("@title", book.getTitle());
            SqlParameter author = new SqlParameter("@author", book.getAuthor());
            SqlParameter genre = new SqlParameter("@genre", book.getGenre());
            SqlParameter isAvailable = new SqlParameter("@is_available", book.getIsAvailable().ToString());

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add(title);
            command.Parameters.Add(author);
            command.Parameters.Add(genre);
            command.Parameters.Add(isAvailable);

            command.ExecuteNonQuery();

            Console.WriteLine("Saved Book");
        }

        private int updateBookInDB(Book book) {
            string query = $"UPDATE BOOKS SET TITLE = @title, AUTHOR = @author, GENRE = @genre, IS_AVAILBLE = @is_available WHERE ID = @id";

            SqlParameter id = new SqlParameter("@id", book.getBookID());
            SqlParameter title = new SqlParameter("@title", book.getTitle());
            SqlParameter author = new SqlParameter("@author", book.getAuthor());
            SqlParameter genre = new SqlParameter("@genre", book.getGenre());
            SqlParameter isAvailable = new SqlParameter("@is_available", book.getIsAvailable().ToString());

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add(id);
            command.Parameters.Add(title);
            command.Parameters.Add(author);
            command.Parameters.Add(genre);
            command.Parameters.Add(isAvailable);

            return command.ExecuteNonQuery();
        }

        private void saveBorrowers(Borrower borrower) {
            string query = $"INSERT INTO BORROWERS (NAME, EMAIL) VALUES (@name, @email)";

            SqlParameter name = new SqlParameter("@name", borrower.getName());
            SqlParameter email = new SqlParameter("@email", borrower.getEmail());

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add(name);
            command.Parameters.Add(email);

            command.ExecuteNonQuery();

            Console.WriteLine("Saved Borrower");
        }

        private int updateBorrowerInDB(Borrower borrower) {
            string query = $"UPDATE BORROWERS SET NAME = @name, EMAIL = @email WHERE ID = @id";

            SqlParameter id = new SqlParameter("@id", borrower.getBorrowerId());
            SqlParameter name = new SqlParameter("@name", borrower.getName());
            SqlParameter email = new SqlParameter("@email", borrower.getEmail());

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add(id);
            command.Parameters.Add(name);
            command.Parameters.Add(email);

            return command.ExecuteNonQuery();
        }

        private void saveTransactions(Transaction transaction) {
            string query = $"INSERT INTO TRANSACTIONS (BOOK_ID, BORROWER_ID, IS_BORROWED, DATE) VALUES (@book_id, @borrower_id, @is_borrowed, @date)";

            SqlParameter bookId = new SqlParameter("@book_id", transaction.getBookId());
            SqlParameter borrowerId = new SqlParameter("@borrower_id", transaction.getBorrowerId());
            SqlParameter isBorrowed = new SqlParameter("@is_borrowed", transaction.getIsBorrowed().ToString());
            SqlParameter date = new SqlParameter("@date", transaction.getDate());

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add(bookId);
            command.Parameters.Add(borrowerId);
            command.Parameters.Add(isBorrowed);
            command.Parameters.Add(date);

            command.ExecuteNonQuery();

            Console.WriteLine("Saved Transaction");
        }

        private int updateTransactionInDB(Transaction transaction) {
            string query = $"UPDATE TRANSACTIONS SET BOOK_ID = @book_id, BORROWER_ID = @borrower_id, IS_BORROWED = @is_borrowed, DATE = @date WHERE ID = @id";

            SqlParameter id = new SqlParameter("@id", transaction.getTransactionId());
            SqlParameter bookId = new SqlParameter("@book_id", transaction.getBookId());
            SqlParameter borrowerId = new SqlParameter("@borrower_id", transaction.getBorrowerId());
            SqlParameter isBorrowed = new SqlParameter("@is_borrowed", transaction.getIsBorrowed().ToString());
            SqlParameter date = new SqlParameter("@date", transaction.getDate());

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add(id);
            command.Parameters.Add(bookId);
            command.Parameters.Add(borrowerId);
            command.Parameters.Add(isBorrowed);
            command.Parameters.Add(date);

            return command.ExecuteNonQuery();
        }

        private int remove(string table, int id) {
            string query = $"DELETE FROM {table} WHERE ID = @id";
            SqlParameter idParameter = new SqlParameter("id", id);
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.Add(idParameter);
            return cmd.ExecuteNonQuery();
        }

        public void load() {
            string query = "SELECT * FROM BOOKS";

            SqlCommand cmd = new SqlCommand(query, connection);

            SqlDataReader reader = cmd.ExecuteReader();
            
            Console.WriteLine("Loading books");

            while (reader.Read()) {
                int id = int.Parse(reader[0].ToString());
                string title = reader[1].ToString();
                string author = reader[2].ToString();
                string genre = reader[3].ToString();
                bool isAvailable = bool.Parse(reader[4].ToString());
                Book book = new Book(id, isAvailable, title, author, genre);
                books.Add(book);
            }

            reader.Close();

            query = "SELECT * FROM BORROWERS";

            cmd = new SqlCommand(query, connection);

            reader = cmd.ExecuteReader();

            Console.WriteLine("Loading borrowers");

            while (reader.Read()) {
                int id = int.Parse(reader[0].ToString());
                string name = reader[1].ToString();
                string email = reader[2].ToString();
                Borrower borrower = new Borrower(id, name, email);
                borrowers.Add(borrower);
            }

            reader.Close();

            query = "SELECT * FROM TRANSACTIONS";

            cmd = new SqlCommand(query, connection);

            reader = cmd.ExecuteReader();

            Console.WriteLine("Loading transactions");

            while (reader.Read()) {
                int id = int.Parse(reader[0].ToString());
                int book = int.Parse(reader[1].ToString());
                int borrower = int.Parse(reader[2].ToString());
                bool isBorrowed = bool.Parse(reader[3].ToString());
                DateTime date = DateTime.Parse(reader[4].ToString());
                Transaction transaction = new Transaction(id, book, borrower, isBorrowed, date);
                transactions.Add(transaction);
            }

            reader.Close();
            Console.WriteLine("Finished Loading Data");
        }

        private void uploadALlDataToDB()
        {
            // books write

            foreach (Book book in this.books) {
                string query = $"INSERT INTO BOOKS (TITLE, AUTHOR, GENRE, IS_AVAILABLE) VALUES (@title, @author, @genre, @is_available)";

                SqlParameter title = new SqlParameter("@title", book.getTitle());
                SqlParameter author = new SqlParameter("@author", book.getAuthor());
                SqlParameter genre = new SqlParameter("@genre", book.getGenre());
                SqlParameter isAvailable = new SqlParameter("@is_available", book.getIsAvailable().ToString());

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add(title);
                command.Parameters.Add(author);
                command.Parameters.Add(genre);
                command.Parameters.Add(isAvailable);

                command.ExecuteNonQuery();
            }

            // executeNonQuery => DML
            // executeReader => SELECT
            // executeScaler => single item returning select like select max() from t;

            // borrowers write

            foreach (Borrower borrower in this.borrowers) {
                string query = $"INSERT INTO BORROWERS (NAME, EMAIL) VALUES (@name, @email)";

                SqlParameter name = new SqlParameter("@name", borrower.getName());
                SqlParameter email = new SqlParameter("@email", borrower.getEmail());

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add(name);
                command.Parameters.Add(email);

                command.ExecuteNonQuery();
            }

            // transactions write

            foreach (Transaction transaction in this.transactions) {
                string query = $"INSERT INTO TRANSACTIONS (BOOK_ID, BORROWER_ID, IS_BORROWED, DATE) VALUES (@book_id, @borrower_id, @is_borrowed, @date)";

                SqlParameter bookId = new SqlParameter("@book_id", transaction.getBookId());
                SqlParameter borrowerId = new SqlParameter("@borrower_id", transaction.getBorrowerId());
                SqlParameter isBorrowed = new SqlParameter("@is_borrowed", transaction.getIsBorrowed().ToString());
                SqlParameter date = new SqlParameter("@date", transaction.getDate());

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add(bookId);
                command.Parameters.Add(borrowerId);
                command.Parameters.Add(isBorrowed);
                command.Parameters.Add(date);

                command.ExecuteNonQuery();
            }
        }

        public void displayAllFromDB() {
            string query = "SELECT * FROM BOOKS";

            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader reader = command.ExecuteReader();

            Console.Write("Books:\n");
            while (reader.Read()) {
                Console.WriteLine($"ID: {reader[0]}, Title: {reader[1]}, Author: {reader[2]}, Genre: {reader[3]}, Available: {reader[4]}");
            }
            Console.Write("\n");
            reader.Close();

            query = "SELECT * FROM BORROWERS";

            command = new SqlCommand(query, connection);

            reader = command.ExecuteReader();

            Console.Write("Borrowers:\n");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader[0]}, Name: {reader[1]}, Email: {reader[2]}");
            }
            Console.Write("\n");
            reader.Close();

            query = "SELECT * FROM TRANSACTIONS";

            command = new SqlCommand(query, connection);

            reader = command.ExecuteReader();

            Console.Write("Transactions:\n");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader[0]}, Book ID: {reader[1]}, Borrower ID: {reader[2]}, Borrowed: {reader[3]}, Date: {reader[4]}");
            }
            Console.Write("\n");
            reader.Close();
        }
    }
}
