// Controllers/NominaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using PayrollWeb.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewEngines; // Para RenderViewToString
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Para RenderViewToString

namespace PayrollWeb.Controllers
{
    public class NominaController : Controller
    {
        private readonly Empleado _empleadoModel = new Empleado();

        public IActionResult VerNomina()
        {
            return View("/Views/Admin/VerNomina.cshtml");
        }

        [HttpGet]
        public JsonResult BuscarEmpleados(string texto)
        {
            List<Empleado> lista = _empleadoModel.BuscarPorTexto(texto);
            var resultado = lista.Select(e => new
            {
                idEmpleado = e.IdEmpleado,
                nombreCompleto = $"{e.Nombre} {e.Apellidos}",
                dui = e.Dui,
                telefono = e.Telefono,
                correo = e.Correo,
                cuentaCorriente = e.CuentaCorriente,
                direccion = e.Direccion,
                estado = e.Estado
            }).ToList();

            return Json(resultado);
        }

        [HttpPost]
        public IActionResult GenerarNominas()
        {
            try
            {
                var nomina = new Nomina();
                nomina.GenerarNominas();
                return Json(new
                {
                    success = true,
                    message = "¡Proceso completado correctamente!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al generar nóminas: " + ex.Message
                });
            }
        }

        [HttpGet]
        public JsonResult ObtenerNomina(int empleadoId, string mesAnno)
        {
            try
            {
                var nomina = Nomina.ObtenerDetalleNomina(empleadoId, mesAnno);
                if (nomina == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se encontró nómina para este empleado en el mes seleccionado"
                    });
                }

                return Json(new
                {
                    success = true,
                    nomina = nomina
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener nómina: " + ex.Message
                });
            }
        }

        
    }
}
