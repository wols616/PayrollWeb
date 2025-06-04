using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;
using System.Collections.Generic;

namespace PayrollWeb.Controllers.Emp
{
    public class AsistenciaController : Controller
    {
        Asistencia _asistencia = new Asistencia();

        public IActionResult VerAsistencia()
        {
            string correo = User.Identity.Name;

            if (string.IsNullOrEmpty(correo))
            {
                return RedirectToAction("Index", "Home"); // o mostrar error
            }

            List<string> asistencia = _asistencia.VerAsistencia(correo);
            return View("/Views/Emp/Asistencia.cshtml", asistencia);
        }

    }
}
