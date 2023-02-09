using Cricket_Club.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Diagnostics;

namespace Cricket_Club.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly DataContext datacontext;

        public HomeController(DataContext context)
        {
            datacontext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Home(Credential cre)
        {
            var data = datacontext.credential.FirstOrDefault(x => x.UserName == cre.UserName);
            if (data != null && data.Password == cre.Password)
            {
                return View();
            }

            return View("Login");

        }
        public IActionResult Home()
        {

            return View();
        }

        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Credential cre)
        {
            var data = new Credential()
            {
                Id = Guid.NewGuid(),
                Name = cre.Name,
                UserName = cre.UserName,
                Email = cre.Email,
                Password = cre.Password,
            };
            await datacontext.credential.AddAsync(data);
            await datacontext.SaveChangesAsync();
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}