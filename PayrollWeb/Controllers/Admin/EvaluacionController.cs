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

        public IActionResult VerEvaluacionDesempeno(int id, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            List<EvaluacionDesempenoViewModel> listaEvaluacionesDesempeno = _evaluacionDesempeno.ObtenerEvaluacionesDeEmpleado(id);
            bool tieneFiltroFecha = fechaInicio.HasValue || fechaFin.HasValue;

            // Aplicar filtro por fecha si se especificó
            if (tieneFiltroFecha)
            {
                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    listaEvaluacionesDesempeno = listaEvaluacionesDesempeno
                        .Where(e => e.fecha == fechaInicio.Value)
                        .ToList();
                }
                else if (fechaInicio.HasValue)
                {
                    listaEvaluacionesDesempeno = listaEvaluacionesDesempeno
                        .Where(e => e.fecha >= fechaInicio.Value)
                        .ToList();
                }
            }

            // Preparar datos para la gráfica
            var kpis = _kpi.ObtenerKPI();
            var datosGrafica = new List<double>();

            if (tieneFiltroFecha)
            {
                // Usar datos filtrados
                foreach (var kpi in kpis)
                {
                    var evaluacion = listaEvaluacionesDesempeno.FirstOrDefault(e => e.id_kpi == kpi.IdKpi);
                    datosGrafica.Add(evaluacion != null ? evaluacion.puntuacion : 0);
                }
            }
            else
            {
                // Calcular promedios
                var evaluacionesAgrupadas = _evaluacionDesempeno.ObtenerEvaluacionesDeEmpleado(id)
                    .GroupBy(e => e.id_kpi)
                    .ToDictionary(g => g.Key, g => g.Average(e => e.puntuacion));

                foreach (var kpi in kpis)
                {
                    datosGrafica.Add(evaluacionesAgrupadas.TryGetValue(kpi.IdKpi, out var promedio) ? promedio : 0);
                }
            }

            ViewBag.Evaluaciones = _evaluacionDesempeno.ObtenerEvaluacionesDeEmpleado(id);
            ViewBag.Empleado = _empleado.ObtenerEmpleado(id);
            ViewBag.KPIs = kpis;
            ViewBag.Metas = _meta.ObtenerMetasDeEmpleado(id);
            ViewBag.DatosGrafica = datosGrafica;
            ViewBag.TieneFiltroFecha = tieneFiltroFecha;

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

        [HttpPost]
        public IActionResult CrearEvaluaciones(int IdEmpleado, DateTime Fecha, Dictionary<int, int> Puntuaciones)
        {
            try
            {
                // Validar si ya existe una evaluación para esta fecha
                var evaluacionesExistentes = _evaluacionDesempeno.ObtenerEvaluacionesDeEmpleado(IdEmpleado);
                if (evaluacionesExistentes.Any(e => e.fecha.Date == Fecha.Date))
                {
                    TempData["Error"] = "Ya existe una evaluación para la fecha seleccionada";
                    return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
                }

                // Procesar cada evaluación
                foreach (var puntuacion in Puntuaciones)
                {
                    var evaluacion = new EvaluacionDesempeno
                    {
                        id_empleado = IdEmpleado,
                        fecha = Fecha,
                        id_kpi = puntuacion.Key,
                        puntuacion = puntuacion.Value
                    };

                    if (!evaluacion.AgregarEvaluacionDesempeno(evaluacion))
                    {
                        TempData["Error"] = "Error al guardar algunas evaluaciones";
                        return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
                    }
                }

                TempData["Success"] = "Evaluaciones guardadas correctamente";
                return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar las evaluaciones: " + ex.Message;
                return RedirectToAction("VerEvaluacionDesempeno", new { id = IdEmpleado });
            }
        }


        [HttpPost]
        public IActionResult ActualizarEvaluacion(int IdEvaluacionDesempeno, int IdEmpleado, int IdKpi, DateTime Fecha, int Puntuacion)
        {
            EvaluacionDesempeno evaluacionDesempeno = new EvaluacionDesempeno
            {
                IdEvaluacionDesempeno = IdEvaluacionDesempeno,
                id_kpi = IdKpi,
                fecha = Fecha,
                puntuacion = Puntuacion
            };

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
