using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class HabilidadController : Controller
    {
        Habilidad _habilidad = new Habilidad();

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult VerHabilidades(string ShowActions)
        {
            ViewBag.ShowActions = ShowActions;
            List<Habilidad> habilidades = _habilidad.ObtenerHabilidades();
            return View("/Views/Admin/VerHabilidades.cshtml", habilidades);
        }

        // Métodos para lógica de habilidades
        [Authorize]
        public IActionResult CrearHabilidad(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                TempData["Error"] = "El nombre de la habilidad no puede estar vacío.";
                return RedirectToAction("VerHabilidades");
            }

            Habilidad habilidad = new Habilidad(Nombre);
            if (habilidad.AgregarHabilidad())
            {
                TempData["Success"] = "Habilidad agregada correctamente";
            }
            else
            {
                TempData["Error"] = "Error al agregar la habilidad.";
            }
            return RedirectToAction("VerHabilidades");
        }


        [Authorize]
        public IActionResult ActualizarHabilidad(int IdHabilidad, string Nombre)
        {
            Habilidad habilidad = new Habilidad(IdHabilidad, Nombre);
            if (habilidad.EditarHabilidad())
            {
                TempData["Success"] = "Habilidad actualizada correctamente";
            }
            else
            {
                TempData["Error"] = "Error al actualizar la habilidad";
            }
            return RedirectToAction("VerHabilidades");
        }

        [Authorize]
        public IActionResult EliminarHabilidad(int IdHabilidad)
        {
            _habilidad.IdHabilidad = IdHabilidad;
            if (!_habilidad.EliminarHabilidad())
            {
                TempData["Error"] = "No se puede eliminar la habilidad porque está asociada a otros registros";
                return RedirectToAction("VerHabilidades");
            }
            TempData["Success"] = "Habilidad eliminada correctamente";
            return RedirectToAction("VerHabilidades");
        }

    }
}
