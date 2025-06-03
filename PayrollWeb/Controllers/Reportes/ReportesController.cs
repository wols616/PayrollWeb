using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Reportes
{
    public class ReportesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerEmpleados()
        {

            ViewBag.ShowActions = "HistorialAscensosReportes";
            return View("/Views/Reportes/VerEmpleadosHistorialAscenso.cshtml", new Empleado().ObtenerEmpleados());
        }

        public IActionResult VerReportes()
        {
            return View("/Views/Reportes/VerReportes.cshtml");
        }
        public IActionResult ReportesHistorialAscensos(int IdEmpleado)
        {
            var reporte = new Reporte().ObtenerHistorialAscensos(IdEmpleado);
            return View("/Views/Reportes/ReportesHistorialAscensos.cshtml", reporte);
        }
    }
}
