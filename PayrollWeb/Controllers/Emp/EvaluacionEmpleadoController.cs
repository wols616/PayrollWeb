using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Emp
{
    public class EmpleadoEvaluacionController : Controller
    {
        private readonly EvaluacionDesempeno _evaluacionDesempeno = new();
        private readonly Empleado _empleado = new();
        private readonly KPI _kpi = new();

        public IActionResult MisEvaluaciones(int idEmpleado, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var empleado = _empleado.ObtenerEmpleado(idEmpleado);
            var evaluaciones = _evaluacionDesempeno.ObtenerEvaluacionesDeEmpleado(idEmpleado);

            bool tieneFiltroFecha = fechaInicio.HasValue || fechaFin.HasValue;

            if (tieneFiltroFecha)
            {
                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    evaluaciones = evaluaciones
                        .Where(e => e.fecha == fechaInicio.Value)
                        .ToList();
                }
                else if (fechaInicio.HasValue)
                {
                    evaluaciones = evaluaciones
                        .Where(e => e.fecha >= fechaInicio.Value)
                        .ToList();
                }
            }

            var kpis = _kpi.ObtenerKPI();
            var datosGrafica = new List<double>();

            if (tieneFiltroFecha)
            {
                foreach (var kpi in kpis)
                {
                    var evaluacion = evaluaciones.FirstOrDefault(e => e.id_kpi == kpi.IdKpi);
                    datosGrafica.Add(evaluacion != null ? evaluacion.puntuacion : 0);
                }
            }
            else
            {
                var evaluacionesAgrupadasProm = evaluaciones
                    .GroupBy(e => e.id_kpi)
                    .ToDictionary(g => g.Key, g => g.Average(e => e.puntuacion));

                foreach (var kpi in kpis)
                {
                    datosGrafica.Add(evaluacionesAgrupadasProm.TryGetValue(kpi.IdKpi, out var promedio) ? promedio : 0);
                }
            }

            // ✅ Agrupar por fecha (clave final para la vista)
            var evaluacionesAgrupadas = evaluaciones
                .GroupBy(e => e.fecha.Date)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            // ViewBags
            ViewBag.Empleado = empleado;
            ViewBag.KPIs = kpis;
            ViewBag.Evaluaciones = evaluaciones;
            ViewBag.DatosGrafica = datosGrafica;
            ViewBag.TieneFiltroFecha = tieneFiltroFecha;
            ViewBag.EvaluacionesAgrupadas = evaluacionesAgrupadas; // ✅ Agregado

            return View("~/Views/Emp/VerEvaluacionesEmpleado.cshtml", evaluaciones);
        }

    }
}
