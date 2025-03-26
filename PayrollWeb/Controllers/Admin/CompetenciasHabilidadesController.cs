using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class CompetenciasHabilidadesController : Controller
    {
        Empleado _empleado = new Empleado();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerCompetenciasHabilidadesSelect()
        {
            return View("/Views/Admin/CompetenciashabilidadesSelect.cshtml");
        }

        public IActionResult VerCompetencias()
        {
            return View("/Views/Admin/VerCompetencias.cshtml");
        }

        public IActionResult VerHabilidades()
        {
            return View("/Views/Admin/VerHabilidades.cshtml");
        }

        public IActionResult VerEmpleados(string ShowActions)
        {
            ViewBag.ShowActions = ShowActions;
            return View("/Views/Admin/VerEmpleados.cshtml", _empleado.ObtenerEmpleados());
        }

        public IActionResult VerHabilidadesEmpleado(int IdEmpleado)
        {
            Empleado empleado = _empleado.ObtenerEmpleado(IdEmpleado);
            ViewBag.Empleado = empleado;
            return View("/Views/Admin/VerHabilidadesEmpleado.cshtml");
        }
        public IActionResult VerCompetenciasEmpleado(int IdEmpleado)
        {
            Empleado empleado = _empleado.ObtenerEmpleado(IdEmpleado);
            ViewBag.Empleado = empleado;
            return View("/Views/Admin/VerCompetenciasEmpleado.cshtml");
        }

    }
}
