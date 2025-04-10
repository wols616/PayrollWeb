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
        Puesto_Historico _puestoHistorico = new Puesto_Historico();

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
            List<Contrato> contratos2 = _contrato.ObtenerContratos(idEmpleado);

            if (contratos == null || !contratos.Any())
            {
                ViewBag.Mensaje = "No hay contratos disponibles para este empleado.";
            }

            ViewBag.Empleado = empleado;
            List<Puesto_Historico> puestosHistoricos = new List<Puesto_Historico>();
            foreach (var contrato in contratos)
            {
                if (contrato.Vigente == "N")
                {
                    //puestosHistoricos.Add(new Puesto_Historico().ObtenerPuestoHistorico(contrato.IdContrato));
                    contrato.SueldoBase = _puestoHistorico.ObtenerPuestoHistorico(contrato.IdContrato).SueldoBase;
                } else
                {
                    //puestosHistoricos.Add(null);
                    contrato.SueldoBase = new Puesto().ObtenerSueldoBasePuesto(contrato.IdPuesto);
                }
            }
            //ViewBag.PuestosHistoricos = puestosHistoricos;
            return View("/Views/Admin/VerContratosEmpleado.cshtml", contratos);
        }

        [Authorize]
        public IActionResult VerAgregarContrato(int idEmpleado)
        {
            //Comprobar que no haya un contrato vigente
            if (_contrato.ComprobarContratoVigente(idEmpleado))
            {
                TempData["Error"] = "El empleado ya tiene un contrato vigente.";
                return RedirectToAction("VerContratosEmpleado", new { idEmpleado = idEmpleado });
            }

            List<ContratoViewModel> contratos = _contrato.ObtenerContratosYPuestos(idEmpleado);
            Empleado empleado = _empleado.ObtenerEmpleado(idEmpleado);
            ViewBag.Contratos = contratos;
            ViewBag.Puestos = _puesto.ObtenerPuestosViewModel();
            ViewBag.Empleado = empleado;
            return View("/Views/Admin/AgregarContrato.cshtml");
        }

        [Authorize]
        public IActionResult VerEmpleados()
        {
            List<Empleado> empleados = _empleado.ObtenerEmpleados();
            ViewBag.ShowActions = "Contratos";
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        [Authorize]
        public IActionResult VerEditarContrato(int idContrato, int idEmpleado)
        {
            ViewBag.Puestos = _puesto.ObtenerPuestosViewModel();
            ViewBag.Empleado = _empleado.ObtenerEmpleado(idEmpleado);
            ViewBag.Contratos = _contrato.ObtenerContratosYPuestos(idEmpleado);
            return View("/Views/Admin/EditarContrato.cshtml", _contrato.ObtenerContratoYPuesto(idContrato));
        }



        //_______________________________________________________________________________________________________________________

        public IActionResult CrearContrato(ContratoViewModel _contrato, string Motivo)
        {
            int IdAdministrador = 0;
            var AdminIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdAdministrador");
            if (AdminIdClaim != null)
            {
                IdAdministrador = Int32.Parse(AdminIdClaim.Value);
            }

            // Obtener el contrato anterior
            int? idContratoAnterior = new Contrato().ObtenerContratoAnterior(_contrato.IdEmpleado);


            Contrato contrato = new Contrato
            {
                IdEmpleado = _contrato.IdEmpleado,
                FechaAlta = _contrato.FechaAlta,
                FechaBaja = _contrato.FechaBaja,
                IdPuesto = _contrato.IdPuesto,
                TipoContrato = _contrato.TipoContrato,
                Vigente = _contrato.Vigente,
            };

            if (contrato.AgregarContrato())
            {
                _empleado.ActualizarCampoEmpleado(contrato.IdEmpleado, "estado", "Activo");
                TempData["Success"] = "Contrato creado correctamente.";

                if (idContratoAnterior == null)
                {
                    Historial_Contrato historial_Contrato = new Historial_Contrato
                    {

                        IdContratoAnterior = contrato.IdContrato,
                        IdContratoNuevo = contrato.IdContrato,
                        Fecha = DateTime.Now,
                        Cambio = "Creación",
                        Motivo = !string.IsNullOrEmpty(Motivo) ? Motivo : "Creación de contrato",
                        IdAdministrador = IdAdministrador
                    };
                    if (contrato.FechaAlta < DateTime.Now)
                    {
                        historial_Contrato.Fecha = contrato.FechaAlta;
                    }
                    historial_Contrato.AgregarHistorialContrato();
                } else if (idContratoAnterior != null)
                {
                    Historial_Contrato historial_Contrato = new Historial_Contrato
                    {
                        IdContratoAnterior = idContratoAnterior,
                        IdContratoNuevo = contrato.IdContrato,
                        Fecha = DateTime.Now,
                        Cambio = "Creación",
                        Motivo = !string.IsNullOrEmpty(Motivo) ? Motivo : "Creación de contrato",
                        IdAdministrador = IdAdministrador
                    };
                    if (contrato.FechaAlta < DateTime.Now)
                    {
                        historial_Contrato.Fecha = contrato.FechaAlta;
                    }
                    historial_Contrato.AgregarHistorialContrato();
                }

                
            }

            return RedirectToAction("VerContratosEmpleado", "Contrato", new { idEmpleado = contrato.IdEmpleado });
        }

        [HttpPost]
        public JsonResult CancelarContrato(int idContrato, string motivo)
        {
            int IdAdministrador = 0;
            var AdminIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdAdministrador");
            if (AdminIdClaim != null)
            {
                IdAdministrador = Int32.Parse(AdminIdClaim.Value);
            }

            Contrato contrato = _contrato.ObtenerContrato(idContrato);
            DateTime? fechaCancelacion = DateTime.Now;
            if (contrato.FechaBaja < DateTime.Now)
            {
                fechaCancelacion = contrato.FechaBaja;
            }
                contrato.ActualizarContrato("vigente", "N");
            contrato.ActualizarContrato("fecha_baja", fechaCancelacion);

            //Genero el registro para el historial
            Historial_Contrato historial_Contrato = new Historial_Contrato
            {
                IdContratoAnterior = idContrato,
                IdContratoNuevo = idContrato,
                Fecha = fechaCancelacion,
                Cambio = "Cancelación",
                Motivo = motivo,
                IdAdministrador = IdAdministrador
            };
            historial_Contrato.AgregarHistorialContrato();

            _puestoHistorico.RegistrarPuestoHistorico(idContrato);

            _empleado.ActualizarCampoEmpleado(contrato.IdEmpleado, "estado", "Inactivo");
            return Json(new { success = true });
        }
    }
}
