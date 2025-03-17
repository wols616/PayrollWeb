using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class DeduccionesPersonalesController : Controller
    {
        Empleado _empleado = new Empleado();
        Deduccion _deduccion = new Deduccion();
        Deduccion_Personal _deduccion_Personal = new Deduccion_Personal();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerEmpleados()
        {
            return View("/Views/Admin/VerEmpleadosDeducciones.cshtml", _empleado.ObtenerEmpleados());
        }

        public IActionResult VerDeducciones(int IdEmpleado)
        {
            List<Deduccion_Personal> deducciones = _deduccion_Personal.ObtenerDeduccionesPersonalesEmpleado(IdEmpleado);
            ViewBag.Empleado = _empleado.ObtenerEmpleado(IdEmpleado);
            ViewBag.Deducciones = _deduccion.ObtenerDeducciones();
            return View("/Views/Admin/VerDeduccionesPersonales.cshtml", deducciones);
        }

        //Métodos para la lógica
        public IActionResult CrearDeduccionPersonal(int IdDeduccion, int IdEmpleado)
        {
            Deduccion_Personal deduccionp = new Deduccion_Personal { IdDeduccion = IdDeduccion,IdEmpleado = IdEmpleado};
            deduccionp.AgregarDeduccionPersonal();
            TempData["Success"] = "Deducción personal creada correctamente";
            return RedirectToAction("VerDeducciones", new { IdEmpleado = IdEmpleado });
        }

        public IActionResult EliminarDeduccionPersonal(int IdDeduccion, int IdEmpleado)
        {
            _deduccion_Personal.EliminarDeduccionPersonal(IdDeduccion, IdEmpleado);
            TempData["Success"] = "Deducción personal eliminada correctamente";
            return RedirectToAction("VerDeducciones", new { IdEmpleado = IdEmpleado });
        }   

    }
}
