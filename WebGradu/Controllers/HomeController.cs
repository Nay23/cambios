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

        public HomeController(ApplicationDbContext context) // Asegúrate de que ApplicationDbContext sea tu contexto
        {
            _context = context; // Inicializa el contexto
        }

        [Authorize] 
        public IActionResult Index()
        {
            // Verificar si el usuario está autenticado
            if (User.Identity.IsAuthenticated)
            {
                // Obtener todos los productos con su stock actual
                var productosConStockMinimo = _context.Stocks
                    .Include(s => s.Producto)
                    .Where(s => s.StockActual <= s.StockMinimo && s.StockActual > 0)
                    .ToList();

                // Filtrar los productos que no tienen reabastecimientos posteriores
                var productosFinales = productosConStockMinimo
                    .Where(stock => !_context.Stocks
                        .Where(s => s.Fk_Producto == stock.Fk_Producto)
                        .Any(s => s.TipoMovimiento == "Reabastecimiento" && s.StockActual > stock.StockActual))
                    .ToList();

                // Si hay productos con stock bajo, establece un mensaje en TempData
                if (productosFinales.Any())
                {
                    TempData["StockBajo"] = "Atención: Hay productos con bajo stock.";
                }

                // Pasar los productos finales a la vista
                return View(productosFinales);
            }

            // Si el usuario no está autenticado, redirigir a la vista Index del controlador Home
            return RedirectToAction("Index", "Home");
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
