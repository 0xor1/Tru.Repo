using System;
using System.Diagnostics;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tru.Repo.Exceptions;
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
        public void Repos_should_patch_the_db_when_they_are_instantiated()
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

        [TestMethod]
        public void Repos_should_only_patch_on_the_first_instantiation()
        {
            var stopwatch = new Stopwatch();

            //will take many orders of magnitude longer than the second instantiation
            stopwatch.Start();
            new CustomerRepo(TestConnectionString);
            var timeToCreate = stopwatch.Elapsed;

            stopwatch.Restart();
            new CustomerRepo(TestConnectionString);
            var timeToRecreate = stopwatch.Elapsed;

            Assert.IsTrue(timeToCreate > timeToRecreate);
        }

        [TestMethod]
        [ExpectedException(typeof(NestedPatchSequenceError))]
        public void Repos_should_throw_NestedPatchSequenceError_if_they_make_nested_calls_to_PatchSequence()
        {
            new NestedPatchSequenceRepo(TestConnectionString);
        }

        [TestMethod]
        [ExpectedException(typeof(PatchCalledOutsidePatchSequenceError))]
        public void Repos_should_throw_PatchCalledOutsidePatchSequenceError_if_they_make_calls_to_Patch_outside_of_PatchSequence()
        {
            new PatchCalledOutsidePatchSequenceRepo(TestConnectionString);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRepoNameError))]
        public void Repos_should_throw_InvalidRepoNameError_if_the_repo_name_is_null()
        {
            new NullRepoNameRepo(TestConnectionString);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRepoNameError))]
        public void Repos_should_throw_InvalidRepoNameError_if_the_repo_name_is_only_white_space_characters()
        {
            new WhiteSpaceRepoNameRepo(TestConnectionString);
        }  
    }
}
