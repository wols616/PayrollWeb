using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;
using System.Diagnostics;

namespace PayrollWeb.Controllers.Admin
{
    public class PuestoController : Controller
    {
        Puesto _puesto = new Puesto();
        Categoria _categoria = new Categoria();
        
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        //CONTROLADORES PARA ABRIR LAS VISTAS
        [Authorize]
        public IActionResult VerPuestos()
        {
            return View("/Views/Admin/VerPuestos.cshtml", _puesto.MostrarPuestosConCategoria());
        }

        [Authorize]
        public IActionResult VerAgregarPuesto()
        {
            ViewBag.Categorias = _categoria.ObtenerCategorias();
            return View("/Views/Admin/AgregarPuesto.cshtml");
        }

        [Authorize]
        public IActionResult VerEditarPuesto(int id)
        {
            Puesto puesto = _puesto.ObtenerPuesto(id);
            ViewBag.Categorias = _categoria.ObtenerCategorias();
            return View("/Views/Admin/EditarPuesto.cshtml", puesto);
        }


        //__________________________________________________________________________________________________________________________________
        //CONTROLADORES PARA CONTROLAR LA LÓGICA
        public IActionResult CrearPuesto(Puesto puesto)
        {
            if (puesto.ExistePuesto())
            {
                ModelState.AddModelError("ErrorPuestoExistente", "El puesto ya está registrado");
            }

            if (!ModelState.IsValid)
            {
                return View("/Views/Admin/AgregarPuesto.cshtml", puesto);
            }
            puesto.AgregarPuesto();
            return RedirectToAction("VerPuestos");
        }

        public IActionResult ActualizarPuesto(Puesto Puesto)
        {
            if (Puesto.ExistePuesto())
            {
                ModelState.AddModelError("ErrorPuestoExistente", "El puesto ya está registrado");
            }
            if (!ModelState.IsValid)
            {
                return View("/Views/Admin/EditarPuesto.cshtml", Puesto);
            }
            Puesto.EditarPuesto();
            return RedirectToAction("VerPuestos");
        }

        public IActionResult EliminarPuesto(int id)
        {
            bool resultado = _puesto.EliminarPuesto(id);
            if (!resultado)
            {
                TempData["ErrorEliminar"] = "No se puede eliminar el puesto porque ya hay contratos asociados a este";
            }
            return RedirectToAction("VerPuestos");
        }
    }
}
