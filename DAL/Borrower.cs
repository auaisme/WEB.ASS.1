using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Borrower
    {
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

        public void print()
        {
            Console.WriteLine($"ID: {borrowerId}, Name: {name}, Email: {email}");
        }
    }
}
