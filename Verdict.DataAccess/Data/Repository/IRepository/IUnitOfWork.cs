using System;
using System.Collections.Generic;
using System.Text;

namespace Verdict.DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ISearchRepository Search { get; }
        void Save();
    }
}
