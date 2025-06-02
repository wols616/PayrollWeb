using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Emp
{
    public class PerfilController : Controller
    {
        Empleado _empleado = new Empleado();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerPerfil()
        {
            var EmpleadoIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpleado");
            int EmpleadoId = Int32.Parse(EmpleadoIdClaim.Value);

            Empleado empleado = _empleado.ObtenerEmpleado(EmpleadoId);
            ViewBag.Empleado = empleado;
            return View("/Views/Emp/Perfil.cshtml", empleado);
        }

        //Métodos para manejar la lógica
        //_______________________________________________________________________________________________________________________________________________________________
        public IActionResult ActualizarDatosGenerales(int idEmpleado, string dui, string nombre, string apellidos, string telefono, string direccion)
        {
            if (!_empleado.EsDUIUnico(dui, idEmpleado))
            {
                TempData["Error"] = "El DUI ya está registrado.";
            }
            if(!_empleado.EsTelefonoUnico(telefono, idEmpleado))
            {
                TempData["Error"] = "El teléfono ya está registrado.";
            }

            if (_empleado.ActualizarDatosGenerales(idEmpleado, dui, nombre, apellidos, telefono, direccion))
            {
                TempData["Success"] = "Datos generales actualizados correctamente.";
            }
            return RedirectToAction("VerPerfil");
        }

        public IActionResult ActualizarDatosSensibles(int idEmpleado, string correo, string cuentaCorriente, string passwordActual, string nuevaPassword, string confirmarPassword)
        {
            //Verificar si el correo ya está registrado
            if (!_empleado.EsCorreoUnico(correo, idEmpleado))
            {
                TempData["Error"] = "El correo ya está registrado.";
                return RedirectToAction("VerPerfil"); // Redirigir a la vista del perfil con el mensaje de error
            }

            // Verificar que la contraseña actual es correcta
            if (!_empleado.ValidarPassword(idEmpleado, Metodos.EncriptarContrasena(passwordActual)))
            {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return RedirectToAction("VerPerfil"); // Redirigir a la vista del perfil con el mensaje de error
            }

            if (!_empleado.ValidarFormatoPassword(nuevaPassword))
            {
                TempData["Error"] = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial.";
                return RedirectToAction("VerPerfil"); // Redirigir a la vista del perfil con el mensaje de error
            }

            // Validar que las nuevas contraseñas coincidan si se quiere cambiar
            if (!string.IsNullOrEmpty(nuevaPassword) && nuevaPassword != confirmarPassword)
            {
                TempData["Error"] = "Las contraseñas nuevas no coinciden.";
                return RedirectToAction("VerPerfil"); // Redirigir a la vista del perfil con el mensaje de error
            }

            // Si no se proporciona una nueva contraseña, mantener la contraseña actual
            string contrasenaFinal = string.IsNullOrEmpty(nuevaPassword) ? null : Metodos.EncriptarContrasena(nuevaPassword);

            // Actualizar los datos
            _empleado.ActualizarDatosSensibles(idEmpleado, correo, cuentaCorriente, contrasenaFinal);

            TempData["Success"] = "Datos sensibles actualizados correctamente.";
            return RedirectToAction("VerPerfil");
        }
    }
}
