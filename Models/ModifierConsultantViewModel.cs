using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GestionTM.Models
{
    public class ModifierConsultantViewModel
    {
        public Consultant Consultant { get; set; }
        public List<SelectListItem> ValidatorSelectList { get; set; }
    }
}

