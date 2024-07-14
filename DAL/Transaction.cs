using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Transaction
    {
        private int transactionId, bookId, borrowerId;
        private bool isBorrowed;
        private DateTime date;

        public Transaction(int transactionId, int bookId, int borrowerId, bool isBorrowed, DateTime date)
        {
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

        public void print()
        {
            Console.WriteLine($"ID: {transactionId}, Book ID: {bookId}, Borrower ID: {borrowerId}, Borrowed: {isBorrowed}, Date: {date}");
        }
    }
}
