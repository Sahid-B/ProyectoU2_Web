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
    public class EmpleadosController : Controller
    {
        private readonly AppDbContext _context;

        public EmpleadosController(AppDbContext context)
        {
            _context = context;
        }

        private bool SesionActiva()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario"));
        }

        // GET: Empleados
        public async Task<IActionResult> Index(int pagina = 1, string buscar = "")
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");

            int tamañoPagina = 20;

            var consulta = _context.Empleados
                .AsNoTracking()
                .Where(e => e.Activo);

            if (!string.IsNullOrEmpty(buscar))
                consulta = consulta.Where(e => e.Nombre.Contains(buscar) || e.Apellido.Contains(buscar) || e.Cedula.Contains(buscar));

            consulta = consulta.OrderBy(e => e.Apellido).ThenBy(e => e.Nombre);

            int totalRegistros = await consulta.CountAsync();

            var empleados = await consulta
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
            return View(empleados);
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index");
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Cedula,Cargo,Telefono,Salario,Activo,FechaCreacion,FechaActualizacion,FechaEliminacion")] Empleado empleado)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index");
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Cedula,Cargo,Telefono,Salario,Activo,FechaCreacion,FechaActualizacion,FechaEliminacion")] Empleado empleado)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
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
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("Rol") != "Admin") return RedirectToAction("Index");
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Auth");
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                empleado.Activo = false;
                empleado.FechaEliminacion = DateTime.UtcNow;
                _context.Update(empleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}


