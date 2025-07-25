using Dapper;
using GestionTareas.Models;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DbConnection _connection;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        //Post : api/Auth/LoginP
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var usuario = _connection.QueryFirstOrDefault<Usuario>(
                "SELECT * FROM Usuarios WHERE correo = @correo AND contrasena = @contrasena",
                new { model.Correo, model.Contrasena }
            );

            if (usuario == null)
                return Unauthorized("Credenciales inválidas");

            var token = GenerateJwtToken(usuario);

            return Ok(new UserResponse
            {
                Id = usuario.id,
                Nombre = usuario.nombre,
                Correo = usuario.correo,
                Token = token
            });
        }

        // POST: api/Auth Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.nombre) || string.IsNullOrEmpty(usuario.correo) || string.IsNullOrEmpty(usuario.contrasena))
            {
                return BadRequest("Invalid registration request.");
            }
            var existingUser = await _connection.QuerySingleOrDefaultAsync<Usuario>(
                "SELECT * FROM Usuarios WHERE correo = @Correo",
                new { Correo = usuario.correo }
            );
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }
            await _connection.ExecuteAsync(
                "INSERT INTO Usuarios (nombre, correo, contrasena) VALUES (@Nombre, @Correo, @Contrasena)",
                usuario
            );
            return Ok(new { Message = "Registration successful", Usuario = usuario });
        }

        private String GenerateJwtToken(Usuario usuario)
        {
            var key = System.Text.Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.nombre),
                    new Claim(ClaimTypes.Email, usuario.correo)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
