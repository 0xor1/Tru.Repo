using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tru.Repo.Test.Repos
{
    public class NullRepoNameRepo : TestRepo
    {
        private const string RepoName = null;
        private static bool HasPatched;

        public NullRepoNameRepo(string connection)
            : base(connection)
        {
            PatchNullRepoNameRepo();
        }

        private void PatchNullRepoNameRepo()
        {
            if (!HasPatched)
            {
                PatchSequence(RepoName, () =>
                {
                    Patch("This won't be reached.");
                });
                HasPatched = true;
            }
        }
    }
}
