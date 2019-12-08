using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Verdict.DataAccess.Data.Repository.IRepository;
using Verdict.Models;

namespace Verdict.DataAccess.Data.Repository
{
    public class SearchRepository : Repository<Search>, ISearchRepository
    {
        private readonly ApplicationDbContext _db;

        public SearchRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<SelectListItem> GetSearchListForDropDown()
        {
            return _db.Search.Select(i => new SelectListItem()
            {
                Text = i.Term,
                Value = i.ID.ToString()
            });
        }

        public void Update(Search search)
        {
            var objFromDb = _db.Search.FirstOrDefault(s => s.ID == search.ID);

            objFromDb.TotalScore = search.TotalScore;

            _db.SaveChanges();
        }
    }
}
