using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class CategoriaController : Controller
    {
        Categoria _categoria = new Categoria();
        public IActionResult Index()
        {
            return View();
        }

        //CONTROLADORES PARA ABRIR LAS VISTAS
        [Authorize]
        public IActionResult VerCategoriaPuestoSelect()
        {
            return View("/Views/Admin/CategoriaPuestoSelect.cshtml");
        }

        [Authorize]
        public IActionResult VerCategorias()
        {
            return View("/Views/Admin/VerCategorias.cshtml", _categoria.ObtenerCategorias());
        }

        //[Authorize]
        //public IActionResult VerAgregarCategoria()
        //{
        //    return View("/Views/Admin/AgregarCategoria.cshtml");
        //}

        //[Authorize]
        //public IActionResult VerEditarCategoria(int id)
        //{
        //    return View("/Views/Admin/EditarCategoria.cshtml", _categoria.ObtenerCategoria(id));
        //}

        //__________________________________________________________________________________________________________________________________
        //CONTROLADORES PARA CONTROLAR LA LÓGICA
        [HttpPost]
        public IActionResult CrearCategoria([FromBody] Categoria categoria)
        {
            var errores = new Dictionary<string, string>();

            // Validar nombre
            if (string.IsNullOrWhiteSpace(categoria.NombreCategoria))
                errores.Add("NombreCategoria", "El nombre de la categoría es obligatorio.");

            // Validar sueldo base
            if (categoria.SueldoBase <= 0)
                errores.Add("SueldoBase", "El sueldo base debe ser mayor a 0.");

            // Validar si la categoría ya existe
            if (categoria.ExisteCategoria())
                errores.Add("NombreCategoria", "La categoría ya está registrada.");

            // Si hay errores, devolverlos en JSON
            if (errores.Any())
                return Json(new { success = false, errors = errores });

            // Guardar en la base de datos
            bool guardado = categoria.AgregarCategoria();
            if (guardado)
            {
                return Json(new { success = true, message = "Categoría agregada correctamente." });
            }
            else
            {
                return Json(new { success = false, message = "Error al agregar la categoría." });
            }
        }


        [HttpPost]
        public IActionResult ActualizarCategoria([FromBody] Categoria categoria)
        {
            var errores = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(categoria.NombreCategoria))
                errores.Add("NombreCategoria", "El nombre de la categoría es obligatorio.");

            if (categoria.SueldoBase <= 0)
                errores.Add("SueldoBase", "El sueldo base debe ser mayor a 0.");

            if (errores.Any())
                return Json(new { success = false, errors = errores });

            categoria.ActualizarCategoria();
            return Json(new { success = true, message = "Categoría actualizada correctamente." });
        }

        public IActionResult EliminarCategoria(int id)
        {
            bool resultado = _categoria.EliminarCategoria(id);
            if (!resultado)
            {
                TempData["Error"] = "No se puede eliminar la categoría porque ya hay puestos asociados a esta";
            }
            return RedirectToAction("VerCategorias");
        }

    }
}
