using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;


namespace Verdict.Models
{
    public class Reports
    {
        public Reports()
        {

        }
        public List<SearchReport> TopSearches { get; set; }

        public List<Reports> MyCounter { get; set; }

        public DateTime MaxDate { get; set; }

        public int MaxScore { get; set; }

        public string MaxTerm { get; set; }

        public DateTime MinDate { get; set; }

        public int MinScore { get; set; }

        public string MinTerm { get; set; }

        public int Counter { get; set; }

  
       

        
           





    }
}
