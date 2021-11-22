using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Verdict.Models;
using Verdict;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using Tweetinvi.Logic.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace Verdict.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SignInManager<IdentityUser> SignInManager;
        UserManager<IdentityUser> UserManager;


        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        //public HomeController(ISearchService searchService)
        //{
        //    _searchService = searchService;
        //}

        public IActionResult Index()
        {
            return View();

        }

        public IActionResult About()
        {
            return View();
        }

        // Search counter view
        public IActionResult Report()
        {
            Reports Report = new Reports();

            {
                // Declare connection with connection string
                using (SqlConnection connection = new SqlConnection("Server=tcp:verdictapp-dbserver.database.windows.net,1433;Database=verdictapp_db;User ID=verdictadmin;Password = Verdict124;Encrypt = true;Connection Timeout = 30;"))
                {

                    // Declare query
                    String query = "SELECT MAX(ID) AS NumOfSearches FROM Search;";
                    String query2 = "SELECT Term, TotalScore, SearchDate FROM Search WHERE TotalScore = (SELECT MAX(TotalScore) FROM Search)";
                    String query3 = "SELECT Term, TotalScore, SearchDate FROM Search WHERE TotalScore = (SELECT MIN(TotalScore) FROM Search)";

                    Reports[] MyCounter = null;
                    var list = new List<Reports>();

                    // Create new command with query and connection, use addwithvalue method to allow variables to be added to the database
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {                                                      
                            while (reader.Read())
                                list.Add(new Reports { Counter = reader.GetInt32(0) });
                            //MyCounter = list.ToArray();
                            //Report.MyCounter = list;
                        }
                    }

                    using (SqlCommand command = new SqlCommand(query2, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                list.Add(new Reports { MaxTerm = reader.GetString(0), MaxScore = reader.GetInt32(1), MaxDate = reader.GetDateTime(2) });
                            //MyCounter = list.ToArray();
                            //Report.MyCounter = list;
                        }
                    }

                    using (SqlCommand command = new SqlCommand(query3, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                list.Add(new Reports { MinTerm = reader.GetString(0), MinScore = reader.GetInt32(1), MinDate = reader.GetDateTime(2) });
                            MyCounter = list.ToArray();
                            Report.MyCounter = list;
                        }
                    }
                }
            }
            return View(Report);
        }

        public IActionResult Collection()
        {

            Reports Report = new Reports();
            {
                // Declare connection with connection string
                //using (SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLOCALDB;Database=VerdictDB;Trusted_Connection=True;"))
                using (SqlConnection connection = new SqlConnection("Server=tcp:verdictapp-dbserver.database.windows.net,1433;Database=verdictapp_db;User ID=verdictadmin;Password=Verdict124;Encrypt=true;Connection Timeout=30;"))
                {
                    string name = User.Identity.Name;
                    // Declare query
                    String query = "SELECT Term, AVG(TotalScore) AS AverageScore, Count(*) AS NumberOfSearches  FROM Search WHERE UserSearchID = @name GROUP BY Term ORDER BY Count(*) DESC;";
                 

                    SearchReport[] MyTopSearches = null;

                    // Create new command with query and connection, use addwithvalue method to allow variables to be added to the database
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", SqlDbType.NVarChar);
                        command.Parameters["@name"].Value = name;


                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var list = new List<SearchReport>();
                            while (reader.Read())
                                list.Add(new SearchReport { Term = reader.GetString(0), avgScore = reader.GetInt32(1), Counter = reader.GetInt32(2) });
                            MyTopSearches = list.ToArray();
                            Report.TopSearches = list;
                        }
                    }
                }
            }




            return View(Report);
        }

        [HttpPost]
        public IActionResult Collection(Models.Search searchTerm)
        {
            List<Models.Tweet> listOfTweets = searchTerm.ListOfTweets;
            int totalScore = searchTerm.TotalScore;



            // Save search to user's collection 

            return View();
        }

        public IActionResult TopFive()
        {
            Reports Report = new Reports();
            {
                // Declare connection with connection string
                //using (SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLOCALDB;Database=VerdictDB;Trusted_Connection=True;"))
                using (SqlConnection connection = new SqlConnection("Server=tcp:verdictapp-dbserver.database.windows.net,1433;Database=verdictapp_db;User ID=verdictadmin;Password=Verdict124;Encrypt=true;Connection Timeout=30;"))
                {

                    // Declare query
                    String query = "SELECT TOP 5 Term, AVG(TotalScore) AS AvgScore, Count(*) AS NumberOfSearches FROM Search GROUP BY Term ORDER BY Count(*) DESC;";

                    SearchReport[] MyTopSearches = null;

                    // Create new command with query and connection, use addwithvalue method to allow variables to be added to the database
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                       



                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var list = new List<SearchReport>();
                            while (reader.Read())
                                list.Add(new SearchReport { Term = reader.GetString(0), avgScore = reader.GetInt32(1), Counter = reader.GetInt32(2) });
                            MyTopSearches = list.ToArray();
                            Report.TopSearches = list;
                        }
                    }
                }
            }
            return View(Report);
        }
        public IActionResult Search(Models.Search searchTerm)
        {
            List<Models.Tweet> TweetList = new List<Models.Tweet>();

            // twitter api

            var CONSUMER_KEY = "";
            var CONSUMER_SECRET = "";
            var ACCESS_TOKEN = "";
            var ACCESS_TOKEN_SECRET = "";

            Auth.SetUserCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);

            // azure apis

            // var key = "b0d42d9325734b5db12af46844f3ed7b";
            var key = "e21c1c66d9db4b4695ec979842da4d8b";
            //var key = Environment.GetEnvironmentVariable(key_var);
            //  var endpoint = "https://verdict.cognitiveservices.azure.com/";
            var endpoint = "https://tweetanalytics.cognitiveservices.azure.com/"; 
            //var endpoint = Environment.GetEnvironmentVariable(endpoint_var);

            TextAnalyticsClient authenticateClient(string apikey, string apiendpoint)
            {
                ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(apikey);
                TextAnalyticsClient apiclient = new TextAnalyticsClient(credentials)
                {
                    Endpoint = endpoint
                };
                return apiclient;
            }

            var client = authenticateClient(key, endpoint);

            // Perform sentiment analysis of each tweet

            double? sentimentAnalysis(ITextAnalyticsClient apiclient, string text)
            {
                
                var result = apiclient.Sentiment(text);
                return result.Score;
            }

            // Set the search to be sent to Twitter

            var searchParameter = new SearchTweetsParameters(searchTerm.Term)
            {
                Lang = LanguageFilter.English,
                SearchType = SearchResultType.Mixed,
                Filters = TweetSearchFilters.Safe
            };

            // Get list of tweets from API

            var tweets = Tweetinvi.Search.SearchTweets(searchParameter);

            // Loop through each tweet, run sentiment analysis, update TweetList

            foreach (var tweet in tweets)
            {
                if (tweet.InReplyToStatusId == null && tweet.IsRetweet == false)
                {
                    Models.Tweet newTweet = new Models.Tweet();

                    newTweet.TwitterUser = tweet.CreatedBy.ScreenName;
                    newTweet.Text = tweet.FullText;
                    newTweet.HoursAgo = Convert.ToInt32(Math.Abs(Math.Round((DateTime.Now - tweet.CreatedAt).TotalHours)));
                    newTweet.TweetLink = tweet.Url;
                    var score = sentimentAnalysis(client, tweet.FullText);

                    // Convert score to a double, trouble with database accepting vars 
                    double newScore = Convert.ToDouble(score);
                    newScore = newScore * 100;


                    // eliminate scores of 0.5 (lack of sentiment)
                    if (score.HasValue)
                    {
                        if (Math.Round(score.Value * 100) != 50)
                        {
                            newTweet.Score = score;
                            TweetList.Add(newTweet);
                        }
                    }

                };

            }

            // Find the search term's overall score

            int findScore(Models.Search apiSearchTerm)
            {
                var total_score = 0.0;
                var average_score = 0;
                var count = 0;
                foreach (var tweet in apiSearchTerm.ListOfTweets)
                {
                    if (tweet.Score.HasValue)
                    {
                        if (Math.Round(tweet.Score.Value) != 0.5)
                        {
                            total_score = total_score + tweet.Score.Value;
                            count++;
                        }
                    }
                }
                if (count > 0)
                {
                    average_score = Convert.ToInt32(Math.Round((total_score / count) * 100));
                }
                return average_score;
            }

            searchTerm.ListOfTweets = TweetList;
            searchTerm.TotalScore = findScore(searchTerm);
            searchTerm.SearchDate = DateTime.Now;

            // Function to save search to database
            SaveSearch(searchTerm);

            return View(searchTerm);
        }


        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            private readonly string apiKey;

            public ApiKeyServiceClientCredentials(string apiKey)
            {
                this.apiKey = apiKey;
            }

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.apiKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public void SaveTweets(Models.Tweet Tweet, double score, Models.Search searchTerm)
        {

            // Declare connection with connection string
            using (SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLOCALDB;Database=VerdictDB;Trusted_Connection=True;"))
            {

                // Declare query
                String query = "INSERT Tweet (Text, TwitterUser, HoursAgo, TweetLink, Score, Term) VALUES (@Text, @TwitterUser, @HoursAgo, @TweetLink, @Score, @Term)";


                // Create new command with query and connection, use addwithvalue method to allow variables to be added to the database
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@Text", SqlDbType.NVarChar);
                    command.Parameters["@Text"].Value = Tweet.Text;
                    command.Parameters.AddWithValue("@TwitterUser", SqlDbType.NVarChar);
                    command.Parameters["@TwitterUser"].Value = Tweet.TwitterUser;
                    command.Parameters.AddWithValue("@HoursAgo", SqlDbType.Int);
                    command.Parameters["@HoursAgo"].Value = Tweet.HoursAgo;
                    command.Parameters.AddWithValue("@TweetLink", SqlDbType.NVarChar);
                    command.Parameters["@TweetLink"].Value = Tweet.TweetLink;
                    command.Parameters.AddWithValue("@Score", SqlDbType.Float);
                    command.Parameters["@Score"].Value = score;
                    command.Parameters.AddWithValue("@Term", SqlDbType.NVarChar);
                    command.Parameters["@Term"].Value = searchTerm.Term;
                    //command.Parameters.AddWithValue("@SearchId", SqlDbType.Int);
                    //command.Parameters["@SearchID"].Value = searchTerm.ID;


                    connection.Open();
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                        Console.WriteLine("Error inserting data into Database!");
                }
            }






        }


        public void SaveSearch(Models.Search Search)
        {
            //using (SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLOCALDB;Database=VerdictDB;Trusted_Connection=True;"))
            using (SqlConnection connection = new SqlConnection("Server=tcp:verdictapp-dbserver.database.windows.net,1433;Database=verdictapp_db;User ID=verdictadmin;Password = Verdict124;Encrypt = true;Connection Timeout = 30;"))
            {
                String query = "INSERT Search (TotalScore, Term, SearchDate, UserSearchID) VALUES (@TotalScore, @Term, @SearchDate, @UserSearchID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {


                    command.Parameters.AddWithValue("@TotalScore", SqlDbType.Int);
                    command.Parameters["@TotalScore"].Value = Search.TotalScore;
                    command.Parameters.AddWithValue("@Term", SqlDbType.NVarChar);
                    command.Parameters["@Term"].Value = Search.Term;
                    command.Parameters.AddWithValue("@SearchDate", SqlDbType.NVarChar);
                    command.Parameters["@SearchDate"].Value = Search.SearchDate;
                    //  command.Parameters.AddWithValue("@UserSearchID", SqlDbType.NVarChar);
                    //  command.Parameters["@UserSearchID"].Value = "Anonymus";

                 


                        if (User.Identity.Name != null)
                        {


                            command.Parameters.AddWithValue("@UserSearchID", SqlDbType.NVarChar);
                            command.Parameters["@UserSearchID"].Value = User.Identity.Name;
                        }
                    




                    else
                    {
                        command.Parameters.AddWithValue("@UserSearchID", SqlDbType.NVarChar);
                        command.Parameters["@UserSearchID"].Value = "Anonymus";
                    }




                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                        Console.WriteLine("Error inserting data into Database!");
                }
            }
        }

     
        

    }
}

