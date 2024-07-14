using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Book
    {

        // Once a book has been entered into the record, we know that its title, author, genre, and id cannot change defacto

        private int bookId;
        private bool isAvailable;
        private string title, author, genre;

        public Book(int bookId, bool isAvailable, string title, string author, string genre)
        {
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

        public int getBookID()
        {
            return bookId;
        }

        public bool getIsAvailable() { return isAvailable; }
        public void setIsAvailable(bool isAvailable) { this.isAvailable = isAvailable; }

        public string getTitle() { return title; }

        public string getAuthor() { return author; }

        public string getGenre() { return genre; }

        public void print()
        {
            Console.Write($"\nID: {this.bookId}");
            Console.Write($"\nTitle: {this.title}");
            Console.Write($"\nAuthor: {this.author}");
            Console.Write($"\nGenre: {this.genre}");
            Console.Write($"\nIs Available: {this.isAvailable.ToString()}");
            Console.Write("\n");
        }
    }
}
