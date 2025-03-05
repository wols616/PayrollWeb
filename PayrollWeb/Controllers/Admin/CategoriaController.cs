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

        [Authorize]
        public IActionResult VerAgregarCategoria()
        {
            return View("/Views/Admin/AgregarCategoria.cshtml");
        }

        [Authorize]
        public IActionResult VerEditarCategoria(int id)
        {
            return View("/Views/Admin/EditarCategoria.cshtml", _categoria.ObtenerCategoria(id));
        }

        //__________________________________________________________________________________________________________________________________
        //CONTROLADORES PARA CONTROLAR LA LÓGICA
        public IActionResult CrearCategoria(Categoria categoria)
        {
            if (categoria.ExisteCategoria())
            {
                ModelState.AddModelError("NombreCategoria", "La categoría ya está registrada");
            }

            if (!ModelState.IsValid)
            {
                return View("/Views/Admin/AgregarCategoria.cshtml", categoria);
            }
            categoria.AgregarCategoria();
            return RedirectToAction("VerCategorias");
        }

        public IActionResult ActualizarCategoria(Categoria categoria)
        {
            if (categoria.ExisteCategoria())
            {
                ModelState.AddModelError("NombreCategoria", "La categoría ya está registrada");
            }
            if (!ModelState.IsValid) { 
                return View("/Views/Admin/EditarCategoria.cshtml", categoria); 
            }
            categoria.ActualizarCategoria();
            return RedirectToAction("VerCategorias");
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
