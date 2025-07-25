using Dapper;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestionTareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private DbConnection connection;

        public TareasController(IConfiguration config)
        {
            var connString = "Data Source = (localdb)\\MSSQLLocalDB; DataBase = DBGestionTareasT; Integrated Security = True; ";
            connection = new SqlConnection(connString);
        }

        // GET: api/Proyectos
        [HttpGet]
        public IEnumerable<Tarea> Get()
        {
            var tareas = connection.Query<Tarea>("SELECT * FROM Tareas").ToList();
            return tareas;
        }

        // GET api/Proyectos/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var tarea = connection.QuerySingleOrDefault<Tarea>(
                    "SELECT id, titulo, descripcion, fechaCreacion, fechaVencimiento, prioridad, estado, usuarioId, proyectoId FROM Tareas WHERE id = @id",
                    new { id = id }
                );

                if (tarea == null)
                    return NotFound($"No se encontró la tarea con ID {id}");

                return Ok(tarea);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST api/Proyectos
        [HttpPost]
        public Tarea Post([FromBody] Tarea tarea)
        {
            connection.Execute(
                "INSERT INTO Tareas (titulo, descripcion, fechaCreacion, fechaVencimiento, prioridad, estado, usuarioId, proyectoId) VALUES (@titulo, @descripcion, @fechaCreacion, @fechaVencimiento, @prioridad, @estado, @usuarioId, @proyectoId)",
                tarea
            );
            return tarea;
        }

        // PUT api/Proyectos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Tarea tarea)
        {
            connection.Execute(
                "UPDATE Tareas SET titulo = @titulo, descripcion = @descripcion, fechaCreacion = @fechaCreacion, fechaVencimiento = @fechaVencimiento, prioridad = @prioridad, estado = @estado, usuarioId = @usuarioId, proyectoId = @proyectoId WHERE id = @id",
                new { tarea.titulo, tarea.descripcion, tarea.fechaCreacion, tarea.fechaVencimiento, tarea.prioridad, tarea.estado, tarea.usuarioId, tarea.proyectoId, id }
            );
        }

        // DELETE api/Proyectos/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            connection.Execute(
                "DELETE FROM Tareas WHERE id = @id",
                new { id = id }
            );
        }
    }
}
