using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Tru.Repo.Exceptions
{
    sealed public class PatchCalledOutsidePatchSequenceError : InvalidOperationException
    {
        public SqlCommand PatchCommand { get; private set; }

        internal PatchCalledOutsidePatchSequenceError(SqlCommand patchCommand)
            : base("It is invalid to make calls to Tru.Repo.Patch outside of the PatchSequence method.")
        {
            PatchCommand = patchCommand;
        }
    }
}
