using GestionTareas.API.Consumer;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTareas.MVC.Controllers
{
    [Authorize]
    public class SeguimientosController : Controller
    {
        // GET: SeguimientoController
        public ActionResult Index()
        {
            try
            {
                SetTokenForRequest();

                var seguimientos = Crud<Seguimiento>.GetAll();
                var tareas = Crud<Tarea>.GetAll();
                var usuarios = Crud<Usuario>.GetAll();

                var seguimientosEnriquecidos = seguimientos.Select(s => new
                {
                    s.id,
                    s.fecha,
                    s.estadoAnterior,
                    s.estadoNuevo,
                    s.tareaId,
                    s.usuarioId,
                    TareaTitulo = tareas.FirstOrDefault(t => t.id == s.tareaId)?.titulo ?? "Tarea no encontrada",
                    UsuarioNombre = usuarios.FirstOrDefault(u => u.id == s.usuarioId)?.nombre ?? "Usuario no encontrado"
                })
                .OrderByDescending(s => s.fecha)
                .ToList();

                ViewBag.SeguimientosData = seguimientosEnriquecidos;
                return View(seguimientos);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // GET: SeguimientoController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                SetTokenForRequest();
                var seguimiento = Crud<Seguimiento>.GetById(id);
                if (seguimiento == null)
                {
                    return NotFound();
                }

                var tarea = Crud<Tarea>.GetById(seguimiento.tareaId);
                var usuario = Crud<Usuario>.GetById(seguimiento.usuarioId);

                ViewBag.TareaTitulo = tarea?.titulo ?? "Tarea no encontrada";
                ViewBag.UsuarioNombre = usuario?.nombre ?? "Usuario no encontrado";

                return View(seguimiento);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // GET: SeguimientoController/Create
        public ActionResult Create()
        {
            try
            {
                SetTokenForRequest();
                CargarDatosSeguimiento();
                return View();
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // POST: SeguimientoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Seguimiento seguimiento)
        {
            try
            {
                SetTokenForRequest();
                seguimiento.fecha = DateTime.Now;
                Crud<Seguimiento>.Create(seguimiento);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                CargarDatosSeguimiento();
                ModelState.AddModelError("", "Error al crear el seguimiento: " + ex.Message);
                return View(seguimiento);
            }
        }

        // GET: SeguimientoController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                SetTokenForRequest();
                var seguimiento = Crud<Seguimiento>.GetById(id);
                if (seguimiento == null)
                {
                    return NotFound();
                }
                CargarDatosSeguimiento();
                return View(seguimiento);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // POST: SeguimientoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Seguimiento seguimiento)
        {
            try
            {
                SetTokenForRequest();
                Crud<Seguimiento>.Update(id, seguimiento);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                CargarDatosSeguimiento();
                ModelState.AddModelError("", "Error al editar el seguimiento: " + ex.Message);
                return View(seguimiento);
            }
        }

        // GET: SeguimientoController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                SetTokenForRequest();
                var seguimiento = Crud<Seguimiento>.GetById(id);
                if (seguimiento == null)
                {
                    return NotFound();
                }

                var tarea = Crud<Tarea>.GetById(seguimiento.tareaId);
                var usuario = Crud<Usuario>.GetById(seguimiento.usuarioId);

                ViewBag.TareaTitulo = tarea?.titulo ?? "Tarea no encontrada";
                ViewBag.UsuarioNombre = usuario?.nombre ?? "Usuario no encontrado";

                return View(seguimiento);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // POST: SeguimientoController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                SetTokenForRequest();
                Crud<Seguimiento>.Delete(id);
                TempData["SuccessMessage"] = "El seguimiento se eliminó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el seguimiento: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private void CargarDatosSeguimiento()
        {
            var tareas = Crud<Tarea>.GetAll();
            var usuarios = Crud<Usuario>.GetAll();

            ViewBag.Tareas = tareas.Select(t => new SelectListItem
            {
                Value = t.id.ToString(),
                Text = t.titulo
            }).ToList();

            ViewBag.Usuarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.id.ToString(),
                Text = u.nombre
            }).ToList();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En Progreso", Text = "En Progreso" },
                new SelectListItem { Value = "Completada", Text = "Completada" }
            };
        }

        private void SetTokenForRequest()
        {
            var token = User.FindFirst("JWTToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                Crud<Seguimiento>.Token = token;
                Crud<Tarea>.Token = token;
                Crud<Usuario>.Token = token;
            }
        }
    }
}
