using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PromoLimit.Services;
using PromoLimit.States;
using PromoLimit.Views.Auth;

namespace PromoLimit.Controllers
{
    public class AuthController : Controller
    {
        private readonly SessionService _sessionManager;

        public AuthController(IServiceProvider provider)
        {
            _sessionManager = provider.GetRequiredService<SessionService>();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel livm)
        {
            try
            { 
                var infoLogin = await _sessionManager.LogIn(livm.Username, livm.Password);
                if (!string.IsNullOrWhiteSpace(infoLogin.ErrorMessage))
                {
                    ModelState.AddModelError("ValidLogin", infoLogin.ErrorMessage);
                    return View();
                }
                HttpContext.Session.SetString("uuid", infoLogin.KeyInfo.uuid);
                //HttpContext.Session.SetInt32("setorId", infoLogin.KeyInfo.setorId);
                //HttpContext.Session.SetInt32("gerencia", Convert.ToInt32(infoLogin.KeyInfo.gerencia));
                HttpContext.Session.SetString("displayName", infoLogin.KeyInfo.displayName);
                HttpContext.Response.Cookies.Append("apiKey", infoLogin.Apikey);
                return RedirectToAction("Index", "Home", new {area = ""});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Response.Cookies.Delete("apiKey");
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home", new {area = ""});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if (rvm.Username.Trim().Contains(' '))
            {
                ModelState.AddModelError("ValidLogin", "Login não pode conter espaços");
                return View();
            }
            var infoRegister = await _sessionManager.RegisterNewUser(rvm.Nome.Trim(), rvm.Username.Trim().ToLower(), rvm.Senha, rvm.Confirmação);

            try
            {
                if (!string.IsNullOrWhiteSpace(infoRegister.ErrorMessage))
                {
                    ModelState.AddModelError("ValidRegister", infoRegister.ErrorMessage);
                    return View();
                }

                TempData["RegisterSuccessful"] = true;
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
