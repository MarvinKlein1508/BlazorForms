using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Interfaces
{
    public interface IDbModel
    {
        int Id { get; }
        Dictionary<string, object?> GetParameters();
    }
}
