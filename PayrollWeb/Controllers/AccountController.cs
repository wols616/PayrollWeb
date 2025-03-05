using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;


namespace PayrollWeb.Controllers
{
    public class AccountController : Controller
    {
        Administrador _administrador = new Administrador();
        Empleado _empleado = new Empleado();
        public IActionResult Index()
        {
            return View();
        }
        //CONTROLADORES PARA ABRIR LAS VISTAS

        //GET
        public IActionResult Login()
        {
            return View("/Views/Home/Login.cshtml");
        }

        //__________________________________________________________________________________________________________________________________
        //POST
        [HttpPost]
        public async Task<IActionResult> Login(string tipo, string correo, string contrasena)
        {
            
            if (tipo == "Admin")
            {
                int idAdministrador = -1;
                idAdministrador = _administrador.LoginAdministrador(correo, contrasena);

                if (idAdministrador == -1)
                {
                    ViewBag.Error = "Correo o contraseña incorrectos";
                    return View("/Views/Home/Login.cshtml");
                }
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, correo),
                    new Claim("IdAdministrador", idAdministrador.ToString()),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                int idEmpleado = -1;
                idEmpleado = _empleado.LoginEmpleado(correo, contrasena);

                if (idEmpleado == -1)
                {
                    ViewBag.Error = "Correo o contraseña incorrectos";
                    return View("/Views/Home/Login.cshtml");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, correo),
                    new Claim("IdEmpleado", idEmpleado.ToString()),
                    new Claim(ClaimTypes.Role, "Empleado")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
