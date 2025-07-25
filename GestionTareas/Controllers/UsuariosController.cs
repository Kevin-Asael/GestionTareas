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
    public class UsuariosController : ControllerBase
    {
        private DbConnection connection;

        public UsuariosController(IConfiguration config)
        {
            var connString = "Data Source = (localdb)\\MSSQLLocalDB; DataBase = DBGestionTareasT; Integrated Security = True; ";
            connection = new SqlConnection(connString);
        }

        // GET: api/Proyectos
        [HttpGet]
        public IEnumerable<Usuario> Get()
        {
            var usuarios = connection.Query<Usuario>("SELECT * FROM Usuarios").ToList();
            return usuarios;
        }

        // GET api/Proyectos/5
        [HttpGet("{id}")]
        public Usuario Get(int id)
        {
            var usuario = connection.QuerySingleOrDefault<Usuario>(
                "SELECT id, nombre, correo, contrasena FROM Usuarios WHERE id = @id",
                new { id = id }
            );
            return usuario;
        }

        // POST api/Proyectos
        [HttpPost]
        public Usuario Post([FromBody] Usuario usuario)
        {
            connection.Execute(
                "INSERT INTO Usuarios (nombre, correo, contrasena) VALUES (@nombre, @correo, @contrasena)",
                usuario
            );
            return usuario;
        }

        // PUT api/Proyectos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Usuario usuario)
        {
            connection.Execute(
                "UPDATE Usuarios SET nombre = @nombre, correo = @correo, contrasena = @contrasena WHERE id = @id",
                new { usuario.nombre, usuario.correo, usuario.contrasena, id }
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
