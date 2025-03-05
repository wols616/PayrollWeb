using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
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

        [Authorize]
        public IActionResult VerAgregarContrato(int idEmpleado)
        {
            List<ContratoViewModel> contratos = _contrato.ObtenerContratosYPuestos(idEmpleado);
            Empleado empleado = _empleado.ObtenerEmpleado(idEmpleado);
            ViewBag.Contratos = contratos;
            ViewBag.Puestos = _puesto.ObtenerPuestosViewModel();
            ViewBag.Empleado = empleado;
            return View("/Views/Admin/AgregarContrato.cshtml");
        }

        [Authorize]
        public IActionResult VerEmpleados(bool showActions = false)
        {
            List<Empleado> empleados = _empleado.MostrarEmpleados();
            ViewBag.ShowActions = showActions;
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        //_______________________________________________________________________________________________________________________

        public IActionResult CrearContrato(ContratoViewModel _contrato)
        {
            Contrato contrato = new Contrato
            {
                IdEmpleado = _contrato.IdEmpleado,
                FechaAlta = _contrato.FechaAlta,
                FechaBaja = _contrato.FechaBaja,
                IdPuesto = _contrato.IdPuesto,
                TipoContrato = _contrato.TipoContrato,
                Vigente = _contrato.Vigente,
            };
            contrato.AgregarContrato();

            return RedirectToAction("VerContratosEmpleado", "Contrato", new { idEmpleado = contrato.IdEmpleado });
        }

        public IActionResult CancelarContrato(int idContrato)
        {
            Contrato contrato = _contrato.ObtenerContrato(idContrato);

            contrato.ActualizarContrato("vigente", "N");
            contrato.ActualizarContrato("fecha_baja", DateTime.Now);
            return RedirectToAction("VerContratosEmpleado", "Contrato", new { idEmpleado = contrato.IdEmpleado });
        }
    }
}
