using Microsoft.AspNetCore.Mvc;
using WebGradu.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebGradu.Data;

namespace WebGradu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Cambia a tu contexto de base de datos

        public HomeController(ApplicationDbContext context) // Aseg�rate de que ApplicationDbContext sea tu contexto
        {
            _context = context; // Inicializa el contexto
        }


        public IActionResult Index()
        {
            // Verificar si el usuario est� autenticado
            if (User.Identity.IsAuthenticated)
            {
                // Obtener el �ltimo movimiento para cada producto
                var ultimosMovimientos = _context.Stocks
                    .GroupBy(s => s.Fk_Producto) // Agrupar por el identificador del producto
                    .Select(g => g.OrderByDescending(s => s.FechaMovimiento).FirstOrDefault()) // Obtener el �ltimo movimiento de cada producto
                    .ToList(); // Ejecutar la consulta y obtener los resultados

                // Filtrar productos que tienen stock actual menor o igual que el stock m�nimo
                var productosConStockBajo = ultimosMovimientos
                    .Where(s => s.StockActual <= s.StockMinimo && s.StockActual > 0) // Filtrar productos con stock bajo
                    .ToList(); // Materializar la lista de productos con stock bajo

                // Si hay productos con stock bajo, establece un mensaje en TempData
                if (productosConStockBajo.Any())
                {
                    TempData["StockBajo"] = "Atenci�n: Hay productos con bajo stock.";
                }

                // Pasar los productos finales a la vista
                return View(productosConStockBajo);
            }

            // Si el usuario no est� autenticado, redirigir a la vista Index del controlador Home
            return RedirectToAction("Inicio", "Home");

        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Inicio()
        {
            return View();
        }

    }
}
