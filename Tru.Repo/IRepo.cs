using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tru.Repo
{
    public interface IRepo
    {
        string ConnectionString { get; }
    }
}
