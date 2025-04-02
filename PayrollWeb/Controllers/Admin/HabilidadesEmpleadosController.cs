using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class HabilidadesEmpleadosController : Controller
    {
        Empleado _empleado = new Empleado();
        Habilidad _habilidad = new Habilidad();
        Habilidad_Empleado _habilidadEmpleado = new Habilidad_Empleado();

        public IActionResult Index()
        {
            return View();
        }

        // Vista para seleccionar un empleado y ver sus habilidades
        public IActionResult VerEmpleados()
        {
            List<Empleado> empleados = _empleado.ObtenerEmpleados();
            ViewBag.ShowActions = "Habilidades";
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        // Vista para ver las habilidades asignadas a un empleado
        public IActionResult VerHabilidadesEmpleado(int IdEmpleado)
        {
            List<Habilidad_Empleado> habilidades = _habilidadEmpleado.ObtenerHabilidadesPorEmpleado(IdEmpleado);
            ViewBag.Empleado = _empleado.ObtenerEmpleado(IdEmpleado);
            ViewBag.Habilidades = _habilidad.ObtenerHabilidades();
            return View("/Views/Admin/VerHabilidadesEmpleado.cshtml", habilidades);
        }

        // Agregar una habilidad a un empleado
        public IActionResult CrearHabilidadEmpleado(int IdHabilidad, int IdEmpleado)
        {
            Habilidad_Empleado habilidadEmpleado = new Habilidad_Empleado
            {
                IdHabilidad = IdHabilidad,
                IdEmpleado = IdEmpleado
            };

            if (habilidadEmpleado.AgregarHabilidadEmpleado())
            {
                TempData["Success"] = "Habilidad asignada correctamente";
            }
            else
            {
                TempData["Error"] = "La habilidad ya está asignada al empleado.";
            }

            return RedirectToAction("VerHabilidadesEmpleado", new { IdEmpleado });
        }


        // Eliminar una habilidad asignada a un empleado
        public IActionResult EliminarHabilidadEmpleado(int IdHabilidadEmpleado)
        {
            _habilidadEmpleado.EliminarHabilidadEmpleado(IdHabilidadEmpleado);
            TempData["Success"] = "Habilidad eliminada correctamente";
            return RedirectToAction("VerEmpleados");
        }
    }
}
