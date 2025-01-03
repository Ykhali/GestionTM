using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GestionTM.Models
{
    public class UpdateValidateurViewModel
    {
        public Validateur Validateur { get; set; }
        public List<SelectListItem> ConsultantSelectList { get; set; }
    }
}


