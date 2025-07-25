using GestionTareas.API.Consumer;
using GestionTareas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTareas.MVC.Controllers
{
    public class ProyectosController : Controller
    {
        // GET: ProyectosController
        public ActionResult Index()
        {
            var proyectos = Crud<Proyecto>.GetAll();
            return View(proyectos);
        }

        // GET: ProyectosController/Details/5
        public ActionResult Details(int id)
        {
            var proyecto = Crud<Proyecto>.GetById(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        // GET: ProyectosController/Create
        public ActionResult Create()
        {
            CargarProyectos();
            return View();
        }

        // POST: ProyectosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proyecto proyecto)
        {
            try
            {
                Crud<Proyecto>.Create(proyecto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el proyecto: {ex.Message}");
                CargarProyectos();
                return View(proyecto);
            }
        }

        // GET: ProyectosController/Edit/5
        public ActionResult Edit(int id)
        {
            var proyecto = Crud<Proyecto>.GetById(id);
            CargarProyectos();
            return View(proyecto);
        }

        // POST: ProyectosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Proyecto proyecto)
        {
            try
            {
                Crud<Proyecto>.Update(id,proyecto);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", $"Error al editar el proyecto: {ex.Message}");
                CargarProyectos();
                return View(proyecto);
            }
        }

        // GET: ProyectosController/Delete/5
        public ActionResult Delete(int id)
        {
            var proyecto = Crud<Proyecto>.GetById(id);
            return View(proyecto);
        }

        // POST: ProyectosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Proyecto proyecto)
        {
            try
            {
                Crud<Proyecto>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar el proyecto: {ex.Message}");
                CargarProyectos();
                return View(proyecto);
            }
        }
        private void CargarProyectos()
        {
            var proyectos = Crud<Proyecto>.GetAll();
            ViewBag.Proyectos = proyectos.Select(d => new SelectListItem
            {
                Value = d.id.ToString(),
                Text = d.nombre
            }).ToList();
        }
    }
}
