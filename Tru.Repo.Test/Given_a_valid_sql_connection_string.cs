using System;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tru.Repo.Test.Repos;
using Tru.Repo.Test.Entities;

namespace Tru.Repo.Test
{
    [TestClass]
    public class Given_a_valid_sql_connection_string
    {
        private const string TestConnectionString = @"Server=localhost\SQL2014EXPRESS64; Database=RepoTest; Trusted_Connection=True; Pooling=False";
        private const string MasterConnectionString = @"Server=localhost\SQL2014EXPRESS64; Database=master; Trusted_Connection=True;";

        [TestInitialize]
        public void Initialize()
        {
            using(var connection = new SqlConnection(MasterConnectionString))
            using (var command = new SqlCommand(TestScript.Initialize, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new SqlConnection(MasterConnectionString))
            using (var command = new SqlCommand(TestScript.Cleanup, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Repos_should_initialise_all_the_necessary_db_objects_when_they_are_instantiated()
        {
            var repo = new CustomerRepo(TestConnectionString);
            var joe = new Customer("joe@mail.domain", "joe", "blogs");
            
            repo.Save(joe);

            var retrievedCustomer = repo.GetWithEmail("joe@mail.domain");
            Assert.IsTrue(joe.Email == retrievedCustomer.Email);

            retrievedCustomer = repo.GetWithFirstName("joe").SingleOrDefault();
            Assert.IsTrue(joe.Email == retrievedCustomer.Email);

            retrievedCustomer = repo.GetWithLastName("blogs").SingleOrDefault();
            Assert.IsTrue(joe.Email == retrievedCustomer.Email);

            retrievedCustomer = repo.GetWithLastName("nobody").SingleOrDefault();
            Assert.IsNull(retrievedCustomer);
        }
    }
}
