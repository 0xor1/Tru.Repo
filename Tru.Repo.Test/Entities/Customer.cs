using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tru.Repo.Test.Entities
{
    public class Customer
    {
        private string _Email;
        public string Email
        {
            get
            {
                return _Email;
            }
            private set
            {
                if (value.Length > 100)
                {
                    throw new InvalidOperationException("Email may not be longer than 100 characters long.");
                }
                _Email = value;
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Customer(string email, string firstName, string lastName)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
