using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tru.Repo.Exceptions
{
    sealed public class NestedPatchSequenceError: InvalidOperationException
    {
        public string ExecutingSequence { get; private set; }
        public string NestedSequence { get; private set; }

        internal NestedPatchSequenceError(string executingSequence, string nestedSequence)
            : base("It is invalid to make nested calls to Tru.Repo.PatchSequence.")
        {
            ExecutingSequence = executingSequence;
            NestedSequence = nestedSequence;
        }
    }
}
