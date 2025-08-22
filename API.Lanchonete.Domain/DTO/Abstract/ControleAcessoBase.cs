using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Lanchonete.Domain.DTO.Abstract
{
    public class ControleAcessoBase
    {
        public int IdPerfil { get; set; }

        public string NomeTela { get; set; }

        public bool Permitido { get; set; }
    }
}
