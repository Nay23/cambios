using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGradu.Models;
using System.Linq;
using WebGradu.Data;
using Microsoft.AspNetCore.Authorization;


namespace WebGradu.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize] // Asegura que solo usuarios autenticados accedan a esta acción
        public IActionResult Index()
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

            // Se ha eliminado la parte de TempData para mostrar alerta

            return View(productosFinales);
        }




    }
}
