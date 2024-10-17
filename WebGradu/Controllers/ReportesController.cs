using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGradu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGradu.Data;

namespace WebGradu.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reportes/ProductosMasVendidos
        public async Task<IActionResult> Index()
        {
            var productosVendidos = await _context.DetalleVentas
                .GroupBy(d => d.Fk_Producto)
                .Select(g => new ProductoMasVendidoViewModel
                {
                    CodigoProducto = _context.Productos.Where(p => p.ProductoID == g.Key).Select(p => p.Codigo_Producto).FirstOrDefault(),
                    NombreProducto = _context.Productos.Where(p => p.ProductoID == g.Key).Select(p => p.Nombre).FirstOrDefault(), // Asumiendo que tienes una propiedad Nombre en Producto
                    TotalVendidos = g.Sum(d => d.Cantidad)
                })
                .ToListAsync();

            return View(productosVendidos);
        }

    }
}

