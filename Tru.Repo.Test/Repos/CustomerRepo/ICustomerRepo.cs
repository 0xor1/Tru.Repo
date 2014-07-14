using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tru.Repo.Test.Entities;

namespace Tru.Repo.Test.Repos
{
    public interface ICustomerRepo: IRepo
    {
        void Save(Customer customer);
        void Delete(Customer customer);
        Customer GetWithEmail(string email);
        IEnumerable<Customer> GetWithFirstName(string firstName);
        IEnumerable<Customer> GetWithLastName(string lastName);
    }
}
