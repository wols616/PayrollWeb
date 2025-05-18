using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Reportes
{
    public class ReporteAsistenciasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult VerEmpleados()
        {
            return View("/Views/Reportes/VerEmpleadosAsistencias.cshtml", new Empleado().ObtenerEmpleados());
        }

        public IActionResult ReportesAsistenciasEmpleado(int IdEmpleado)
        {
            var reporte = new ReporteAsistencias().ObtenerAsistenciasEmpleado(IdEmpleado);
            return View("/Views/Reportes/ReportesAsistencias.cshtml", reporte);
        }
    }
}
