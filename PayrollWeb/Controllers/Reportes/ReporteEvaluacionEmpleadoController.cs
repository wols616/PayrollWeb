using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Reportes
{
    public class ReporteEvaluacionEmpleadoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult VerEmpleados()
        {
            return View("/Views/Reportes/VerEmpleadoEvaluaciones.cshtml", new Empleado().ObtenerEmpleados());
        }

        public IActionResult ReporteEvaluacionEmpleado(int IdEmpleado)
        {
            var reporte = new ReporteEvaluacionEmpleado().ObtenerEvaluacionEmpleado(IdEmpleado);
            return View("/Views/Reportes/ReporteEvaluacionEmpleado.cshtml", reporte);
        }


    }
}
