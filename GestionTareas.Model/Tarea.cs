using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTareas.Models
{
    public class Tarea
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public string prioridad { get; set; }
        public string estado { get; set; }
        public int usuarioId { get; set; }
        public int proyectoId { get; set; }
    }
}
