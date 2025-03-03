using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;
using System.Reflection.Metadata;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace PayrollWeb.Controllers.Admin
{
    public class EmpleadoController : Controller
    {
        Empleado _empleado = new Empleado();
        MetodosUtiles metodosUtiles = new MetodosUtiles();

        public IActionResult Index()
        {
            return View();
        }

        //CONTROLADORES PARA ABRIR LAS VISTAS
        public IActionResult VerEmpleados(bool showActions = true)
        {
            List<Empleado> empleados = _empleado.MostrarEmpleados();
            ViewBag.ShowActions = showActions;
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        public IActionResult VerEmpleado(int id)
        {
            Empleado empleado = _empleado.ObtenerEmpleado(id);

            return View("/Views/Admin/VerEmpleado.cshtml", empleado);
        }

        //Método para mostrar la vista de agregar empleado
        public IActionResult VerAgregarEmpleado()
        {
            return View("/Views/Admin/AgregarEmpleado.cshtml");
        }

        public IActionResult VerEditarEmpleado(int id)
        {
            Empleado empleado = _empleado.ObtenerEmpleado(id);
            return View("/Views/Admin/EditarEmpleado.cshtml", empleado);
        }

        //__________________________________________________________________________________________________________________________________
        //CONTROLADORES PARA CONTROLAR LA LÓGICA
        [HttpPost]
        public IActionResult CrearEmpleado(Empleado empleado)
        {
            //empleado.Estado = "Activo";

            if (!_empleado.EsDUIUnico(empleado.Dui))
            {
                ModelState.AddModelError("Dui", "El DUI ya está registrado");
            }
            if (!_empleado.EsCorreoUnico(empleado.Correo))
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado");
            }
            if (!_empleado.EsCuentaUnica(empleado.CuentaCorriente))
            {
                ModelState.AddModelError("CuentaCorriente", "La cuenta corriente ya está registrada");
            }

            if (!ModelState.IsValid)
            {
                // Regresar a la vista "VerAgregarEmpleado" y pasar el modelo actual con los errores
                return View("/Views/Admin/AgregarEmpleado.cshtml", empleado);
            }
            empleado.AgregarEmpleado();
            return RedirectToAction("VerEmpleados");
        }

        [HttpPost]
        public IActionResult ActualizarEmpleado(Empleado empleado)
        {
            Empleado existingEmpleado = _empleado.ObtenerEmpleado(empleado.IdEmpleado);

            if (existingEmpleado == null)
            {
                ModelState.AddModelError(string.Empty, "Empleado no encontrado");
                return View("EditarEmpleado", empleado); // Retorna la vista con errores
            }

            // Validar DUI si cambió
            if (empleado.Dui != existingEmpleado.Dui && !_empleado.EsDUIUnico(empleado.Dui))
            {
                ModelState.AddModelError("Dui", "El DUI ya está registrado");
            }

            // Validar Correo si cambió
            if (empleado.Correo != existingEmpleado.Correo && !_empleado.EsCorreoUnico(empleado.Correo))
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado");
            }

            // Validar Cuenta Corriente si cambió
            if (empleado.CuentaCorriente != existingEmpleado.CuentaCorriente && !_empleado.EsCuentaUnica(empleado.CuentaCorriente))
            {
                ModelState.AddModelError("CuentaCorriente", "La cuenta corriente ya está registrada");
            }

            // Si hay errores, regresa a la vista con el modelo y los errores
            if (!ModelState.IsValid)
            {
                return View("/Views/Admin/EditarEmpleado.cshtml", empleado);
            }

            _empleado.EditarEmpleado(empleado);
            return RedirectToAction("VerEmpleados");
        }



        public IActionResult EliminarEmpleado(int id)
        {
            bool resultado = _empleado.EliminarEmpleado(id);
            if (!resultado)
            {
                TempData["ErrorEliminar"] = "No se puede eliminar el empleado porque tiene contratos asociados.";
            }
            return RedirectToAction("VerEmpleados");
        }



        public IActionResult GenerarContrasena()
        {
            string nuevaContrasena = _empleado.GenerarContrasena();
            return Content(nuevaContrasena);  // Devuelve la contraseña como texto plano
        }

    }
}
