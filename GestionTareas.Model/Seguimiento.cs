using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTareas.Models
{
    public class Seguimiento
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string estadoAnterior { get; set; }
        public string estadoNuevo { get; set; }
        public int tareaId { get; set; }
        public int usuarioId { get; set; }
    }
}
