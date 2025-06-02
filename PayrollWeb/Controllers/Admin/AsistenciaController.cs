using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PayrollWeb.Models;
using System.Collections.Generic;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;


namespace PayrollWeb.Controllers.Admin
{
    public class AsistenciaController : Controller
    {
        Asistencia _asistencia = new Asistencia();
        Empleado _empleado = new Empleado();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuscarNombrePorCorreo(string correo)
        {
            List<Empleado> empleados = _empleado.ObtenerEmpleados();

            Empleado empleadoEncontrado = empleados
                .FirstOrDefault(e => e.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));

            if (empleadoEncontrado != null)
            {
                var resultado = new
                {
                    success = true,
                    nombreCompleto = empleadoEncontrado.Nombre + " " + empleadoEncontrado.Apellidos
                };
                return Json(resultado);
            }
            else
            {
                return Json(new { success = false });
            }
        }




        // Ver la asistencia de un empleado específico

        public IActionResult VerAsistenciaEmpleado(string dui)
        {
            List<string> listaAsistencia = _asistencia.VerAsistencia(dui);
            return View("/Views/Admin/Asistencia.cshtml", listaAsistencia);
        }

        public IActionResult EditarAsistencia(int id, string fecha)
        {
            Console.WriteLine($"Fecha seleccionada: {fecha}");
            Console.WriteLine($"ID del empleado: {id}");

            List<string> listaAsistenciaMostrar = _asistencia.ObtenerAsistenciaPorEmpleadoYFecha(id, fecha);
           

            return View("/Views/Admin/EditarAsistencia.cshtml", listaAsistenciaMostrar);
        }

        public IActionResult AsistenciaEmpleados()
        {
            List<string> listaAsistencias = _asistencia.VerAsistencia();

            // Agrupamos las asistencias por fecha
            var asistenciaPorFecha = listaAsistencias
                .GroupBy(a => a.Split('|')[0]) // Agrupamos por la fecha (primer valor en la cadena)
                .ToList();


            return View("/Views/Admin/AsistenciaEmpleados.cshtml", asistenciaPorFecha);
        }


        [HttpPost]
        public IActionResult GuardarEdicion(int idEmpleado, string fechaAsistencia, string horaEntrada, string horaSalida)
        {
            bool actualizacionExitosa = _asistencia.ActualizarAsistencia(idEmpleado, fechaAsistencia, horaEntrada, horaSalida);

            return Json(new
            {
                success = actualizacionExitosa,
                message = actualizacionExitosa
                    ? "La asistencia ha sido actualizada correctamente."
                    : "Hubo un error al actualizar la asistencia."
            });
        }


        [HttpGet]
        public IActionResult AsistenciaEntrada()
        {
            // Pasamos una lista vacía para evitar el error en la vista
            List<Asistencia> listaAsistencia = new List<Asistencia>();
            return View("/Views/Admin/AsistenciaEntrada.cshtml", listaAsistencia);
        }


        [HttpGet]
        public IActionResult AgregarAsistencia()
        {
            // Pasamos una lista vacía para evitar el error en la vista
            List<Asistencia> listaAsistencia = new List<Asistencia>();
            return View("/Views/Admin/AgregarAsistencia.cshtml", listaAsistencia);
        }

        public IActionResult AsistenciaSalida()
        {
            // Pasamos una lista vacía para evitar el error en la vista
            List<Asistencia> listaAsistencia = new List<Asistencia>();
            return View("/Views/Admin/AsistenciaSalida.cshtml", listaAsistencia);
        }


        [HttpPost]
        public IActionResult AgregarAsistenciaEntrada(int id, DateTime fecha, TimeSpan horaEntrada, TimeSpan horaSalida, string ausencia)
        {           
            try
            {
                // Registrar la asistencia y obtener la lista actualizada
                bool verificacion = _asistencia.RegistrarAsistenciaEntrada(id, fecha, horaEntrada, horaSalida, ausencia);

                if (verificacion)
                {
                    return Json(new { success = true, message = "Asistencia agregada con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo agregar la asistencia." });
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones y responder con un error
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Error al agregar la asistencia." });
            }
        }


        [HttpPost]
        public IActionResult AgregarAsistenciaSalida(int id, DateTime fecha, TimeSpan nuevaHoraSalida)
        {
            try
            {
                // Registrar la asistencia y obtener la lista actualizada
                bool verificacion = _asistencia.RegistrarHoraSalida(id, fecha, nuevaHoraSalida);

                if (verificacion)
                {
                    return Json(new { success = true, message = "Asistencia agregada con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo agregar la asistencia." });
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones y responder con un error
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Error al agregar la hora de salida." });
            }
        }

        [HttpGet]
        public IActionResult ObtenerHoraEntrada(int id, string fecha)
        {
            List<string> asistencia = _asistencia.ObtenerAsistenciaPorEmpleadoYFecha(id, fecha);

            if (asistencia.Count > 0)
            {
                string[] datos = asistencia[0].Split('|'); // Datos en el orden definido en el modelo
                string horaEntrada = datos[4]; // La posición 4 es la hora de entrada
                return Json(new { success = true, horaEntrada });
            }

            return Json(new { success = false, message = "No se encontró hora de entrada para este empleado." });
        }




        public JsonResult ObtenerEmpleadosSinAsistencia(DateTime fecha)
        {
            var empleados = _asistencia.EmpleadosSinAsistencia(fecha);
            return Json(empleados);  // Empleados es una lista de strings con el formato "nombre apellidos"
        }


        public JsonResult ObtenerEmpleadosSinSalida(DateTime fecha)
        {
            var empleados = _asistencia.EmpleadosSinSalida(fecha);

            return Json(empleados);  // Empleados es una lista de strings con el formato "nombre apellidos"
        }


        public JsonResult ObtenerMotivoAusencia(int idEmpleado, string fecha)
        {
            var motivo = _asistencia.ObtenerAusenciaPorEmpleadoYFecha(idEmpleado, fecha); // Llama a tu método que obtiene el motivo de la ausencia
            return Json(new { success = true, motivo = motivo });
        }


        [HttpPost]
        public JsonResult ActualizarAusencia(int idEmpleado, string fecha, string ausencia)
        {
            var resultado = _asistencia.ActualizarAusencia(idEmpleado, fecha, ausencia); 

            return Json(new { success = true, message = "Datos actualizados" });

        }

        public IActionResult AsistenciaAusencia()
        {
            return View("/Views/Admin/AsistenciaAusencia.cshtml");
        }

        [HttpPost]
        public JsonResult RegistrarAusencia(int idEmpleado, string fecha, string ausencia)
        {
            var resultado = _asistencia.InsertarAusencia(idEmpleado, fecha, ausencia);

            return Json(new { success = true, message = "Ausencia Ingresada" });

        }

        public JsonResult ObtenerEmpleadosSinAsistenciaNiAusencia(DateTime fecha)
        {
            var empleados = _asistencia.ObtenerEmpleadosSinAsistenciaNiAusencia(fecha);
            return Json(empleados);  // Empleados es una lista de strings con el formato "nombre apellidos"
        }


    }
}
