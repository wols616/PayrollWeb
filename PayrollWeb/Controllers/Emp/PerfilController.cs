using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Emp
{
    public class PerfilController : Controller
    {
        Empleado _empleado = new Empleado();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerPerfil()
        {
            var EmpleadoIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpleado");
            int EmpleadoId = Int32.Parse(EmpleadoIdClaim.Value);

            Empleado empleado = _empleado.ObtenerEmpleado(EmpleadoId);
            return View("/Views/Emp/Perfil.cshtml", empleado);
        }
    }
}
