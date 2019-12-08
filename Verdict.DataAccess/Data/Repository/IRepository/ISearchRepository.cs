using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Verdict.Models;

namespace Verdict.DataAccess.Data.Repository.IRepository
{
    public interface ISearchRepository : IRepository<Search>
    {
        IEnumerable<SelectListItem> GetSearchListForDropDown();

        void Update(Search search);
    }
}
