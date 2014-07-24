using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tru.Repo.Test.Repos
{
    public class WhiteSpaceRepoNameRepo : TestRepo
    {
        private const string RepoName = "     ";
        private static bool HasPatched;

        public WhiteSpaceRepoNameRepo(string connection)
            : base(connection)
        {
            PatchWhiteSpaceRepoNameRepo();
        }

        private void PatchWhiteSpaceRepoNameRepo()
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
