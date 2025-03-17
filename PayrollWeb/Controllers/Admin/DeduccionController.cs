using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class DeduccionController : Controller
    {
        Deduccion _deduccion = new Deduccion();
        public IActionResult Index()
        {
            return View();
        }

        //Métodos para mostrar las vistas
        public IActionResult VerDeduccionesSelect()
        {
            return View("/Views/Admin/DeduccionesSelect.cshtml");
        }

        [Authorize]
        public IActionResult VerDeducciones()
        {
            List<Deduccion> deducciones = _deduccion.ObtenerDeducciones();
            return View("/Views/Admin/VerDeducciones.cshtml", deducciones);
        }

        //Métodos para la lógica
        [Authorize]
        public IActionResult CrearDeduccion(string NombreDeduccion, decimal Porcentaje, string Fija)
        {
            Deduccion deduccion = new Deduccion(NombreDeduccion, Porcentaje, Fija);
            if (deduccion.ExisteDeduccion())
            {
                TempData["Error"] = "La deducción ya está registrada";
                return RedirectToAction("VerDeducciones");
            }
            if (deduccion.Porcentaje < 0 || deduccion.Porcentaje > 100)
            {
                TempData["Error"] = "El porcentaje debe ser un valor entre 0 y 100";
                return RedirectToAction("VerDeducciones");
            }
            if (deduccion.AgregarDeduccion())
            {
                TempData["Success"] = "Deducción agregada correctamente";
            }
            return RedirectToAction("VerDeducciones");
        }
        [Authorize]
        public IActionResult ActualizarDeduccion(int IdDeduccion, string NombreDeduccion, decimal Porcentaje, string Fija)
        {
            Deduccion deduccion = new Deduccion(IdDeduccion, NombreDeduccion, Porcentaje, Fija);

            // Verificar si el porcentaje está dentro del rango válido
            if (deduccion.Porcentaje < 0 || deduccion.Porcentaje > 100)
            {
                TempData["Error"] = "El porcentaje debe ser un valor entre 0 y 100";
                return RedirectToAction("VerDeducciones");
            }

            // Verificar si la deducción ya existe (solo si el nombre ha cambiado)
            if (deduccion.ExisteDeduccion() && deduccion.NombreDeduccion != _deduccion.ObtenerDeduccion(IdDeduccion).NombreDeduccion)
            {
                TempData["Error"] = "La deducción ya está registrada";
                return RedirectToAction("VerDeducciones");
            }

            // Actualizar la deducción
            if (deduccion.EditarDeduccion())
            {
                TempData["Success"] = "Deducción actualizada correctamente";
            }
            else
            {
                TempData["Error"] = "Error al actualizar la deducción";
            }

            return RedirectToAction("VerDeducciones");
        }

        [Authorize]
        public IActionResult EliminarDeduccion(int id)
        {
            bool resultado = _deduccion.EliminarDeduccion(id);
            if (!resultado)
            {
                TempData["Error"] = "No se puede eliminar la deducción porque ya hay contratos asociados a esta";
                return RedirectToAction("VerDeducciones");
            }
            TempData["Success"] = "Deducción eliminada correctamente";
            return RedirectToAction("VerDeducciones");
        }
    }
}
