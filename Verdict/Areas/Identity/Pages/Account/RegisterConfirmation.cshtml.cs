using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Data.SqlClient;
using System;
using System.Data;

namespace Verdict.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = true;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);
                //Verdict.Models.User newUser = new Verdict.Models.User();
                //newUser.email = email;
                //SaveUser(newUser);



            }

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
