using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class CargoController : Controller
    {
        Empleado _empleado = new Empleado();
        Cargo _cargo = new Cargo();
        Complemento_Cargo _complementoCargo = new Complemento_Cargo();
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

        public IActionResult VerEmpleados()
        {
            List<Empleado> empleados = _empleado.ObtenerEmpleados();
            ViewBag.ShowActions = "Cargos";
            return View("/Views/Admin/VerEmpleados.cshtml", empleados);
        }

        public IActionResult VerCargosEmpleado(int IdEmpleado)
        {
            ViewBag.Empleado = _empleado.ObtenerEmpleado(IdEmpleado);
            ViewBag.Cargos = _cargo.ObtenerCargos();
            List<Complemento_Cargo> complementoCargo = _complementoCargo.ObtenerComplementosCargos(IdEmpleado);
            return View("/Views/Admin/VerComplementoCargo.cshtml", complementoCargo);
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

        public IActionResult CrearComplementoCargo(int IdCargo, int IdEmpleado, DateTime FechaInicio, DateTime FechaFin, decimal MontoComplemento)
        {
            Complemento_Cargo complementoCargo = new Complemento_Cargo
            {
                IdCargo = IdCargo,
                IdEmpleado = IdEmpleado,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin,
                MontoComplemento = MontoComplemento
            };

            if (complementoCargo.AgregarComplementoCargo())
            {
                TempData["Success"] = "El complemento de cargo se ha agregado correctamente";
                return RedirectToAction("VerCargosEmpleado", new { IdEmpleado = IdEmpleado });
            }
            TempData["Error"] = "Error al agregar el complemento de cargo";
            return RedirectToAction("VerCargosEmpleado", new { IdEmpleado = IdEmpleado });
        }

        public IActionResult EliminarComplementoCargo(int IdComplementoCargo, int IdEmpleado)
        {
            if (_complementoCargo.EliminarComplementoCargo(IdComplementoCargo))
            {
                TempData["Success"] = "El complemento de cargo se ha eliminado";
                return RedirectToAction("VerCargosEmpleado", new { IdEmpleado = IdEmpleado });
            }
            TempData["Error"] = "Error al eliminar el complemento de cargo";
            return RedirectToAction("VerCargosEmpleado", new { IdEmpleado = IdEmpleado });
        }

    }
}
