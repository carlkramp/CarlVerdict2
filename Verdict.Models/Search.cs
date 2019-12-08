using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Verdict.Models
{
    public class Search
    {
 
        //public int counter = 0;
        public List<Tweet> ListOfTweets { get; set; }
        public int TotalScore { get; set; }
        public DateTime SearchDate { get; set; }

        public String UserID { get; set; }

        [Key]
        public int ID { get; set; }

        [Required]
        public string Term { get; set; }


    }

    public class Tweet
    {
        // Give each tweet an ID adasdshdzjbd,kaj 5
        public int counter = 0;
        [Key]
        public int TweetID
        {
            get
            {
                return TweetID;
            }
            set
            {
                TweetID = counter;
                counter++;
            }
        }
        [Required]
        public string Term { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string TwitterUser { get; set; }
        [Required]
        public int HoursAgo { get; set; }
        [Required]
        public string TweetLink { get; set; }
        [Required]
        public double? Score { get; set; }





    }
}
