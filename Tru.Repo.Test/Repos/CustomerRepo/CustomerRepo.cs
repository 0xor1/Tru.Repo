using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Tru.Repo;
using Tru.Repo.Test.Entities;
using Tru.Repo.Test.Repos.Scripts;

namespace Tru.Repo.Test.Repos
{
    public class CustomerRepo: TestRepo, ICustomerRepo
    {
        private const string RepoName = "TruTest.CustomerRepo";
        private static bool HasPatched;

        public CustomerRepo(string connection)
            : base(connection)
        {
            PatchCustomerRepo();
        }

        #region ICustomerRepo
        public void Save(Customer customer)
        {
            var command = new SqlCommand(CustomerRepoScript.Save);

            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = customer.Email;
            command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100).Value = customer.FirstName;
            command.Parameters.Add("@LastName", SqlDbType.NVarChar, 100).Value = customer.LastName;

            Execute(command, () => command.ExecuteNonQuery());
        }

        public void Delete(Customer customer)
        {
            var command = new SqlCommand(CustomerRepoScript.Delete);

            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = customer.Email;

            Execute(command, () => command.ExecuteNonQuery());
        }

        public Customer GetWithEmail(string email)
        {
            var command = new SqlCommand(CustomerRepoScript.GetWithEmail);

            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
            
            Customer customer = null;
            Execute(command, () => ForEach(command.ExecuteReader(), (reader) => customer = new Customer(reader.GetString(0), reader.GetString(1), reader.GetString(2))));
           
            return customer;
        }

        public IEnumerable<Customer> GetWithFirstName(string firstName)
        {
            var command = new SqlCommand(CustomerRepoScript.GetWithFirstName);

            command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100).Value = firstName;

            List<Customer> customers = new List<Customer>();
            Execute(command, () => ForEach(command.ExecuteReader(), (reader) => customers.Add(new Customer(reader.GetString(0), reader.GetString(1), reader.GetString(2)))));

            return customers;
        }

        public IEnumerable<Customer> GetWithLastName(string lastName)
        {
            var command = new SqlCommand(CustomerRepoScript.GetWithLastName);

            command.Parameters.Add("@LastName", SqlDbType.NVarChar, 100).Value = lastName;

            List<Customer> customers = new List<Customer>();
            Execute(command, () => ForEach(command.ExecuteReader(), (reader) => customers.Add(new Customer(reader.GetString(0), reader.GetString(1), reader.GetString(2)))));

            return customers;
        }
        #endregion

        #region Patch
        private void PatchCustomerRepo()
        {
            if (!HasPatched)
            {
                PatchSequence(RepoName, () =>
                {
                    Patch(CustomerRepoScript.Patch0);
                });
                HasPatched = true;
            }
        }
        #endregion
    }
}
