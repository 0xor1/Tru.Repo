using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tru.Repo.Test.Repos
{
    class PatchCalledOutsidePatchSequenceRepo : TestRepo
    {
        private const string RepoName = "TruTest.PatchCalledOutsidePatchSequence";
        private static bool HasPatched;

        public PatchCalledOutsidePatchSequenceRepo(string connection)
            : base(connection)
        {
            PatchNestedPatchSequenceRepo();
        }

        private void PatchNestedPatchSequenceRepo()
        {
            if (!HasPatched)
            {
                Patch("This will Fail.");
                HasPatched = true;
            }
        }
    }
}
