using gunluk_mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;



namespace gunluk_mvc.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-51T12NN\\SQLEXPRESS;Initial Catalog=gunluk;Integrated Security=True");

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }


        public IActionResult GecmisGunler()
        {
            List<Icerik> Iceriks = new List<Icerik>();

            //Veritabanına bağlan

            connection.Open();

            //Tüm kullanıcıları getir
            string query = "SELECT * FROM dbo.Iceriks";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            //Kullanıcıları oku ve listeye ekle
            while (reader.Read())
            {
                Icerik icerik = new Icerik();
                icerik.Id = reader.GetInt32(0);
                icerik.Baslik = reader.GetString(1);
                icerik.Gunlugum = reader.GetString(2);
                Iceriks.Add(icerik);
            }

            //Veritabanı bağlantısını kapat
            connection.Close();
            return View(Iceriks);
        }

        public IActionResult KayitOl()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KayitOl(string inputEmail, string inputPassword)
        {
            connection.Open();

            //Tüm kullanıcıları getir
            string query = "SELECT * FROM dbo.Users WHERE KullaniciAdi='" + inputEmail + "'";
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
            if (user.KullaniciAdi != inputEmail)
            {
                string insert_query = "INSERT INTO dbo.Users (KullaniciAdi, Sifre) VALUES (@KullaniciAdi, @Sifre)";

                try
                {
                    SqlCommand insert_command = new SqlCommand(insert_query, connection);

                    // Parametreler ekle
                    insert_command.Parameters.AddWithValue("@KullaniciAdi", inputEmail);
                    insert_command.Parameters.AddWithValue("@Sifre", inputPassword);
                    connection.Open();
                    // Sorguyu çalıştır
                    int rowsAffected = insert_command.ExecuteNonQuery();

                    //Veritabanı bağlantısını kapat
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return View();
        }


        public IActionResult BugunuYaz()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BugunuYaz(int UserId, string inputbaslik, string inputicerik)
        {

            string insert_query = "INSERT INTO dbo.Iceriks (UserId,Baslik,Gunlugum) VALUES (@UserId,@Baslik,@Gunlugum)";

            try
            {
                SqlCommand insert_command = new SqlCommand(insert_query, connection);

                // Parametreler ekle
                insert_command.Parameters.AddWithValue("@UserId", 1);
                insert_command.Parameters.AddWithValue("@Baslik", inputbaslik);
                insert_command.Parameters.AddWithValue("@Gunlugum", inputicerik);
                connection.Open();
                // Sorguyu çalıştır
                int rowsAffected = insert_command.ExecuteNonQuery();

                //Veritabanı bağlantısını kapat
                connection.Close();
                Console.WriteLine("okey");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}