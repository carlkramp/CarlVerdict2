using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Verdict.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";


            //Verdict.Models.User newUser = new Verdict.Models.User();
            //newUser.email = userId;
            //SaveUser(newUser);

            return Page();
        }


        //public void SaveUser(Models.User User)
        //{
        //    using (SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLOCALDB;Database=VerdictDB;Trusted_Connection=True;"))
        //    //  using (SqlConnection connection = new SqlConnection("Server=tcp:verdictapp-dbserver.database.windows.net,1433;Database=verdictapp_db;User ID=verdictadmin;Password = Verdict124;Encrypt = true;Connection Timeout = 30;"))
        //    {
        //        String query = "INSERT User (Email) VALUES (@Email)";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {



        //            command.Parameters.AddWithValue("@Email", SqlDbType.NVarChar);
        //            command.Parameters["@Email"].Value = User.email;


        //            connection.Open();
        //            int result = command.ExecuteNonQuery();

        //            // Check Error
        //            if (result < 0)
        //                Console.WriteLine("Error inserting data into Database!");
        //        }
        //    }
        //}
    }
}
