using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tru.Repo.Exceptions
{
    sealed public class InvalidRepoNameError : InvalidOperationException
    {
        internal InvalidRepoNameError()
            : base("Repo names must be a string containing 1 or more non white space characters.")
        {
        }
    }
}
