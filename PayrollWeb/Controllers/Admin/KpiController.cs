﻿using Microsoft.AspNetCore.Mvc;
using PayrollWeb.Models;

namespace PayrollWeb.Controllers.Admin
{
    public class KpiController : Controller
    {
        KPI _kpi = new KPI();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerAgregarKpi()
        {
            List<KPI> listaKPI = _kpi.ObtenerKPI();
            return View("/Views/Admin/AgregarKpi.cshtml", listaKPI);
        }

        //Métodos para controlar la lógica
        public IActionResult CrearKpi(string Nombre)
        {
            KPI kpi = new KPI(Nombre);
            if (!kpi.AgregarKPI())
            {
                TempData["Error"] = "Error al agregar el KPI";
                return RedirectToAction("VerAgregarKpi");
            }
            TempData["Success"] = "KPI agregado correctamente";
            return RedirectToAction("VerAgregarKpi");
        }

        public IActionResult ActualizarKpi(int id,  string Nombre)
        {
            KPI kpi = new KPI(id, Nombre);
            if (!kpi.ActualizarKPI())
            {
                TempData["Error"] = "Error al actualizar el KPI";
                return RedirectToAction("VerAgregarKpi");
            }
            TempData["Success"] = "KPI actualizado correctamente";
            return RedirectToAction("VerAgregarKpi");
        }
    }
}
