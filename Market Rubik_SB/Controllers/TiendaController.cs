using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMarketApp.Data;
using MiniMarketApp.Models;
using System.Text.Json;

namespace MiniMarketApp.Controllers
{
    public class TiendaController : Controller
    {
        private readonly AppDbContext _context;

        public TiendaController(AppDbContext context)
        {
            _context = context;
        }

        private bool SesionActiva()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario"));
        }

        // Obtener el carrito actual desde la sesión
        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
                return new List<CarritoItem>();
            return JsonSerializer.Deserialize<List<CarritoItem>>(carritoJson) ?? new List<CarritoItem>();
        }

        // Guardar el carrito en la sesión
        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            var carritoJson = JsonSerializer.Serialize(carrito);
            HttpContext.Session.SetString("Carrito", carritoJson);
        }

        // GET: Tienda
        public async Task<IActionResult> Index()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");

            // Para evitar duplicados en el catálogo (agrupamos por nombre)
            var productos = await _context.Productos
                .Where(p => p.Activo && p.Stock > 0)
                .OrderBy(p => p.Id)
                .ToListAsync();

            var catalogoUnico = productos
                .GroupBy(p => p.Nombre)
                .Select(g => g.First())
                .Take(50) // Mostramos 50 productos para no saturar
                .ToList();

            ViewBag.CarritoItems = ObtenerCarrito().Sum(c => c.Cantidad);
            return View(catalogoUnico);
        }

        // POST: Tienda/AgregarAlCarrito
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int productoId)
        {
            if (!SesionActiva()) return Json(new { success = false, message = "Debe iniciar sesión" });

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null || !producto.Activo || producto.Stock <= 0)
                return Json(new { success = false, message = "Producto no disponible" });

            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.ProductoId == productoId);

            if (item != null)
            {
                if (item.Cantidad < producto.Stock)
                    item.Cantidad++;
                else
                    return Json(new { success = false, message = "Stock máximo alcanzado" });
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = 1
                });
            }

            GuardarCarrito(carrito);
            return Json(new { success = true, totalItems = carrito.Sum(c => c.Cantidad) });
        }

        // GET: Tienda/Carrito
        public IActionResult Carrito()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");

            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        // POST: Tienda/QuitarDelCarrito
        [HttpPost]
        public IActionResult QuitarDelCarrito(int productoId)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.ProductoId == productoId);
            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarrito(carrito);
            }
            return RedirectToAction(nameof(Carrito));
        }

        // POST: Tienda/Pagar
        [HttpPost]
        public async Task<IActionResult> Pagar()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");

            var carrito = ObtenerCarrito();
            if (!carrito.Any()) return RedirectToAction(nameof(Index));

            var nombreCompleto = HttpContext.Session.GetString("NombreCompleto") ?? "Cliente Anónimo";
            var nombres = nombreCompleto.Split(' ');
            var nombre = nombres.Length > 0 ? nombres[0] : nombreCompleto;
            var apellido = nombres.Length > 1 ? string.Join(" ", nombres.Skip(1)) : "---";

            // Buscar si ya existe este cliente
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Nombre == nombre && c.Apellido == apellido);
            if (cliente == null)
            {
                cliente = new Cliente { Nombre = nombre, Apellido = apellido, Activo = true };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }

            decimal subtotal = carrito.Sum(c => c.Subtotal);
            decimal ivaCalculado = subtotal * 0.15m; // IVA del 15% en Ecuador
            decimal totalFinal = subtotal + ivaCalculado;

            // Crear la Venta
            var venta = new Venta
            {
                NumeroVenta = "V-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                ClienteId = cliente.Id,
                FechaVenta = DateTime.UtcNow,
                Subtotal = subtotal,
                Iva = ivaCalculado,
                Total = totalFinal,
                Estado = "Pendiente",
                Activo = true
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync(); // Para obtener el VentaId

            // Crear Detalles
            foreach (var item in carrito)
            {
                var detalle = new DetalleVenta
                {
                    VentaId = venta.Id,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio
                };
                _context.DetallesVenta.Add(detalle);

                // Opcional: Descontar stock
                var productoDb = await _context.Productos.FindAsync(item.ProductoId);
                if (productoDb != null)
                {
                    productoDb.Stock -= item.Cantidad;
                }
            }

            await _context.SaveChangesAsync();

            // Limpiar carrito
            HttpContext.Session.Remove("Carrito");

            return RedirectToAction(nameof(Exito), new { id = venta.Id });
        }

        // GET: Tienda/Exito
        public IActionResult Exito(int id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            ViewBag.VentaId = id;
            return View();
        }
    }
}
