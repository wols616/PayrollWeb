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
        Meta _meta = new Meta();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerEvaluacionDesempeno(int id)
        {
            List<EvaluacionDesempenoViewModel> listaEvaluacionesDesempeno = _evaluacionDesempeno.ObtenerEvaluacionesDeEmpleado(id);
            ViewBag.Empleado = _empleado.ObtenerEmpleado(id);
            ViewBag.KPIs = _kpi.ObtenerKPI();
            ViewBag.Metas = _meta.ObtenerMetasDeEmpleado(id);
            return View("/Views/Admin/VerEvaluacionDesempeno.cshtml", listaEvaluacionesDesempeno);
        }

        [Authorize]
        public IActionResult VerEmpleados()
        {
            List<Empleado> empleados = _empleado.ObtenerEmpleados();
            ViewBag.ShowActions = "Evaluaciones";
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
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

        public IActionResult CrearMeta(string MetaDescripcion, string Estado, int IdEmpleado)
        {
            Meta meta = new Meta
            {
                IdEmpleado = IdEmpleado,
                MetaDescripcion = MetaDescripcion,
                Estado = Estado
            };
            if (meta.AgregarMeta())
            {
                TempData["Success"] = "Meta agregada correctamente";
            }
            else
            {
                TempData["Error"] = "Error al agregar la meta";
            }
            return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
        }

        public IActionResult ActualizarMeta(string MetaDescripcion, string Estado, int IdEmpleado, int IdMeta)
        {
            Meta meta = new Meta
            {
                IdMeta = IdMeta,
                MetaDescripcion = MetaDescripcion,
                Estado = Estado
            };

            if (meta.ActualizarMeta())
            {
                TempData["Success"] = "Meta actualizada correctamente";
            }
            else
            {
                TempData["Error"] = "Error al actualizar la meta";
            }
            return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
        }

    }
}
