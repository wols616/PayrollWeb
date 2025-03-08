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
            return View("/Views/Emp/Perfil.cshtml", empleado);
        }

        //Métodos para manejar la lógica
        //_______________________________________________________________________________________________________________________________________________________________
        public IActionResult ActualizarDatosGenerales(int idEmpleado, string dui, string nombre, string apellidos, string telefono, string direccion)
        {
            _empleado.ActualizarDatosGenerales(idEmpleado, dui, nombre, apellidos, telefono, direccion);
            return RedirectToAction("VerPerfil");
        }

        public IActionResult ActualizarDatosSensibles(int idEmpleado, string correo, string cuentaCorriente, string passwordActual, string nuevaPassword, string confirmarPassword)
        {
            // Verificar que la contraseña actual es correcta
            if (!_empleado.ValidarPassword(idEmpleado, passwordActual))
            {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return RedirectToAction("VerPerfil"); // Redirigir a la vista del perfil con el mensaje de error
            }

            // Validar que las nuevas contraseñas coincidan si se quiere cambiar
            if (!string.IsNullOrEmpty(nuevaPassword) && nuevaPassword != confirmarPassword)
            {
                TempData["Error"] = "Las contraseñas nuevas no coinciden.";
                return RedirectToAction("VerPerfil"); // Redirigir a la vista del perfil con el mensaje de error
            }

            // Si no se proporciona una nueva contraseña, mantener la contraseña actual
            string contrasenaFinal = string.IsNullOrEmpty(nuevaPassword) ? null : nuevaPassword;

            // Actualizar los datos
            _empleado.ActualizarDatosSensibles(idEmpleado, correo, cuentaCorriente, contrasenaFinal);

            TempData["Success"] = "Datos sensibles actualizados correctamente.";
            return RedirectToAction("VerPerfil");
        }
    }
}
