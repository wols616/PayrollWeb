using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class CargoController : Controller
    {
        Cargo _cargo = new Cargo();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult VerCargosSelect()
        {
            return View("/Views/Admin/CargoComplementoSelect.cshtml");
        }
        public IActionResult VerCargos()
        {
            List<Cargo> cargos = _cargo.ObtenerCargos();
            return View("/Views/Admin/VerCargos.cshtml", cargos);
        }

        //_____________________________________________________________________________________________________________________________________
        public IActionResult CrearCargo(string NombreCargo, string Descripcion)
        {
            Cargo cargo = new Cargo { 
                NombreCargo = NombreCargo,
                Descripcion = Descripcion
            };

            if (cargo.CargoExiste())
            {
                TempData["Error"] = "El cargo ya existe";
                return RedirectToAction("VerCargos");
            }

            if (cargo.AgregarCargo())
            {
                TempData["Success"] = "El cargo se ha agregado correctamente";
                return RedirectToAction("VerCargos");
            }

            TempData["Error"] = "Error al agregar el cargo";
            return RedirectToAction("VerCargos");
        }

        public IActionResult ActualizarCargo(int IdCargo, string NombreCargo, string Descripcion)
        {
            Cargo cargo = new Cargo
            {
                IdCargo = IdCargo,
                NombreCargo = NombreCargo,
                Descripcion = Descripcion
            };

            if (!cargo.ActualizarCargo())
            {
                TempData["Error"] = "El nombre del cargo ya existe.";
                return RedirectToAction("VerCargos");
            }

            TempData["Success"] = "El cargo se ha actualizado correctamente.";
            return RedirectToAction("VerCargos");
        }

        public IActionResult EliminarCargo(int IdCargo)
        {
            if (_cargo.EliminarCargo(IdCargo))
            {
                TempData["Success"] = "El cargo se ha eliminado correctamente.";
                return RedirectToAction("VerCargos");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el cargo. El cargo ya está asociado a un complemento";
                return RedirectToAction("VerCargos");
            }
        }

    }
}
