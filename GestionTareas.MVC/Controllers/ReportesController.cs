using GestionTareas.API.Consumer;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTareas.MVC.Controllers
{
    public class ReportesController : Controller
    {
        [Authorize]
        // GET: ReportesController
        [HttpGet]
        public ActionResult Index(string criterio = "estado", string valor = "")
        {
            try
            {
                var token = User.FindFirst("JWTToken")?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "AuthV");
                }

                Crud<Tarea>.Token = token;
                Crud<Proyecto>.Token = token;
                Crud<Usuario>.Token = token;
                Crud<Seguimiento>.Token = token;

                var tareas = Crud<Tarea>.GetAll();
                var proyectos = Crud<Proyecto>.GetAll();
                var usuarios = Crud<Usuario>.GetAll();
                var seguimientos = Crud<Seguimiento>.GetAll();

                var seguimientosEnriquecidos = seguimientos.Select(s => new
                {
                    s.id,
                    s.descripcion,
                    s.fecha,
                    s.estadoAnterior,
                    s.estadoNuevo,
                    TareaTitulo = tareas.FirstOrDefault(t => t.id == s.tareaId)?.titulo ?? "Tarea no encontrada",
                    UsuarioNombre = usuarios.FirstOrDefault(u => u.id == s.usuarioId)?.nombre ?? "Usuario no encontrado"
                })
                .OrderByDescending(s => s.fecha)
                .ToList();

                ViewBag.TareasPorEstado = tareas.GroupBy(t => t.estado)
                                               .ToDictionary(g => g.Key, g => g.ToList());

                ViewBag.TareasPorPrioridad = tareas.GroupBy(t => t.prioridad)
                                                  .ToDictionary(g => g.Key, g => g.ToList());

                ViewBag.TareasPorFecha = tareas.OrderBy(t => t.fechaVencimiento)
                                             .ToList();

                
                ViewBag.ValoresEstado = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                    new SelectListItem { Value = "En Progreso", Text = "En Progreso" },
                    new SelectListItem { Value = "Completada", Text = "Completada" }
                };

                ViewBag.ValoresPrioridad = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Baja", Text = "Baja" },
                    new SelectListItem { Value = "Media", Text = "Media" },
                    new SelectListItem { Value = "Alta", Text = "Alta" }
                };

                ViewBag.ValoresProyecto = proyectos.Select(p => new SelectListItem
                {
                    Value = p.id.ToString(),
                    Text = p.nombre
                }).ToList();

                ViewBag.ValoresUsuario = usuarios.Select(u => new SelectListItem
                {
                    Value = u.id.ToString(),
                    Text = u.nombre
                }).ToList();

                ViewBag.Seguimientos = seguimientosEnriquecidos;
                ViewBag.Criterio = criterio;
                ViewBag.Valor = valor;

                return View(tareas);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

        public ActionResult Buscar(string criterio, string valor)
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

                if (!string.IsNullOrEmpty(criterio) && !string.IsNullOrEmpty(valor))
                {
                    switch (criterio.ToLower())
                    {
                        case "estado":
                            tareas = tareas.Where(t => t.estado.Contains(valor, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                        case "prioridad":
                            tareas = tareas.Where(t => t.prioridad.Contains(valor, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                        case "proyecto":
                            tareas = tareas.Where(t => t.proyectoId.ToString() == valor).ToList();
                            break;
                        case "usuario":
                            tareas = tareas.Where(t => t.usuarioId.ToString() == valor).ToList();
                            break;
                    }
                }

                return PartialView("_TareasListaPartial", tareas);
            }
            catch
            {
                return RedirectToAction("Login", "AuthV");
            }
        }

    }
}
