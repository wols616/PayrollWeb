using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class ContratoController : Controller
    {
        Empleado _empleado = new Empleado();
        Contrato _contrato = new Contrato();
        Puesto _puesto = new Puesto();
        Categoria _categoria = new Categoria();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult VerContratosEmpleado(int idEmpleado)
        {
            if (idEmpleado <= 0)
            {
                return BadRequest("ID de empleado no válido");
            }

            var empleado = _empleado.ObtenerEmpleado(idEmpleado);

            if (empleado == null)
            {
                return NotFound("El empleado no fue encontrado");
            }

            List<ContratoViewModel> contratos = _contrato.ObtenerContratosYPuestos(idEmpleado);

            if (contratos == null || !contratos.Any())
            {
                ViewBag.Mensaje = "No hay contratos disponibles para este empleado.";
            }

            ViewBag.Empleado = empleado;
            return View("/Views/Admin/VerContratosEmpleado.cshtml", contratos);
        }

        public IActionResult VerEditarContrato(int IdContrato, int idEmpleado)
        {
            ContratoViewModel contrato = _contrato.ObtenerContratoYPuesto(IdContrato);
            Empleado empleado = _empleado.ObtenerEmpleado(idEmpleado);
            ViewBag.Empleado = empleado;

            ViewBag.Puestos = _puesto.ObtenerPuestosViewModel();
            
            
            return View("/Views/Admin/EditarContrato.cshtml", contrato);
        }


        public IActionResult VerEmpleados(bool showActions = false)
        {
            List<Empleado> empleados = _empleado.MostrarEmpleados();
            ViewBag.ShowActions = showActions;
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        //_______________________________________________________________________________________________________________________

        [HttpPost]
        public IActionResult ActualizarContrato(ContratoViewModel _contrato)
        {
            try
            {
                // Mapear el ViewModel al modelo de base de datos
                Contrato contrato = new Contrato
                {
                    IdContrato = _contrato.IdContrato,
                    IdEmpleado = _contrato.IdEmpleado, // ✅ Ahora se obtiene directamente del ViewModel
                    FechaAlta = _contrato.FechaAlta,
                    FechaBaja = _contrato.FechaBaja,
                    IdPuesto = _contrato.IdPuesto,
                    TipoContrato = _contrato.TipoContrato
                };

                // Llamar al método de actualización en el modelo
                contrato.ActualizarContrato();

                // Redirigir a la vista de contratos del empleado
                return RedirectToAction("VerContratosEmpleado", "Contrato", new { idEmpleado = contrato.IdEmpleado });
            }
            catch (Exception ex)
            {
                // Manejar errores
                ModelState.AddModelError("", "Ocurrió un error al actualizar el contrato: " + ex.Message);
                return View(_contrato);
            }
        }

    }
}
