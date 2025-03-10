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
        Complemento_Puesto _complementoPuesto = new Complemento_Puesto();

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

        [Authorize]

        public IActionResult VerComplementos(int id)
        {
            List<Complemento_Puesto> complementos = _complementoPuesto.ObtenerComplementosPuesto(id);
            ViewBag.Puesto = _puesto.ObtenerPuesto(id);
            return View("/Views/Admin/VerComplementos.cshtml", complementos);
        }

        [Authorize]
        public IActionResult VerAgregarComplementos(int idPuesto)
        {
            ViewBag.Puesto = _puesto.ObtenerPuesto(idPuesto);
            return View("/Views/Admin/AgregarComplemento.cshtml");
        }
        [Authorize]
        public IActionResult VerEditarComplemento(int id)
        {
            Complemento_Puesto complemento = _complementoPuesto.ObtenerComplementoPorId(id);
            return View("/Views/Admin/EditarComplemento.cshtml", complemento);
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

        public IActionResult CrearComplemento(string NombreComplemento, decimal Monto, int IdPuesto)
        {
            Complemento_Puesto complemento_Puesto = new Complemento_Puesto
            {
                NombreComplemento = NombreComplemento,
                Monto = Monto,
                IdPuesto = IdPuesto
            };

            if (!complemento_Puesto.AgregarComplemento())
            {
                TempData["Error"] = "Error al agregar el complemento";
                return RedirectToAction("VerComplementos", new { id = IdPuesto });
            }

            TempData["Success"] = "Complemento agregado correctamente";
            return RedirectToAction("VerComplementos", new { id = IdPuesto });
        }

        public IActionResult ActualizarComplemento(string NombreComplemento, decimal Monto, int IdPuesto, int IdComplementoPuesto)
        {
            Complemento_Puesto complemento = new Complemento_Puesto
            {
                NombreComplemento = NombreComplemento,
                Monto = Monto,
                IdPuesto = IdPuesto,
                IdComplementoPuesto = IdComplementoPuesto
            };

            if (!complemento.ActualizarComplemento())
            {
                TempData["Error"] = "Error al actualizar el complemento";
                return RedirectToAction("VerComplementos", new { id = IdPuesto });

            }
            TempData["Success"] = "Complemento actualizado correctamente";
            return RedirectToAction("VerComplementos", new { id = IdPuesto });
        }

        public IActionResult EliminarComplemento(int id, int IdPuesto)
        {
            if (!_complementoPuesto.EliminarComplemento(id))
            {
                TempData["Error"] = "Error al eliminar el complemento";
            }
            TempData["Success"] = "Complemento eliminado correctamente";
            return RedirectToAction("VerComplementos", new { id = IdPuesto });
        }
    }
}
