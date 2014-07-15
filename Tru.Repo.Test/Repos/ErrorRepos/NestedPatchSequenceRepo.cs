using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tru.Repo.Test.Repos
{
    public class NestedPatchSequenceRepo : TestRepo
    {
        private const string RepoName = "TruTest.NestedPatchSequence";
        private static bool HasPatched;

        public NestedPatchSequenceRepo(string connection)
            : base(connection)
        {
            PatchNestedPatchSequenceRepo();
        }

        private void PatchNestedPatchSequenceRepo()
        {
            if (!HasPatched)
            {
                PatchSequence(RepoName, () =>
                {
                    PatchSequence(RepoName, () => 
                    {
                        Patch("This won't be reached.");
                    });
                });
                HasPatched = true;
            }
        }
    }
}
