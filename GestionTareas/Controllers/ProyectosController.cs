using Dapper;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestionTareas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private DbConnection connection;

        public ProyectosController(IConfiguration config)
        {
            var connString = "Data Source = (localdb)\\MSSQLLocalDB; DataBase = DBGestionTareasT; Integrated Security = True; ";
            connection = new SqlConnection(connString);
        }

        // GET: api/Proyectos
        [HttpGet]
        public IEnumerable<Proyecto> Get()
        {
            var proyectos = connection.Query<Proyecto>("SELECT * FROM Proyectos").ToList();
            return proyectos;
        }

        // GET api/Proyectos/5
        [HttpGet("{id}")]
        public Proyecto Get(int id)
        {
            var proyecto = connection.QuerySingleOrDefault<Proyecto>(
                "SELECT id, nombre, descripcion, fechaInicio, fechaFin FROM Proyectos WHERE id = @id",
                new { id = id }
            );
            return proyecto;
        }

        // POST api/Proyectos
        [HttpPost]
        public Proyecto Post([FromBody] Proyecto proyecto)
        {
            connection.Execute(
                "INSERT INTO Proyectos (nombre, descripcion, fechaInicio, fechaFin) VALUES (@nombre, @descripcion, @fechaInicio, @fechaFin)",
                proyecto
            );
            return proyecto;
        }

        // PUT api/Proyectos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Proyecto proyecto)
        {
            connection.Execute(
                "UPDATE Proyectos SET nombre = @nombre, descripcion = @descripcion, fechaInicio = @fechaInicio, fechaFin = @fechaFin WHERE id = @id",
                new { proyecto.nombre, proyecto.descripcion, proyecto.fechaInicio, proyecto.fechaFin, id }
            );
        }

        // DELETE api/Proyectos/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            connection.Execute("DELETE FROM Proyectos WHERE id = @id", new { id = id });
        }
    }
}
