using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Graficos
{
    public class GraficosController : Controller
    {
        Grafico Grafico = new Grafico();
        Empleado _Empleado = new Empleado();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerGraficos()
        {
            return View();
        }


        public IActionResult VerEmpleados()
        {

            ViewBag.ShowActions = "HistorialAscensos";
            return View("/Views/Admin/VerEmpleados.cshtml", _Empleado.ObtenerEmpleados());
        }

        [Authorize]
        public IActionResult VerEvaluacionesEmpleado()
        {
            List<Empleado> empleados = _Empleado.ObtenerEmpleados();
            ViewBag.ShowActions = "Evaluaciones";
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        public IActionResult VerAscensosEmpleados(int IdEmpleado)
        {
            ViewBag.Empleado = _Empleado.ObtenerEmpleado(IdEmpleado);
            return View("/Views/Graficos/HistorialAscensos.cshtml");
        }

        public IActionResult VerAscensosGlobal()
        {
            return View("/Views/Graficos/HistorialAscensosGlobal.cshtml");
        }
        //-------------------------------------------------------------------------------------------------------------

        public IActionResult ObtenerSalarioPorCategoria()
        {
            return Json(Grafico.ObtenerSalarioPorCategoria());
        }

        public IActionResult ObtenerSalarioPorCategoriaConDetalles()
        {
            var datosGrafica = Grafico.ObtenerDatosParaGraficaPuestos();
            return View("/Views/Graficos/DistribucionSalarial.cshtml", datosGrafica);
        }

        public IActionResult ObtenerHistorialAscensos(int IdEmpleado)
        {
            return Json(Grafico.ObtenerHistorialAscensos(IdEmpleado));
        }

        public IActionResult ObtenerHistorialAscensosGlobal()
        {
            return Json(Grafico.ObtenerHistorialAscensosGlobal());
        }

    }
}
