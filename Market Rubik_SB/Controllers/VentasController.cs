using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniMarketApp.Data;
using MiniMarketApp.Models;
using MiniMarketApp.ViewModels;
namespace MiniMarketApp.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        private bool SesionActiva()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario"));
        }

        // GET: Ventas
        public async Task<IActionResult> Index(int pagina = 1, string buscar = "")
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");

            int tamañoPagina = 20;

            var consulta = _context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Where(v => v.Activo);

            if (!string.IsNullOrEmpty(buscar))
                consulta = consulta.Where(v => v.NumeroVenta.Contains(buscar) || v.Cliente.Nombre.Contains(buscar) || v.Cliente.Apellido.Contains(buscar));

            consulta = consulta.OrderByDescending(v => v.FechaVenta);

            int totalRegistros = await consulta.CountAsync();

            var ventas = await consulta
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();

            ViewBag.Paginacion = new PaginacionViewModel
            {
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamañoPagina),
                TotalRegistros = totalRegistros,
                TamañoPagina = tamañoPagina
            };

            ViewBag.Buscar = buscar;
            return View(ventas);
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index");
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido");
            return View();
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumeroVenta,ClienteId,FechaVenta,Subtotal,Iva,Total,Estado,Activo,FechaCreacion,FechaActualizacion")] Venta venta)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (ModelState.IsValid)
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", venta.ClienteId);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index");
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", venta.ClienteId);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroVenta,ClienteId,FechaVenta,Subtotal,Iva,Total,Estado,Activo,FechaCreacion,FechaActualizacion")] Venta venta)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id != venta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", venta.ClienteId);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index");
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
                venta.Activo = false;
                venta.FechaEliminacion = DateTime.UtcNow;
                _context.Update(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}


