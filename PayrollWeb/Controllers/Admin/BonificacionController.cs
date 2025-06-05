// /Controllers/BonificacionController.cs
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;
using System;
using System.Collections.Generic;

namespace PayrollWeb.Controllers
{
    public class BonificacionController : Controller
    {
        private readonly Empleado _empleadoModel = new Empleado();
        private readonly Bonificacion _bonoModel = new Bonificacion();
        private readonly CategoriaBonificacion _categoriaModel = new CategoriaBonificacion();

        // GET: /Bonificacion/Index
        public IActionResult VerBonificacion()
        {
            // Cargar lista de categorías para el combobox
            ViewBag.Categorias = _categoriaModel.ObtenerTodas();
            // Cargar todas las bonificaciones inicialmente
            ViewBag.Bonificaciones = _bonoModel.ObtenerTodas();
            return View("/Views/Admin/VerBonificacion.cshtml");
        }

        /// <summary>
        /// Busca empleados (usado desde AJAX) y devuelve JSON.
        /// </summary>
        [HttpGet]
        public JsonResult BuscarEmpleados(string texto)
        {
            var lista = _empleadoModel.BuscarPorTexto(texto);
            // Construir un objeto JSON mínimo (id + nombre + apellidos)
            var resultado = new List<object>();
            foreach (var e in lista)
            {
                decimal sueldoBase = _empleadoModel.ObtenerSueldoBaseEmpleado(e.IdEmpleado);
                resultado.Add(new
                {
                    e.IdEmpleado,
                    NombreCompleto = $"{e.Nombre} {e.Apellidos}",
                    SueldoBase = sueldoBase
                });
            }
            return Json(resultado);
        }


        /// <summary>
        /// Obtiene las bonificaciones de un empleado específico (o todas si idEmpleado=0).
        /// </summary>
        [HttpGet]
        public JsonResult ObtenerBonificaciones(int idEmpleado = 0)
        {
            var lista = _bonoModel.ObtenerTodas(idEmpleado);
            var resultado = new List<object>();
            foreach (var b in lista)
            {
                string nombreCategoria = "";
                if (b.CategoriaId > 1)
                {
                    // Obtener nombre de la categoría (puede optimizarse con diccionario)
                    var cat = _categoriaModel.ObtenerTodas().Find(c => c.IdCategoriaBono == b.CategoriaId);
                    nombreCategoria = cat != null ? cat.Nombre : "-";
                }
                else
                {
                    nombreCategoria = "Aguinaldo";
                }

                resultado.Add(new
                {
                    b.IdBonificacion,
                    b.IdEmpleado,
                    Categoria = nombreCategoria,
                    Monto = b.Monto,
                    Fecha = b.Fecha
                });
            }
            return Json(resultado);
        }

        /// <summary>
        /// Agrega una bonificación nueva (normal o aguinaldo).
        /// </summary>
        [HttpPost]
        public JsonResult AgregarBonificacion(
            int? idEmpleado,
            int? categoriaId,
            decimal? monto,
            DateTime? fecha,
            bool aguinaldo,
            decimal? salarioBase,
            decimal? aniosServicio,
            bool paraTodos)
        {
            bool exito = false;
            string mensaje = "";

            // Validaciones básicas
            if (paraTodos)
            {
                if (!categoriaId.HasValue || !monto.HasValue || !fecha.HasValue)
                {
                    mensaje = "Debe proporcionar categoría, monto y fecha para aplicar a todos.";
                    return Json(new { exito = false, mensaje });
                }

                exito = _bonoModel.AgregarATodos(categoriaId.Value, monto.Value, fecha.Value);
                mensaje = exito ? "Bonificación aplicada a todos los empleados." : "Error al aplicar bonificación a todos.";
            }
            else
            {
                if (!idEmpleado.HasValue)
                {
                    mensaje = "Debe seleccionar un empleado.";
                    return Json(new { exito = false, mensaje });
                }

                if (aguinaldo)
                {
                    if (!salarioBase.HasValue || !aniosServicio.HasValue || !fecha.HasValue)
                    {
                        mensaje = "Para calcular aguinaldo, debe ingresar salario base, años de servicio y fecha.";
                        return Json(new { exito = false, mensaje });
                    }

                    exito = _bonoModel.AgregarAguinaldo(
                    idEmpleado.Value,
                    salarioBase.Value,
                    Convert.ToInt32(aniosServicio.Value),
                    fecha.Value);

                    mensaje = exito
                        ? "Aguinaldo agregado correctamente."
                        : $"Error al agregar aguinaldo. Datos: idEmpleado={idEmpleado.Value}, salarioBase={salarioBase.Value}, aniosServicio={Convert.ToInt32(aniosServicio.Value)}, fecha={fecha.Value}";
                }
                else
                {
                    if (!categoriaId.HasValue || !monto.HasValue || !fecha.HasValue)
                    {
                        mensaje = "Debe ingresar categoría, monto y fecha.";
                        return Json(new { exito = false, mensaje });
                    }

                    exito = _bonoModel.AgregarSimple(
                        idEmpleado.Value,
                        categoriaId.Value,
                        monto.Value,
                        fecha.Value);
                    mensaje = exito ? "Bonificación agregada correctamente." : "Error al agregar bonificación.";
                }
            }

            return Json(new { exito, mensaje });
        }


            /// <summary>
            /// Elimina una bonificación dada (desde AJAX).
            /// </summary>
            [HttpPost]
        public JsonResult EliminarBonificacion(int idBonificacion)
        {
            bool exito = _bonoModel.Eliminar(idBonificacion);
            string mensaje = exito ? "Bonificación eliminada." : "No se pudo eliminar.";
            return Json(new { exito, mensaje });
        }

        #region Métodos para gestión de categorías

        /// <summary>
        /// Retorna todas las categorías (JSON).
        /// </summary>
        [HttpGet]
        public JsonResult ObtenerCategorias()
        {
            var lista = _categoriaModel.ObtenerTodas();
            return Json(lista);
        }

        /// <summary>
        /// Agrega una categoría nueva.
        /// </summary>
        [HttpPost]
        public JsonResult AgregarCategoria(string nombre)
        {
            bool exito = _categoriaModel.Agregar(nombre);
            string mensaje = exito ? "Categoría creada con éxito." : "Error al crear categoría.";
            return Json(new { exito, mensaje });
        }

        /// <summary>
        /// Edita una categoría existente.
        /// </summary>
        [HttpPost]
        public JsonResult EditarCategoria(int id, string nombre)
        {
            bool exito = _categoriaModel.Editar(id, nombre);
            string mensaje = exito ? "Categoría actualizada con éxito." : "Error al actualizar categoría (quizá hay bonos asociados).";
            return Json(new { exito, mensaje });
        }

        /// <summary>
        /// Elimina una categoría si no tiene bonos asociados.
        /// </summary>
        [HttpPost]
        public JsonResult EliminarCategoria(int id)
        {
            bool exito = _categoriaModel.Eliminar(id);
            string mensaje = exito ? "Categoría eliminada con éxito." : "No se pudo eliminar (quizá tiene bonificaciones asociadas).";
            return Json(new { exito, mensaje });
        }

        #endregion
    }
}
