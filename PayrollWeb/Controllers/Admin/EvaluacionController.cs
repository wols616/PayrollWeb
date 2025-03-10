using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class EvaluacionController : Controller
    {
        EvaluacionDesempeno _evaluacionDesempeno = new EvaluacionDesempeno();
        Empleado _empleado = new Empleado();
        KPI _kpi = new KPI();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerEvaluacionDesempeno(int id)
        {
            List<EvaluacionDesempenoViewModel> listaEvaluacionesDesempeno = _evaluacionDesempeno.ObtenerEvaluacionesDeEmpleado(id);
            ViewBag.Empleado = _empleado.ObtenerEmpleado(id);
            ViewBag.KPIs = _kpi.ObtenerKPI();
            return View("/Views/Admin/VerEvaluacionDesempeno.cshtml", listaEvaluacionesDesempeno);
        }

        [Authorize]
        public IActionResult VerEmpleados()
        {
            List<Empleado> empleados = _empleado.MostrarEmpleados();
            return View("/Views/Admin/VerEmpleadosEvaluacion.cshtml", empleados);
        }

        //Métodos para la lógica
        public IActionResult CrearEvaluacion(int IdEmpleado, DateTime Fecha, int IdKpi, int Puntuacion)
        {
            EvaluacionDesempeno evaluacionDesempeno = (new EvaluacionDesempeno
            {
                id_empleado = IdEmpleado,
                fecha = Fecha,
                id_kpi = IdKpi,
                puntuacion = Puntuacion
            });

            if (!evaluacionDesempeno.AgregarEvaluacionDesempeno(evaluacionDesempeno))
            {
                TempData["Error"] = "Evaluación de desempeño no agregada correctamente";
                return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
            }
            TempData["Success"] = "Evaluación de desempeño agregada correctamente";
            return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
        }

        public IActionResult ActualizarEvaluacion(int IdEmpleado, int IdKpi, DateTime Fecha, int Puntuacion, int IdEvaluacionDesempeno)
        {
            EvaluacionDesempeno evaluacionDesempeno = (new EvaluacionDesempeno
            {
                id_kpi = IdKpi,
                fecha = Fecha,
                puntuacion = Puntuacion,
                IdEvaluacionDesempeno = IdEvaluacionDesempeno
            });

            if (!_evaluacionDesempeno.ActualizarEvaluacionDesempeno(evaluacionDesempeno))
            {
                TempData["Error"] = "Evaluación de desempeño no actualizada correctamente";
                return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
            }
            TempData["Success"] = "Evaluación de desempeño actualizada correctamente";
            return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
        }

    }
}
