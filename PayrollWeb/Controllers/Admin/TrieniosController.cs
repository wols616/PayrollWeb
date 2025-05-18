using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class TrieniosController : Controller
    {
        // Método para obtener y mostrar los trienios
        public IActionResult VerTrienios()
        {
            // Crear una instancia de la clase Trienios
            Trienios trienios = new Trienios();
            List<Trienios> listaTrienios = trienios.MostrarTrienios();
            var valoresDispersos = new List<int> { 7, 10, 9, 10, 8, 10, 10, 6, 10, 9 };

            ViewBag.ValoresDispersos = valoresDispersos;

            // Pasar la lista de trienios a la vista
            return View("/Views/Admin/VerTrienios.cshtml", listaTrienios);
        }

        // Acción para ejecutar el procedimiento almacenado CalcularTrienios
        [HttpPost]
        public IActionResult CalcularTrienios()
        {
            try
            {
                // Crear una instancia de la clase Trienios
                Trienios trienios = new Trienios();
                trienios.CalcularTrienios(); // Llamar al método que ejecuta el procedimiento almacenado

                // Redirigir de nuevo a la vista VerTrienios después de ejecutar el procedimiento
                return RedirectToAction("VerTrienios");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error al calcular los trienios: " + ex.Message);
                return View("Error"); // Puedes crear una vista de error si lo deseas
            }
        }
    }
}
