using Dapper;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTareas.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SeguimientosController : ControllerBase
    {
        private DbConnection connection;

        public SeguimientosController(IConfiguration config)
        {
            var connString = "Data Source = (localdb)\\MSSQLLocalDB; DataBase = DBGestionTareasT; Integrated Security = True; ";
            connection = new SqlConnection(connString);
        }

        // GET: api/Proyectos
        [HttpGet]
        public IEnumerable<Seguimiento> Get()
        {
            var seguimientos = connection.Query<Seguimiento>("SELECT * FROM Seguimientos").ToList();
            return seguimientos;
        }

        // GET api/Proyectos/5
        [HttpGet("{id}")]
        public Seguimiento Get(int id)
        {
            var seguimiento = connection.QuerySingleOrDefault<Seguimiento>(
                "SELECT id, fecha, estadoAnterior, estadoNuevo, tareaId, usuarioId FROM Seguimientos WHERE id = @id",
                new { id = id }
            );
            return seguimiento ?? throw new KeyNotFoundException($"No se encontró el seguimiento con ID {id}");
        }

        // POST api/Proyectos
        [HttpPost]
        public Seguimiento Post([FromBody] Seguimiento seguimiento)
        {
            var sql = @"
                INSERT INTO Seguimientos (fecha, estadoAnterior, estadoNuevo, tareaId, usuarioId)
                VALUES (@fecha, @estadoAnterior, @estadoNuevo, @tareaId, @usuarioId);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = connection.QuerySingle<int>(sql, seguimiento);
            seguimiento.id = id;
            return seguimiento;
        }

        // PUT api/Proyectos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Seguimiento seguimiento)
        {
            connection.Execute(
                "UPDATE Seguimientos SET fecha = @fecha, estadoAnterior = @estadoAnterior, estadoNuevo = @estadoNuevo, tareaId = @tareaId, usuarioId = @usuarioId WHERE id = @id",
                new { seguimiento.fecha, seguimiento.estadoAnterior, seguimiento.estadoNuevo, seguimiento.tareaId, seguimiento.usuarioId, id }
            );  
        }

        // DELETE api/Proyectos/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            connection.Execute(
                "DELETE FROM Seguimientos WHERE id = @id",
                new { id = id }
            );
        }
    }
}
