using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tru.Repo.Test.Repos.Scripts;

namespace Tru.Repo.Test.Repos
{
    public class TestRepo: Repo
    {
        private const string RepoName = "TruTest";
        private static bool HasPatched;

        public TestRepo(string connectionString)
            :base(connectionString)
        {
            PatchTestRepo();
        }

        private void PatchTestRepo()
        {
            if (!HasPatched)
            {
                PatchSequence(RepoName, () =>
                {
                    Patch(TestRepoScript.Patch0);
                });
                HasPatched = true;
            }
        }
    }
}
