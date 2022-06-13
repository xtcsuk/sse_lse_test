using System.Collections.Generic;

namespace Asos.CodeTest
{
    public interface IFailoverRepository
    {
        List<FailoverEntry> GetFailOverEntries();
    }
}