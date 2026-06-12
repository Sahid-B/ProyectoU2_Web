using MiniMarketApp.Data;
using MiniMarketApp.Models;

namespace MiniMarketApp.Services
{
    public class MassiveSeederService
    {
        private readonly AppDbContext _context;

        public MassiveSeederService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.Categorias.Count() > 50) return;

            var random = new Random();

            // ─── 100 Categorías ───────────────────────────────
            if (_context.Categorias.Count() < 100)
            {
                var categorias = new List<Categoria>();
                for (int i = 1; i <= 100; i++)
                    categorias.Add(new Categoria
                    {
                        Nombre = $"Categoría {i}",
                        Descripcion = $"Descripción de categoría {i}",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                await _context.Categorias.AddRangeAsync(categorias);
                await _context.SaveChangesAsync();
            }

            // ─── 1.000 Proveedores ────────────────────────────
            if (_context.Proveedores.Count() < 1000)
            {
                var proveedores = new List<Proveedor>();
                for (int i = 1; i <= 1000; i++)
                    proveedores.Add(new Proveedor
                    {
                        Nombre = $"Proveedor {i}",
                        Contacto = $"Contacto {i}",
                        Telefono = $"09{random.Next(10000000, 99999999)}",
                        Correo = $"proveedor{i}@mail.com",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                await _context.Proveedores.AddRangeAsync(proveedores);
                await _context.SaveChangesAsync();
            }

            // ─── 1.000 Empleados ──────────────────────────────
            if (_context.Empleados.Count() < 1000)
            {
                var empleados = new List<Empleado>();
                for (int i = 1; i <= 1000; i++)
                    empleados.Add(new Empleado
                    {
                        Nombre = $"Empleado {i}",
                        Apellido = $"Apellido {i}",
                        Cedula = $"17{random.Next(10000000, 99999999)}",
                        Cargo = i % 3 == 0 ? "Cajero" : i % 3 == 1 ? "Bodeguero" : "Vendedor",
                        Salario = random.Next(450, 2000),
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                await _context.Empleados.AddRangeAsync(empleados);
                await _context.SaveChangesAsync();
            }

            // ─── 30.000 Productos ─────────────────────────────
            if (_context.Productos.Count() < 1000)
            {
                var catIds = _context.Categorias.Select(c => c.Id).ToList();
                var provIds = _context.Proveedores.Select(p => p.Id).ToList();
                var productos = new List<Producto>();
                for (int i = 1; i <= 30000; i++)
                {
                    productos.Add(new Producto
                    {
                        Nombre = $"Producto {i}",
                        Descripcion = $"Descripción producto {i}",
                        Precio = Math.Round((decimal)(random.NextDouble() * 100 + 0.5), 2),
                        Stock = random.Next(0, 500),
                        CategoriaId = catIds[random.Next(catIds.Count)],
                        ProveedorId = provIds[random.Next(provIds.Count)],
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                    if (productos.Count % 5000 == 0)
                    {
                        await _context.Productos.AddRangeAsync(productos);
                        await _context.SaveChangesAsync();
                        productos.Clear();
                    }
                }
                if (productos.Any())
                {
                    await _context.Productos.AddRangeAsync(productos);
                    await _context.SaveChangesAsync();
                }
            }

            // ─── 80.000 Clientes ──────────────────────────────
            if (_context.Clientes.Count() < 1000)
            {
                var clientes = new List<Cliente>();
                for (int i = 1; i <= 80000; i++)
                {
                    clientes.Add(new Cliente
                    {
                        Nombre = $"Cliente {i}",
                        Apellido = $"Apellido {i}",
                        Cedula = $"17{random.Next(10000000, 99999999)}",
                        Telefono = $"09{random.Next(10000000, 99999999)}",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                    if (clientes.Count % 10000 == 0)
                    {
                        await _context.Clientes.AddRangeAsync(clientes);
                        await _context.SaveChangesAsync();
                        clientes.Clear();
                    }
                }
                if (clientes.Any())
                {
                    await _context.Clientes.AddRangeAsync(clientes);
                    await _context.SaveChangesAsync();
                }
            }

            // ─── 150.000 Ventas ───────────────────────────────
            if (_context.Ventas.Count() < 100)
            {
                var clienteIds = _context.Clientes.Select(c => c.Id).ToList();
                var ventas = new List<Venta>();
                for (int i = 1; i <= 150000; i++)
                {
                    ventas.Add(new Venta
                    {
                        NumeroVenta = $"V-{i:D6}",
                        ClienteId = clienteIds[random.Next(clienteIds.Count)],
                        FechaVenta = DateTime.UtcNow.AddDays(-random.Next(0, 365)),
                        Subtotal = Math.Round((decimal)(random.NextDouble() * 200 + 1), 2),
                        Iva = 0,
                        Total = 0,
                        Estado = i % 5 == 0 ? "Pendiente" : "Completada",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                    if (ventas.Count % 10000 == 0)
                    {
                        await _context.Ventas.AddRangeAsync(ventas);
                        await _context.SaveChangesAsync();
                        ventas.Clear();
                    }
                }
                if (ventas.Any())
                {
                    await _context.Ventas.AddRangeAsync(ventas);
                    await _context.SaveChangesAsync();
                }
            }

            // ─── 237.900 Detalles de Venta ────────────────────
            if (_context.DetallesVenta.Count() < 100)
            {
                var ventaIds = _context.Ventas.Select(v => v.Id).ToList();
                var productoIds = _context.Productos.Select(p => p.Id).ToList();
                var detalles = new List<DetalleVenta>();
                for (int i = 1; i <= 237900; i++)
                {
                    detalles.Add(new DetalleVenta
                    {
                        VentaId = ventaIds[random.Next(ventaIds.Count)],
                        ProductoId = productoIds[random.Next(productoIds.Count)],
                        Cantidad = random.Next(1, 20),
                        PrecioUnitario = Math.Round((decimal)(random.NextDouble() * 100 + 0.5), 2),
                        FechaCreacion = DateTime.UtcNow
                    });
                    if (detalles.Count % 10000 == 0)
                    {
                        await _context.DetallesVenta.AddRangeAsync(detalles);
                        await _context.SaveChangesAsync();
                        detalles.Clear();
                    }
                }
                if (detalles.Any())
                {
                    await _context.DetallesVenta.AddRangeAsync(detalles);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
