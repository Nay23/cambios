using System.ComponentModel.DataAnnotations;

namespace WebGradu.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }
        public int VentaId { get; set; } // Referencia a la venta

        public string? CodigoProducto { get; set; }
        
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }

       
        public virtual Venta Venta { get; set; } // Relación con la tabla Ventas
        //public virtual Producto Producto { get; set; } // Relación con la tabla Productos
    }
}
