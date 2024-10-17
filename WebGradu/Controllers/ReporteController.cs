using Microsoft.AspNetCore.Mvc;
using WebGradu.Models;
using System.Linq;
using System;
using WebGradu.Data;

namespace WebGradu.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Muestra el formulario para generar el reporte
        public IActionResult Index()
        {
            return View();
        }

        // Procesa la solicitud del formulario y genera el reporte
        [HttpPost]
        public IActionResult GenerarReporte(DateTime fechaInicio, DateTime fechaFin)
        {
            // Validación de fechas
            if (fechaInicio == DateTime.MinValue || fechaFin == DateTime.MinValue || fechaFin < fechaInicio)
            {
                ModelState.AddModelError(string.Empty, "Por favor selecciona un rango de fechas válido.");
                return View("Index");  // Regresa al formulario en caso de error
            }

            // Filtra las ventas entre las fechas proporcionadas
            var ventas = _context.Ventas
                .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin)
                .ToList();

            if (ventas.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No se encontraron ventas en este rango de fechas.");
                return View("Index"); // Regresa al formulario si no hay ventas
            }

            // Pasa los datos de las ventas a la vista del reporte
            return View("Reporte", ventas);
        }
    }
}
