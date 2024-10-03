using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebGradu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebGradu.Data;


namespace Productos.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ventas/Create
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Venta venta, string detalles)
        {
            if (ModelState.IsValid)
            {
                // Asigna el UsuarioId desde el usuario autenticado
                venta.UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                venta.Fecha = DateTime.Now;

                // Guardar la venta
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync(); // Guardar la venta primero

                // Deserializar los detalles de la venta
                var listaDetalles = JsonConvert.DeserializeObject<List<DetalleVenta>>(detalles);
                foreach (var detalle in listaDetalles)
                {
                    detalle.VentaId = venta.Id; // Asignar el Id de la venta al detalle
                    _context.DetalleVentas.Add(detalle); // Agregar detalle a la venta

                    // Obtener el producto en base al código del producto
                    var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Codigo_Producto == detalle.CodigoProducto);

                    if (producto != null)
                    {
                        // Obtener el último registro de stock actual para el producto
                        var stockAnterior = await _context.Stocks
                            .AsNoTracking() // Evita que el stock anterior sea rastreado y modificado
                            .Where(s => s.Fk_Producto == producto.ProductoID)
                            .OrderByDescending(s => s.FechaMovimiento)
                            .FirstOrDefaultAsync();

                        if (stockAnterior != null)
                        {
                            // Crear un nuevo registro de movimiento en la tabla de Stock
                            var nuevoStockMovimiento = new Stock
                            {
                                Fk_Producto = producto.ProductoID, // Relacionar con el producto
                                StockInicial = stockAnterior.StockActual, // Asignar el stock actual del último movimiento como inicial
                                StockActual = stockAnterior.StockActual - detalle.Cantidad, // Disminuir la cantidad vendida
                                StockMinimo = stockAnterior.StockMinimo, // Mantener el stock mínimo
                                StockMaximo = stockAnterior.StockMaximo, // Mantener el stock máximo
                                TipoMovimiento = "Venta", // Registrar como movimiento de venta
                                FechaMovimiento = DateTime.Now, // Asignar la fecha del movimiento
                            };

                            // Agregar el nuevo movimiento de stock a la base de datos
                            _context.Stocks.Add(nuevoStockMovimiento);
                        }
                        else
                        {
                            // Si no hay stock asociado, mostrar un error
                            TempData["Error"] = "No se encontró registro de stock para el producto: " + producto.Nombre;
                            return View(venta); // Retornar a la vista de creación con el mensaje de error
                        }
                    }
                    else
                    {
                        // Si no se encontró el producto, también se puede manejar un error
                        TempData["Error"] = "No se encontró el producto con código: " + detalle.CodigoProducto;
                        return View(venta); // Retornar a la vista de creación con el mensaje de error
                    }
                }

                // Guardar los cambios en los detalles y en el nuevo registro de stock
                await _context.SaveChangesAsync(); // Asegúrate de guardar todos los cambios

                TempData["VentaCreada"] = "La venta se ha registrado exitosamente.";
                return RedirectToAction("Crear");
            }
            return View(venta);
        }



        // Método para obtener el precio por código de producto
        [HttpGet]
        public JsonResult GetPrecioPorCodigo(string codigo)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.Codigo_Producto == codigo);
            if (producto != null)
            {
                return Json(new { precio = producto.Precio });
            }
            return Json(new { precio = 0 });
        }
    }
}
