using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class CompetenciasEmpleadosController : Controller
    {
        Empleado _empleado = new Empleado();
        Competencia _competencia = new Competencia();
        Competencia_Empleado _competenciaEmpleado = new Competencia_Empleado();

        public IActionResult Index()
        {
            return View();
        }

        // Vista para seleccionar un empleado y ver sus competencias
        public IActionResult VerEmpleados()
        {
            List<Empleado> empleados = _empleado.ObtenerEmpleados();
            ViewBag.ShowActions = "Competencias";
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        // Vista para ver las competencias asignadas a un empleado
        public IActionResult VerCompetenciasEmpleado(int IdEmpleado)
        {
            List<Competencia_Empleado> competencias = _competenciaEmpleado.ObtenerCompetenciasPorEmpleado(IdEmpleado);
            ViewBag.Empleado = _empleado.ObtenerEmpleado(IdEmpleado);
            ViewBag.Competencias = _competencia.ObtenerCompetencias();
            return View("/Views/Admin/VerCompetenciasEmpleado.cshtml", competencias);
        }

        // Agregar una competencia a un empleado
        public IActionResult CrearCompetenciaEmpleado(int IdCompetencia, int IdEmpleado)
        {
            Competencia_Empleado competenciaEmpleado = new Competencia_Empleado
            {
                IdCompetencia = IdCompetencia,
                IdEmpleado = IdEmpleado
            };

            if (competenciaEmpleado.AgregarCompetenciaEmpleado())
            {
                TempData["Success"] = "Competencia asignada correctamente";
            }
            else
            {
                TempData["Error"] = "La competencia ya está asignada al empleado.";
            }

            return RedirectToAction("VerCompetenciasEmpleado", new { IdEmpleado });
        }



        // Eliminar una competencia asignada a un empleado
        public IActionResult EliminarCompetenciaEmpleado(int IdCompetenciaEmpleado)
        {
            _competenciaEmpleado.EliminarCompetenciaEmpleado(IdCompetenciaEmpleado);
            TempData["Success"] = "Competencia eliminada correctamente";
            return RedirectToAction("VerEmpleados");
        }
    }
}
