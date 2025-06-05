using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace PayrollWeb.Controllers.Admin
{
    public class PerfilAdministradorController : Controller
    {
        Administrador _admin = new Administrador();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerPerfil()
        {
            var adminIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdAdministrador");
            int adminId = int.Parse(adminIdClaim.Value);

            Administrador administrador = _admin.ObtenerAdministrador(adminId);
            ViewBag.Administrador = administrador;

            return View("/Views/Admin/PerfilAdministrador.cshtml", administrador);
        }

        [HttpPost]
        public IActionResult ActualizarDatosGeneralesAdministrador(int idAdministrador, string dui, string nombre, string apellidos, string telefono)
        {
            // Puedes agregar validaciones si tienes métodos como EsDUIUnico o EsTelefonoUnico en la clase Administrador

            bool actualizado = _admin.ActualizarDatosGenerales(idAdministrador, dui, nombre, apellidos, telefono);

            if (actualizado)
            {
                TempData["Success"] = "Datos generales actualizados correctamente.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al actualizar los datos generales.";
            }

            return RedirectToAction("VerPerfil");
        }



        // Validar correo si hay método similar en tu clase (opcional)
            // if (!_admin.EsCorreoUnico(correo, idAdministrador)) { ... }

            //SIN ENCRIPTAR
            //if (!_admin.ValidarPasswordSin(idAdministrador, passwordActual))   

            //if (!_admin.ValidarPassword(idAdministrador, Metodos.EncriptarContrasena(passwordActual)))con encriptar

       [HttpPost]
        public IActionResult ActualizarDatosSensiblesAdministrador(int idAdministrador, string correo, string passwordActual, string nuevaPassword, string confirmarPassword)
        {
            if (!_admin.ValidarPasswordSin(idAdministrador, passwordActual))
                {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return RedirectToAction("VerPerfil");
            }

            if (!string.IsNullOrEmpty(nuevaPassword) && nuevaPassword != confirmarPassword)
            {
                TempData["Error"] = "Las contraseñas nuevas no coinciden.";
                return RedirectToAction("VerPerfil");
            }

            //cambiar la linea de contrase;a encriptada
            string contrasenaFinal = string.IsNullOrEmpty(nuevaPassword) ? null : nuevaPassword;
            bool actualizado = _admin.ActualizarDatosSensibles(idAdministrador, correo, contrasenaFinal);

            if (actualizado)
                TempData["Success"] = "Datos sensibles actualizados correctamente.";
            else
                TempData["Error"] = "No se pudo actualizar los datos. Verifica que hayan cambiado.";

            return RedirectToAction("VerPerfil");
        }

    }
}
