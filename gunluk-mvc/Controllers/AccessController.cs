using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using gunluk_mvc.Models;
using System.Data.SqlClient;

namespace gunluk_mvc.Controllers
{
    public class AccessController : Controller
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-51T12NN\\SQLEXPRESS;Initial Catalog=gunluk;Integrated Security=True");

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {

            connection.Open();

            //Tüm kullanıcıları getir
            string query = "Select * from dbo.Users where KullaniciAdi='" + modelLogin.Email + "' and Sifre='" + modelLogin.Password + "' ;";
            //string query = "SELECT * FROM dbo.Users WHERE KullaniciAdi='" + inputEmail + "'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            User user = new User();
            while (reader.Read())
            {

                user.Id = reader.GetInt32(0);
                user.KullaniciAdi = reader.GetString(1);
                user.Sifre = reader.GetString(2);
            }

            connection.Close();

            if (modelLogin.Email == user.KullaniciAdi &&
                modelLogin.Password == user.Sifre.Trim())
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,modelLogin.Email),
                    new Claim("OtherProperties","Example Role")
                };
                ClaimsIdentity claimsidentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = modelLogin.KeepLoggedIn
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsidentity), properties);
                return RedirectToAction("Index", "Home");
            }
            ViewData["ValidateMessage"] = "user not found";
            return View();
        }
    }
}
