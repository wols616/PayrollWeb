using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Reportes
{
    public class ReporteCompetenciasHabilidadesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ReportesCompetenciasHabilidades(int IdEmpleado)
        {
            var reporte = new ReporteCompetenciasHabilidades().ObtenerCompetenciasYHabilidadesEmpleado(IdEmpleado);
            return View("/Views/Reportes/ReporteCompetenciasHabilidades.cshtml", reporte);
        }

        public IActionResult VerEmpleadosCompetenciasHabilidades()
        {
            return View("/Views/Reportes/VerEmpleadosCompetenciasHabilidades.cshtml", new Empleado().ObtenerEmpleados());
        }


    }
}
