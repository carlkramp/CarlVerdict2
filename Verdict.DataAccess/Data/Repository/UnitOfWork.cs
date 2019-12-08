using System;
using System.Collections.Generic;
using System.Text;
using Verdict.DataAccess.Data.Repository.IRepository;

namespace Verdict.DataAccess.Data.Repository.IRepository
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Search = new SearchRepository(_db);
        }

        public ISearchRepository Search { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
