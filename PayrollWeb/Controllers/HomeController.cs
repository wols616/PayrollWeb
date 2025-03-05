using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    Administrador _administrador = new Administrador();
    Empleado _empleado = new Empleado();
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {

        if (User.IsInRole("Admin"))
        {
            var AdminIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdAdministrador");
            if (AdminIdClaim != null)
            {
                int adminId = Int32.Parse(AdminIdClaim.Value);
                ViewBag.Admin = _administrador.ObtenerAdministrador(adminId);

            }
        }
        else if (User.IsInRole("Empleado"))
        {
            var EmpleadoIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpleado");
            if (EmpleadoIdClaim != null)
            {
                int EmpleadoId = Int32.Parse(EmpleadoIdClaim.Value);
                ViewBag.Empleado = _administrador.ObtenerAdministrador(EmpleadoId);
            }
        }

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
