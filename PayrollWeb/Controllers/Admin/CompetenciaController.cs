using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class CompetenciaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private Competencia _competencia = new Competencia();

        // Vistas
        public IActionResult VerCompetencias(string ShowActions)
        {
            ViewBag.ShowActions = ShowActions;
            List<Competencia> competencias = _competencia.ObtenerCompetencias();
            return View("/Views/Admin/VerCompetencias.cshtml", competencias);
        }


        // Métodos para lógica de competencias
        public IActionResult CrearCompetencia(string Nombre)
        {
            Competencia competencias = new Competencia(Nombre);
            if (competencias.AgregarCompetencia())
            {
                TempData["Success"] = "Competencia agregada correctamente";
            }
            else
            {
                TempData["Error"] = "Error al agregar la competencia";
            }
            return RedirectToAction("VerCompetencias");
        }

        [Authorize]
        public IActionResult ActualizarCompetencia(int IdCompetencia, string Nombre)
        {
            Competencia competencias = new Competencia(IdCompetencia, Nombre);
            if (competencias.EditarCompetencia())
            {
                TempData["Success"] = "Competencia actualizada correctamente";
            }
            else
            {
                TempData["Error"] = "Error al actualizar la competencia";
            }
            return RedirectToAction("VerCompetencias");
        }

        [Authorize]
        public IActionResult EliminarCompetencia(int IdCompetencia)
        {
            _competencia.IdCompetencia = IdCompetencia;
            if (!_competencia.EliminarCompetencia())
            {
                TempData["Error"] = "No se puede eliminar la competencia porque está asociada a empleados.";
                return RedirectToAction("VerCompetencias");
            }
            TempData["Success"] = "Competencia eliminada correctamente";
            return RedirectToAction("VerCompetencias");
        }


    }
}
