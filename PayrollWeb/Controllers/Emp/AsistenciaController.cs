using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;
using System.Collections.Generic;

namespace PayrollWeb.Controllers.Emp
{
    public class AsistenciaController : Controller
    {
        Asistencia _asistencia = new Asistencia();

        public IActionResult VerAsistencia(string dui)
        {
            if (string.IsNullOrEmpty(dui))
            {
                return RedirectToAction("Index", "Home"); // o mostrar error
            }

            List<string> asistencia = _asistencia.VerAsistencia(dui);
            return View("/Views/Emp/Asistencia.cshtml", asistencia);
        }

    }
}
