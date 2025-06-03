using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Reportes
{
    public class ReporteAntiguedadEmpleadoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReporteAntiguedad(int IdEmpleado)
        {
            var reporte = new ReporteAntiguedad().ObtenerHistorialAscensos(IdEmpleado);
            return View("/Views/Reportes/ReporteAntiguedad.cshtml", reporte);
        }

        public IActionResult VerEmpleados()
        {
            return View("/Views/Reportes/VerEmpleadoAntiguedad.cshtml", new Empleado().ObtenerEmpleados());
        }
    }
}
