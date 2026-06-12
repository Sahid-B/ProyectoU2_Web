using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniMarketApp.Data;
using MiniMarketApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MiniMarketApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        private bool SesionActiva()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario"));
        }

        public async Task<IActionResult> Index()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");
            return View();
        }

        public async Task<IActionResult> TotalVentas()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var ventasActivas = _context.Ventas.Where(v => v.Activo);
            var count = await ventasActivas.CountAsync();
            
            var model = new TotalVentasDto
            {
                NumeroVentas = count,
                TotalVentas = count > 0 ? await ventasActivas.SumAsync(v => v.Total) : 0,
                PromedioPorVenta = count > 0 ? await ventasActivas.AverageAsync(v => v.Total) : 0
            };

            return View(model);
        }

        public async Task<IActionResult> VentasPorMes()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var model = await _context.Ventas
                .Where(v => v.Activo)
                .GroupBy(v => new { v.FechaVenta.Year, v.FechaVenta.Month })
                .Select(g => new VentasPorMesDto
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    Total = g.Sum(v => v.Total),
                    Cantidad = g.Count()
                })
                .OrderByDescending(g => g.Anio).ThenByDescending(g => g.Mes)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> TopProductos()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var model = await _context.DetallesVenta
                .Include(d => d.Producto)
                .Where(d => d.Venta != null && d.Venta.Activo)
                .GroupBy(d => d.Producto!.Nombre)
                .Select(g => new TopProductoDto
                {
                    Producto = g.Key,
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(g => g.CantidadVendida)
                .Take(10)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> ClientesMasCompras()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var model = await _context.Ventas
                .Include(v => v.Cliente)
                .Where(v => v.Activo)
                .GroupBy(v => v.Cliente!.Nombre + " " + v.Cliente.Apellido)
                .Select(g => new ClienteMasComprasDto
                {
                    Cliente = g.Key,
                    TotalComprado = g.Sum(v => v.Total)
                })
                .OrderByDescending(g => g.TotalComprado)
                .Take(10)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> ProductosBajoStock(int pagina = 1)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            int tamañoPagina = 20;

            var model = await _context.Productos
                .Where(p => p.Stock < 10 && p.Activo)
                .OrderBy(p => p.Stock)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .Select(p => new ProductoStockDto
                {
                    Nombre = p.Nombre,
                    Stock = p.Stock
                })
                .ToListAsync();

            ViewBag.Pagina = pagina;

            return View(model);
        }

        public async Task<IActionResult> VentasPorEstado()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var model = await _context.Ventas
                .Where(v => v.Activo)
                .GroupBy(v => v.Estado)
                .Select(g => new VentasPorEstadoDto
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    Total = g.Sum(v => v.Total)
                })
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> ProveedoresMasProductos()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var model = await _context.Productos
                .Include(p => p.Proveedor)
                .Where(p => p.Activo && p.Proveedor != null)
                .GroupBy(p => p.Proveedor!.Nombre)
                .Select(g => new ProveedorMasProductosDto
                {
                    Proveedor = g.Key,
                    CantidadProductos = g.Count()
                })
                .OrderByDescending(g => g.CantidadProductos)
                .Take(10)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> CategoriasMasVendidas()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var model = await _context.DetallesVenta
                .Include(d => d.Producto)
                .ThenInclude(p => p!.Categoria)
                .Where(d => d.Venta != null && d.Venta.Activo && d.Producto != null && d.Producto.Categoria != null)
                .GroupBy(d => d.Producto!.Categoria!.Nombre)
                .Select(g => new CategoriaMasVendidaDto
                {
                    Categoria = g.Key,
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(g => g.CantidadVendida)
                .Take(10)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> PromedioVentasPorCliente()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var model = await _context.Ventas
                .Include(v => v.Cliente)
                .Where(v => v.Activo)
                .GroupBy(v => v.Cliente!.Nombre + " " + v.Cliente.Apellido)
                .Select(g => new PromedioVentaClienteDto
                {
                    Cliente = g.Key,
                    PromedioVentas = g.Average(v => v.Total)
                })
                .OrderByDescending(g => g.PromedioVentas)
                .Take(20)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> ResumenGeneral()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth"); if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index", "Home");

            var unMesAtras = DateTime.UtcNow.AddMonths(-1);
            var ventasUltimoMes = _context.Ventas.Where(v => v.FechaVenta >= unMesAtras && v.Activo);

            var model = new ResumenGeneralDto
            {
                ClientesActivos = await _context.Clientes.CountAsync(c => c.Activo),
                ClientesInactivos = await _context.Clientes.CountAsync(c => !c.Activo),
                ProductosActivos = await _context.Productos.CountAsync(p => p.Activo),
                ProductosInactivos = await _context.Productos.CountAsync(p => !p.Activo),
                VentasActivas = await _context.Ventas.CountAsync(v => v.Activo),
                VentasInactivas = await _context.Ventas.CountAsync(v => !v.Activo),
                TotalVentasUltimoMes = await ventasUltimoMes.CountAsync(),
                TotalIngresosUltimoMes = await ventasUltimoMes.SumAsync(v => v.Total)
            };

            return View(model);
        }
    }
}

