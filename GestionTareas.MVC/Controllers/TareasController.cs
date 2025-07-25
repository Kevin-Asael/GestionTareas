using GestionTareas.API.Consumer;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTareas.MVC.Controllers
{
    [Authorize]
    public class TareasController : Controller
    {
        // GET: TareasController
        public ActionResult Index() 
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                var tareas = Crud<Tarea>.GetAll();
                return View(tareas);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // GET: TareasController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                var tarea = Crud<Tarea>.GetById(id);
                if (tarea == null)
                {
                    return NotFound();
                }

                // Cargar nombres
                Crud<Usuario>.Token = token;
                Crud<Proyecto>.Token = token;
                var usuario = Crud<Usuario>.GetById(tarea.usuarioId);
                var proyecto = Crud<Proyecto>.GetById(tarea.proyectoId);

                ViewBag.NombreUsuario = usuario?.nombre ?? "No asignado";
                ViewBag.NombreProyecto = proyecto?.nombre ?? "No asignado";

                return View(tarea);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // GET: TareasController/Create
        public ActionResult Create()
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                CargarProyectosYUsuarios();
                return View();
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // POST: TareasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tarea tarea)
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                tarea.fechaCreacion = DateTime.Now;
                Crud<Tarea>.Create(tarea);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear la tarea: {ex.Message}");
                CargarProyectosYUsuarios();
                return View(tarea);
            }
        }

        // GET: TareasController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                var tarea = Crud<Tarea>.GetById(id);
                if (tarea == null)
                {
                    return NotFound();
                }

                CargarProyectosYUsuarios();
                return View(tarea);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // POST: TareasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Tarea tarea)
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                Crud<Tarea>.Update(id, tarea);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al editar la tarea: {ex.Message}");
                CargarProyectosYUsuarios();
                return View(tarea);
            }
        }

        // GET: TareasController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                var tarea = Crud<Tarea>.GetById(id);
                if (tarea == null)
                {
                    return NotFound();
                }
                return View(tarea);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        // POST: TareasController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                Crud<Tarea>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar la tarea: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        private void CargarProyectosYUsuarios()
        {
            Crud<Proyecto>.Token = User.FindFirst("JWTToken")?.Value;
            Crud<Usuario>.Token = User.FindFirst("JWTToken")?.Value;

            var proyectos = Crud<Proyecto>.GetAll();
            var usuarios = Crud<Usuario>.GetAll();

            ViewBag.Proyectos = proyectos.Select(p => new SelectListItem
            {
                Value = p.id.ToString(),
                Text = p.nombre
            }).ToList();

            ViewBag.Usuarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.id.ToString(),
                Text = u.nombre
            }).ToList();

            ViewBag.Prioridades = new List<SelectListItem>
            {
                new SelectListItem { Value = "Baja", Text = "Baja" },
                new SelectListItem { Value = "Media", Text = "Media" },
                new SelectListItem { Value = "Alta", Text = "Alta" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En Progreso", Text = "En Progreso" },
                new SelectListItem { Value = "Completada", Text = "Completada" }
            };
        }
    }
}
